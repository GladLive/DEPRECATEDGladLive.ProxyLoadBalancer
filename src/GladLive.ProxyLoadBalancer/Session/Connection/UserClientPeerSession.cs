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
	/// Represents a user session on the server.
	/// </summary>
	public class UserClientPeerSession : ClientPeerSession
	{
		public UserClientPeerSession(ILog logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler) 
			: base(logger, sender, details, subService, disconnectHandler)
		{
			logger.Debug("Created new client session.");
		}

		protected override void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters)
		{
			//We're not interested in unencrypted messages on the ProxyLoadBalancing server
			if (!parameters.Encrypted)
				return;
			else
				Logger.WarnFormat("Client: {0} at IP {1} tried to send unencrypted payload Type: {2}", PeerDetails.ConnectionID, PeerDetails.RemoteIP.ToString(), payload.GetType());
		}
	}
}
