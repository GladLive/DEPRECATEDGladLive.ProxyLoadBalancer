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
		private IResponsePayloadHandlerService<AuthServiceClientPeer> responseHandlerService { get; }

		/// <summary>
		/// Creates a new object that represents an auth service connection.
		/// </summary>
		/// <param name="logger">Logging service for this session.</param>
		/// <param name="sender">Network message sending service.</param>
		/// <param name="details">Connection details for this specific incoming game server session.</param>
		/// <param name="netMessageSubService">Subscription service for incoming messages.</param>
		/// <param name="disconnectHandler">Disconnection handler for the session.</param>
		/// <param name="responseHandler">Request payload handler for the session.</param>
		public AuthServiceClientPeer(ILog logger, INetworkMessageSender messageSender, IConnectionDetails details, 
			INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, IResponsePayloadHandlerService<AuthServiceClientPeer> responseHandler) 
			: base(logger, messageSender, details, subService, disconnectHandler)
		{
			//We check logger null because we want to log now
			Throw<ArgumentNullException>.If.IsNull(logger, nameof(logger), $"Logging service provided must be non-null.");
			Throw<ArgumentNullException>.If.IsNull(responseHandler, nameof(responseHandler), $"Response handling service provided must be non-null.");


			//the authservice doesn't really 'connect'
			logger.Debug($"An {nameof(AuthServiceClientPeer)} was created but not connected yet.");

			//We only have a response handler; we won't be provided with an event handler (maybe in the future) because we don't want to handle any events.
			responseHandlerService = responseHandler;
		}

		/// <summary>
		/// Called internally when an event is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="payload">Incoming payload.</param>
		/// <param name="parameters">Parameters the message was sent with (Null right now in this implementation).</param>
		protected override void OnReceiveEvent(PacketPayload payload, IMessageParameters parameters)
		{
			//Right now web clients should recieve events
			//There just isn't an implementation for it
			Logger.Error($"{nameof(AuthServiceClientPeer)} recieved an event in {nameof(OnReceiveEvent)} but should not recieve events.");
		}

		/// <summary>
		/// Called internally when a response is recieved. (setup using the subscription service)
		/// </summary>
		/// <param name="payload">Incoming payload.</param>
		/// <param name="parameters">Parameters the message was sent with (Null right now in this implementation).</param>
		protected override void OnReceiveResponse(PacketPayload payload, IMessageParameters parameters)
		{
			//As of right now the implementation for the web client cannot provide message parameters.
			//As such we should not check them, they're likely to be null until their implemented

			//So, just check payloads
			Throw<ArgumentNullException>.If
				.IsNull(payload, nameof(payload), $"This indicates a critical error. The GladNet API has provided a null {nameof(PacketPayload)} to the {nameof(AuthServiceClientPeer.OnReceiveResponse)} method.");

			if(!responseHandlerService.TryProcessPayload(payload, parameters, this))
			{
				//TODO: When message parameters are implemented for GladNet.ASP.Client we should include them in the logging statement.
				Logger.Warn($"Failed to handle response message in {nameof(OnReceiveResponse)} for Type {nameof(AuthServiceClientPeer)} with PayloadType {payload.GetType().Name}.");
			}
		}

		//implementation of: IUserAuthService
		public AuthServiceState TryAuthenticateUser(IPAddress ipOfUser, string userLoginString, byte[] userPassword)
		{
			if (this.CanSend(OperationType.Request) && this.Status == NetStatus.Connected)
			{
				throw new NotImplementedException();
			}
			else
				return AuthServiceState.Unavailable;
		}
	}
}
