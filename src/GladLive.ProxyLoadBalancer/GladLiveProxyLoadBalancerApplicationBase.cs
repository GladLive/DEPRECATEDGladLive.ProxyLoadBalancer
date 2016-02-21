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

namespace GladLive.ProxyLoadBalancer
{
	public class GladLiveProxyLoadBalancerApplicationBase : GladNetAppBase
	{
		public override IDeserializerStrategy Deserializer
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public override ISerializerStrategy Serializer
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		protected override ILog AppLogger
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

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
			//throw new NotImplementedException();
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
