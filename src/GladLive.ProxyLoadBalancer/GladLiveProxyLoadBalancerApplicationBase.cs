using GladNet.PhotonServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet.Common;
using GladNet.Serializer;
using GladNet.Server.Common;
using Autofac;
using System.IO;
using GladLive.ProxyLoadBalancer.Settings;
using GladLive.Server.Common;
using Autofac.Core;

namespace GladLive.ProxyLoadBalancer
{
	public class GladLiveProxyLoadBalancerApplicationBase : GladNetAppBase
	{
		/// <summary>
		/// Deserialization strategy for incoming messages.
		/// </summary>
		public override IDeserializerStrategy Deserializer { get { return appBaseContainer.Resolve<IDeserializerStrategy>(); } protected set { } }

		/// <summary>
		/// Serialization strategy for outgoing messages.
		/// </summary>
		public override ISerializerStrategy Serializer { get { return appBaseContainer.Resolve<ISerializerStrategy>(); } protected set { } }

		/// <summary>
		/// Application logging service.
		/// </summary>
		protected override ILog AppLogger { get; set; }

		/// <summary>
		/// IoC container type resolver.
		/// </summary>
		private IContainer appBaseContainer;

		//This is NOT the UserClientPeer. ClientPeerSession is a GladNet type.
		private IPeerFactoryService<ClientPeerSession, ProxySessionType> peerFactory;

		public override ServerPeer CreateServerPeer(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			//This shouldn't be called by the ProxyLoadBalancing server
			AppLogger.ErrorFormat("Outgoing connection attempt on Proxy to IP {0} Port {1}. Proxy should not be connecting to other peers", details.RemoteIP, details.RemotePort);

			return null;
		}

		public override ServerPeerSession CreateServerPeerSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			throw new NotImplementedException("This is deprecated and will be removed. Somehow a someone tried to create a peer session.");
		}

		protected override ClientPeerSession CreateClientSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			return peerFactory.Create(sender, details, subService, disconnectHandler);
		}

		protected override void Setup()
		{
			//Setup logging first
			AppLogger = new PhotonServerLog4NetCommonLoggingILogAdapter(this.ApplicationRootPath, this.ApplicationName, this.BinaryPath);

			//We should setup AutoFac IoC with the dependencies it'll need to be resolving.
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterAssemblyModules(typeof(GladLiveProxyLoadBalancerApplicationBase).Assembly);

			//Register the app-wide logging instance
			builder.RegisterInstance(AppLogger)
				.As<ILog>()
				.SingleInstance();

			//Builds out the IoC container
			appBaseContainer = builder.Build();

			AppLogger.DebugFormat("GLADLIVE: {0} {1} complete.", nameof(GladLiveProxyLoadBalancerApplicationBase), nameof(Setup));

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
	}
}
