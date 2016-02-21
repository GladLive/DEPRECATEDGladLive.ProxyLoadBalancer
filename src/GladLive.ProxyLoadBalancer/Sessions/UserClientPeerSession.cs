using GladNet.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common;
using Common.Logging;

namespace GladLive.ProxyLoadBalancer
{
	public class UserClientPeerSession : ClientPeerSession
	{
		public UserClientPeerSession(ILog logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler) 
			: base(logger, sender, details, subService, disconnectHandler)
		{
			//For testing
			throw new EncoderFallbackException();
		}

		protected override void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters)
		{
			//We're not interested in unencrypted messages on the ProxyLoadBalancing server
			if (!parameters.Encrypted) //TODO: Some logging
				return;
		}
	}
}
