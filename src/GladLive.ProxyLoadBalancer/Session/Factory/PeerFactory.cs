using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Defined simplified delegate type for a factory delegate that produces <typeparamref name="TPeerType"/> peers.
	/// </summary>
	/// <typeparam name="TPeerType">The peer type to create.</typeparam>
	/// <param name="sender">Network message sending service.</param>
	/// <param name="details">Connection details that define the peer connection.</param>
	/// <param name="subService">Message subscription service.</param>
	/// <param name="disconnectHandler">Disconnection handler and subscription service.</param>
	/// <returns>A new non-null instance of <typeparamref name="TPeerType"/> or throws.</returns>
	public delegate TPeerType PeerFactory<TPeerType>(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		where TPeerType : INetPeer;
}
