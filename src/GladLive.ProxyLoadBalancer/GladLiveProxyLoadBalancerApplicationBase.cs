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

namespace GladLive.ProxyLoadBalancer
{
	public class GladLiveProxyLoadBalancerApplicationBase : GladNetAppBase
	{
		public override IDeserializerStrategy Deserializer { get { return appBaseContainer.Resolve<IDeserializerStrategy>(); } set { } }

		public override ISerializerStrategy Serializer { get { return appBaseContainer.Resolve<ISerializerStrategy>(); } set { } }

		protected override ILog AppLogger { get; set; }

		private IContainer appBaseContainer;

		public override ServerPeer CreateServerPeer(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			throw new NotImplementedException();
		}

		public override ServerPeerSession CreateServerPeerSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			throw new NotImplementedException();
		}

		protected override ClientPeerSession CreateClientSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			throw new NotImplementedException();
		}

		protected override void Setup()
		{
			//We should setup AutoFac IoC with the dependencies it'll need to be resolving.
			ContainerBuilder builder = new ContainerBuilder();

			//Registers the protobuf serializer type for serialization
			builder.RegisterType<GladNet.Serializer.Protobuf.ProtobufnetSerializerStrategy>()
				.As<ISerializerStrategy>()
				.InstancePerDependency();

			//Registers the protobuf deserializer type for deserialization
			builder.RegisterType<GladNet.Serializer.Protobuf.ProtobufnetDeserializerStrategy>()
				.As<IDeserializerStrategy>()
				.InstancePerDependency();
		}

		protected override bool ShouldServiceIncomingPeerConnect(IConnectionDetails details)
		{
			throw new NotImplementedException();
		}

		protected override void TearDown()
		{
			//throw new NotImplementedException();
		}
	}
}
