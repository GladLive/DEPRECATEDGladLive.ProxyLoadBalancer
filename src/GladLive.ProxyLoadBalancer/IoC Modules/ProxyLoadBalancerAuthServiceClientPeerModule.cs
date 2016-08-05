using Autofac;
using Autofac.Core;
using Common.Logging;
using GladLive.Common;
using GladLive.ProxyLoadBalancer.Settings;
using GladNet.ASP.Client.Lib;
using GladNet.ASP.Client.RestSharp;
using GladNet.Common;
using GladNet.Engine.Common;
using GladNet.Message.Handlers;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerAuthServiceClientPeerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//We need to create and manually resolve the dependencies for the AuthService since it uses a different GladNet2 implementation as the other
			//session objects
			builder.Register(con =>
			{
				IConnectionDetails details = new GladNet.PhotonServer.Server.PhotonServerIConnectionDetailsAdapter(IPAddress.Any.ToString(), 5, 5); //this is a hack. We need an AUID

				//Publisher: INetworkReciever, SubService and etc.
				NetworkMessagePublisher publisher = new NetworkMessagePublisher();

				//TODO: Change this to a task-based/async multithreaded strategy when it's implemented
				//This is the message sending service for web clients
				//It doesn't usually need a publisher but right now we use a singlethreaded blocking implementation

				//TODO: Make it so that we don't need to resolve the routeback service.
				WebPeerClientMessageSender webMessageSender = new WebPeerClientMessageSender(new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(OutboundConnectionSettings.Default.AuthServiceIP, con.Resolve<IDeserializerStrategy>(), publisher, details.ConnectionID, con.Resolve<INetworkMessageRouteBackService>()));

				//Because this differs in GladNet implementation we must resolve some of these depencencies manually
				AuthServiceClientPeer peer = new AuthServiceClientPeer(con.Resolve<ILog>(), webMessageSender, new WebClientPeerDetails(OutboundConnectionSettings.Default.AuthServiceIP, -1, details.ConnectionID), publisher, con.Resolve<IDisconnectionServiceHandler>(), con.Resolve<INetworkMessageRouteBackService>(), con.Resolve<IResponseMessageHandlerService<AuthServiceClientPeer>>());

				//TODO: Use ping the webserver so we can give a real status
				//Because this is web service we don't really connect to it like the other RUDP connections so we should spoof the NetStatus
				publisher.OnNetworkMessageReceive(new GladNet.PhotonServer.Common.PhotonStatusMessageAdapter(NetStatus.Connected), null);

				return peer;
			})
				.AsSelf()
				.SingleInstance();
		}
	}
}
