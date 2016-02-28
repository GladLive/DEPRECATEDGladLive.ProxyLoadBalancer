using GladNet.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common;
using Common.Logging;
using GladLive.Server.Common;
using GladLive.Common;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Represents an AuthService session connected to the server.
	/// </summary>
	public class AuthServicePeerSession : ServerPeerSession, IElevatableSession
	{
		public Guid UniqueAuthToken { get; set; }

		private IRequestPayloadHandlerService<AuthServicePeerSession> requestHandler { get; }

		public AuthServicePeerSession(ILog logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService, 
			IDisconnectionServiceHandler disconnectHandler, IRequestPayloadHandlerService<AuthServicePeerSession> requestHandlerService) 
				: base(logger, sender, details, netMessageSubService, disconnectHandler)
		{
			logger.Debug("Created new authservice.");

			requestHandler = requestHandlerService;
		}

		protected override void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters)
		{
			//We're not interested in unencrypted messages on the ProxyLoadBalancing server
			/*if (!parameters.Encrypted)
				return;
			else
				Logger.WarnFormat("AuthService: {0} at IP {1} tried to send unencrypted payload Type: {2}", PeerDetails.ConnectionID, PeerDetails.RemoteIP.ToString(), payload.GetType());*/

			Logger.Debug("Recieved a message.");

			//Pass this message to the handlers
			requestHandler.TryProcessPayload(payload, parameters, this);
		}
	}
}
