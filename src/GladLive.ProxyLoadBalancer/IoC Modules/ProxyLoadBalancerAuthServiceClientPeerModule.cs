using Autofac;
using Autofac.Core;
using Common.Logging;
using GladLive.Common;
using GladLive.ProxyLoadBalancer.Settings;
using GladNet.ASP.Client.Lib;
using GladNet.ASP.Client.RestSharp;
using GladNet.Common;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
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
				//Publisher: INetworkReciever, SubService and etc.
				NetworkMessagePublisher publisher = new NetworkMessagePublisher();

				//TODO: Change this to a task-based/async multithreaded strategy when it's implemented
				//This is the message sending service for web clients
				//It doesn't usually need a publisher but right now we use a singlethreaded blocking implementation
				WebPeerClientMessageSender webMessageSender = new WebPeerClientMessageSender(new RestSharpCurrentThreadEnqueueRequestHandlerStrategy(OutboundConnectionSettings.Default.AuthServiceIP, con.Resolve<IDeserializerStrategy>(), publisher));

				//Because this differs in GladNet implementation we must resolve some of these depencencies manually
				return new AuthServiceClientPeer(con.Resolve<ILog>(), webMessageSender, new WebClientPeerDetails(OutboundConnectionSettings.Default.AuthServiceIP, -1, 0), publisher, con.Resolve<IDisconnectionServiceHandler>(), con.Resolve<IResponsePayloadHandlerService<AuthServiceClientPeer>>());
			})
				.AsSelf()
				.SingleInstance();
		}
	}
}
