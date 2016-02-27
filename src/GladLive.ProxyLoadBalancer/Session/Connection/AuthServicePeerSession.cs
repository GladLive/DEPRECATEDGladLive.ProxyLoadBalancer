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
	/// <summary>
	/// Represents an AuthService session connect to the server.
	/// </summary>
	public class AuthServicePeerSession : ServerPeerSession
	{
		public AuthServicePeerSession(ILog logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService, IDisconnectionServiceHandler disconnectHandler) 
			: base(logger, sender, details, netMessageSubService, disconnectHandler)
		{
			logger.Debug("Created new authservice.");
		}

		protected override void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters)
		{
			throw new NotImplementedException();
		}
	}
}
