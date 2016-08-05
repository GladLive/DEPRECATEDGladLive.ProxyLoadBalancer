using GladNet.Common;
using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	//We created this delegate definition for two reasons. One, simplier definition to work with inside of the factory
	//Two, we needed a factory delegate to point to, in a clean way, for AutoFac factory delegate registration
	/// <summary>
	/// Defined simplified delegate type for a factory delegate that produces <typeparamref name="TPeerType"/> peers.
	/// </summary>
	/// <typeparam name="TPeerType">The peer type to create.</typeparam>
	/// <param name="sender">Network message sending service.</param>
	/// <param name="details">Connection details that define the peer connection.</param>
	/// <param name="subService">Message subscription service.</param>
	/// <param name="disconnectHandler">Disconnection handler and subscription service.</param>
	/// <returns>A new non-null instance of <typeparamref name="TPeerType"/> or throws.</returns>
	public delegate TPeerType PeerFactory<TPeerType>(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routeBack)
		where TPeerType : INetPeer;
}
