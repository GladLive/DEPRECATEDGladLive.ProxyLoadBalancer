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
	/// Represents an GameServer session connected to the server.
	/// </summary>
	public class GameServicePeerSession : ServerPeerSession, IElevatableSession
	{
		/// <summary>
		/// Token used for elevated priviliges.
		/// </summary>
		public Guid UniqueAuthToken { get; set; }

		/// <summary>
		/// Request payload handler service.
		/// </summary>
		private IRequestPayloadHandlerService<GameServicePeerSession> requestHandler { get; }

		/// <summary>
		/// Creates a new object that represents a game server session.
		/// </summary>
		/// <param name="logger">Logging service for this session.</param>
		/// <param name="sender">Network message sending service.</param>
		/// <param name="details">Connection details for this specific incoming game server session.</param>
		/// <param name="netMessageSubService">Subscription service for incoming messages.</param>
		/// <param name="disconnectHandler">Disconnection handler for the session.</param>
		/// <param name="requestHandlerService">Request payload handler for the session.</param>
		public GameServicePeerSession(ILog logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService, 
			IDisconnectionServiceHandler disconnectHandler, IRequestPayloadHandlerService<GameServicePeerSession> requestHandlerService) 
				: base(logger, sender, details, netMessageSubService, disconnectHandler)
		{
			logger.Debug("Created new a new gameserver service peer session.");

			requestHandler = requestHandlerService;
		}

		/// <summary>
		/// Called internally when a request is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="payload">Incoming payload.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected override void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters)
		{
			//We're not interested in unencrypted messages on the ProxyLoadBalancing server
			if (!parameters.Encrypted)
				return;
			else
				Logger.WarnFormat("GameService: {0} at IP {1} tried to send unencrypted payload Type: {2}", PeerDetails.ConnectionID, PeerDetails.RemoteIP.ToString(), payload.GetType());

			Logger.Debug("GameService Recieved a message.");

			//Pass this message to the handlers
			requestHandler.TryProcessPayload(payload, parameters, this);
		}
	}
}
