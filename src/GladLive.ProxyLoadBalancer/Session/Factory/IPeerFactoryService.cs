using GladNet.Common;
using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public interface IPeerFactoryService<TPeerType, TSessionEnumType>
		where TPeerType : INetPeer where TSessionEnumType : struct, IConvertible
	{
		TPeerType Create(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routeBack);

		bool CanCreate(IConnectionDetails details);
	}
}
