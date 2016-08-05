using Common.Logging;
using GladLive.Common;
using GladNet.Common;
using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Message.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	//TODO: Find a way to order handlers by priority
	public class PayloadDebugLoggingHandler : IRequestMessageHandler<INetPeer>, IEventMessageHandler<INetPeer>, IResponseMessageHandler<INetPeer>, IClassLogger
	{
		public ILog Logger { get; }

		public PayloadDebugLoggingHandler(ILog logger)
		{
			Logger = logger;
		}

		public bool TryProcessMessage(IRequestMessage message, IMessageParameters parameters, INetPeer peer)
		{
			return HandleDebug(message, parameters, peer);
		}

		public bool TryProcessMessage(IEventMessage message, IMessageParameters parameters, INetPeer peer)
		{
			return HandleDebug(message, parameters, peer);

		}

		public bool TryProcessMessage(IResponseMessage message, IMessageParameters parameters, INetPeer peer)
		{
			return HandleDebug(message, parameters, peer);

		}

		public bool HandleDebug(INetworkMessage message, IMessageParameters parameters, INetPeer peer)
		{
			Logger.Debug($"Recieved Payload Type: {message?.Payload?.Data?.GetType()} from Peer: {peer?.PeerDetails?.ConnectionID}");

			//Always return false for logging so we don't consume
			return false;
		}
	}
}
