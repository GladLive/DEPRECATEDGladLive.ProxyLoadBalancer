using GladNet.PhotonServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet.Common;
using GladNet.Serializer;
using Autofac;
using System.IO;
using GladLive.ProxyLoadBalancer.Settings;
using GladLive.Server.Common;
using Autofac.Core;
using GladNet.Serializer.Protobuf;
using GladNet.Engine.Common;
using GladNet.Engine.Server;

namespace GladLive.ProxyLoadBalancer
{
	public class GladLiveProxyLoadBalancerApplicationBase : GladNetAppBase<ProtobufnetSerializerStrategy, ProtobufnetDeserializerStrategy, ProtobufnetRegistry>
	{
		/// <summary>
		/// IoC container type resolver.
		/// </summary>
		private IContainer appBaseContainer;

		//This is NOT the UserClientPeer. ClientPeerSession is a GladNet type.
		private IPeerFactoryService<ClientPeerSession, ProxySessionType> peerFactory;

		protected override void ServerSetup()
		{
			//We should setup AutoFac IoC with the dependencies it'll need to be resolving.
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterAssemblyModules(typeof(GladLiveProxyLoadBalancerApplicationBase).Assembly);

			//Register the app-wide logging instance
			builder.RegisterInstance(AppLogger)
				.As<ILog>()
				.SingleInstance();

			//Builds out the IoC container
			appBaseContainer = builder.Build();

			AppLogger.DebugFormat("GLADLIVE: {0} {1} complete.", nameof(GladLiveProxyLoadBalancerApplicationBase), nameof(ServerSetup));

			InitServices();
		}

		public void InitServices()
		{
			peerFactory = appBaseContainer.Resolve<IPeerFactoryService<ClientPeerSession, ProxySessionType>>();
			AppLogger.DebugFormat("GLADLIVE: {0} {1} complete.", nameof(GladLiveProxyLoadBalancerApplicationBase), nameof(InitServices));
		}

		protected override bool ShouldServiceIncomingPeerConnect(IConnectionDetails details)
		{
			return peerFactory.CanCreate(details);
		}

		protected override void TearDown()
		{
			AppLogger.InfoFormat("{0} is tearing down server instance.", nameof(GladLiveProxyLoadBalancerApplicationBase));
		}

		protected override ClientPeerSession CreateClientSession(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routeBack)
		{
			return peerFactory.Create(sender, details, subService, disconnectHandler, routeBack);
		}

		public override ClientPeer CreateServerPeer(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routeBack)
		{
			//This shouldn't be called by the ProxyLoadBalancing server
			AppLogger.ErrorFormat("Outgoing connection attempt on Proxy to IP {0} Port {1}. Proxy should not be connecting to other peers", details.RemoteIP, details.RemotePort);

			return null;
		}

		protected override void SetupSerializationRegistration(ISerializerRegistry serializationRegistry)
		{
			throw new NotImplementedException();
		}

		protected override ILog SetupLogging()
		{
			return new PhotonServerLog4NetCommonLoggingILogAdapter(this.ApplicationRootPath, this.ApplicationName, this.BinaryPath);
		}
	}
}
