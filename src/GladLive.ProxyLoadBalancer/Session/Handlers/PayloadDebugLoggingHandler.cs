using Common.Logging;
using GladLive.Common;
using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class PayloadDebugLoggingHandler : IRequestPayloadHandler<INetPeer>//, IEventPayloadHandler<INetPeer>, IResponsePayloadHandler<INetPeer>, IClassLogger
	{
		public ILog Logger { get; }

		public PayloadDebugLoggingHandler(ILog logger)
		{
			Logger = logger;
		}

		public bool TryProcessPayload(PacketPayload payload, IMessageParameters parameters, INetPeer peer)
		{
			Logger.DebugFormat("Recieved Payload Type: {0} from Peer: {1}", payload?.GetType(), peer.PeerDetails.ConnectionID);

			//Always return false for logging so we don't consume
			return false;
		}
	}
}
