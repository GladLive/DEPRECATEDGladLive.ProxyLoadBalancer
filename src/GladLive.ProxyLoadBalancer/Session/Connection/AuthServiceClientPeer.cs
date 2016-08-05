using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Easyception;
using GladLive.Common;
using System.Net;
using GladLive.Web.Payloads.Authentication;
using GladNet.Engine.Common;
using GladNet.Payload;
using GladNet.Message;
using GladNet.Message.Handlers;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Represents an outgoing client connection to the auth service.
	/// </summary>
	public class AuthServiceClientPeer : ClientPeer, IUserAuthService
	{
		/// <summary>
		/// Response payload handling service for <see cref="PacketPayload"/> and <see cref="AuthServiceClientPeer"/> pairs.
		/// </summary>
		private IResponseMessageHandlerService<AuthServiceClientPeer> responseHandlerService { get; }

		/// <summary>
		/// Creates a new object that represents an auth service connection.
		/// </summary>
		/// <param name="logger">Logging service for this session.</param>
		/// <param name="sender">Network message sending service.</param>
		/// <param name="details">Connection details for this specific incoming game server session.</param>
		/// <param name="netMessageSubService">Subscription service for incoming messages.</param>
		/// <param name="disconnectHandler">Disconnection handler for the session.</param>
		/// <param name="responseHandler">Request payload handler for the session.</param>
		public AuthServiceClientPeer(ILog logger, INetworkMessageRouterService messageSender, IConnectionDetails details, 
			INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routebackService, IResponseMessageHandlerService<AuthServiceClientPeer> responseHandler) 
			: base(logger, messageSender, details, subService, disconnectHandler, routebackService)
		{
			//We check logger null because we want to log now
			Throw<ArgumentNullException>.If.IsNull(logger)?.Now(nameof(logger), $"Logging service provided must be non-null.");
			Throw<ArgumentNullException>.If.IsNull(responseHandler)?.Now(nameof(responseHandler), $"Response handling service provided must be non-null.");


			//the authservice doesn't really 'connect'
			logger.Debug($"An {nameof(AuthServiceClientPeer)} was created but not connected yet.");

			//We only have a response handler; we won't be provided with an event handler (maybe in the future) because we don't want to handle any events.
			responseHandlerService = responseHandler;
		}

		/// <summary>
		/// Called internally when an event is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="message">Incoming event message.</param>
		/// <param name="parameters">Parameters the message was sent with (Null right now in this implementation).</param>
		protected override void OnReceiveEvent(IEventMessage message, IMessageParameters parameters)
		{
			//Right now web clients should recieve events
			//There just isn't an implementation for it
			Logger.Error($"{nameof(AuthServiceClientPeer)} recieved an event in {nameof(OnReceiveEvent)} but should not recieve events.");
		}

		/// <summary>
		/// Called internally when a response is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="message">Incoming message.</param>
		/// <param name="parameters">Parameters the message was sent with (Null right now in this implementation).</param>
		protected override void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters)
		{
			//As of right now the implementation for the web client cannot provide message parameters.
			//As such we should not check them, they're likely to be null until their implemented

			//So, just check payloads
			Throw<ArgumentNullException>.If
				.IsNull(message)?.Now(nameof(message), $"This indicates a critical error. The GladNet API has provided a null {nameof(IResponseMessage)} to the {nameof(AuthServiceClientPeer.OnReceiveResponse)} method.");

			if(!responseHandlerService.TryProcessMessage(message, parameters, this))
			{
				//TODO: When message parameters are implemented for GladNet.ASP.Client we should include them in the logging statement.
				Logger.Warn($"Failed to handle response message in {nameof(OnReceiveResponse)} for Type {nameof(AuthServiceClientPeer)} with PayloadType {message.GetType().Name}.");
			}
		}

		//implementation of: IUserAuthService
		public void TryAuthenticateUser(IPAddress ipOfUser, string userLoginString, byte[] userPassword)
		{
			if (CheckServiceState() == AuthServiceState.Available)
			{
				//Build the GladNet payload for the AuthRequest to be send to the auth service.
				AuthRequest request = new AuthRequest(ipOfUser, new LoginDetails(userLoginString, userPassword));

				//Right now the web service is TCP, obviously, and only really can do reliable unordered without discard.
				//This is a design fault of GladNet2 as it was never intended to be built on top of TCP/Web
				//Also, if the SendResult is invalid we know something is wrong otherwise it's queued/sent.
				SendRequest(request, DeliveryMethod.ReliableUnordered, true, 0);
			}
		}

		public AuthServiceState CheckServiceState()
		{
			if (this.CanSend(OperationType.Request) && this.Status == NetStatus.Connected)
				return AuthServiceState.Available;
			else
				return AuthServiceState.Unavailable;
		}
	}
}
