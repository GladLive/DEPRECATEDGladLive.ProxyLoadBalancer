using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common;
using Common.Logging;
using GladLive.Common;
using Easyception;
using GladNet.Message.Handlers;
using GladNet.Engine.Server;
using GladNet.Engine.Common;
using GladNet.Message;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Represents a user session on the server.
	/// </summary>
	public class UserClientPeerSession : ClientPeerSession
	{
		/// <summary>
		/// Request payload handler service.
		/// </summary>
		private IRequestMessageHandlerService<UserClientPeerSession> requestHandlerService { get; }

		/// <summary>
		/// Creates a new object that represents a client/user connection.
		/// </summary>
		/// <param name="logger">Logging service for this session.</param>
		/// <param name="sender">Network message sending service.</param>
		/// <param name="details">Connection details for this specific incoming game server session.</param>
		/// <param name="netMessageSubService">Subscription service for incoming messages.</param>
		/// <param name="disconnectHandler">Disconnection handler for the session.</param>
		/// <param name="requestHandler">Request payload handler for the session.</param>
		public UserClientPeerSession(ILog logger, INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler,
			INetworkMessageRouteBackService routebackService, IRequestMessageHandlerService<UserClientPeerSession> requestHandler)
			: base(logger, sender, details, subService, disconnectHandler, routebackService)
		{
			//We check logger null because we want to log now
			Throw<ArgumentNullException>.If.IsNull(logger)?.Now(nameof(logger), $"Logging service provided must be non-null.");
			Throw<ArgumentNullException>.If.IsNull(requestHandler)?.Now(nameof(requestHandler), $"Request handling service provided must be non-null.");

			logger.Debug("Created new client session.");

			requestHandlerService = requestHandler;
		}

		/// <summary>
		/// Called internally when a request is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="message">Incoming payload.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected override void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters)
		{
			Throw<ArgumentNullException>.If
				.IsNull(message)?.Now(nameof(message), $"This indicates a critical error. The GladNet API has provided a null {nameof(IRequestMessage)} to the {nameof(OnReceiveRequest)} method.");

			Throw<ArgumentNullException>.If
				.IsNull(parameters)?.Now(nameof(parameters), $"This indicates a critical error. The GladNet API has provided a null {nameof(IMessageParameters)} to the {nameof(OnReceiveRequest)} method.");

			//We're not interested in unencrypted messages on the ProxyLoadBalancing server
			if (!parameters.Encrypted)
			{
				Logger.WarnFormat("Client: {0} at IP {1} tried to send unencrypted payload Type: {2}", PeerDetails.ConnectionID, PeerDetails.RemoteIP, message.GetType());
				return;
			}
				

			//Sends off the payload to the provided handlers.
			requestHandlerService.TryProcessMessage(message, parameters, this);
		}
	}
}
