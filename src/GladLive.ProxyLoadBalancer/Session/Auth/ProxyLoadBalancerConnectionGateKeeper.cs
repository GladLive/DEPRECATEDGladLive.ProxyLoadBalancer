using GladLive.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common;
using GladLive.ProxyLoadBalancer.Settings;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerConnectionGateKeeper : IConnectionGateKeeper<ProxySessionType>
	{
		public bool isValidPort(int port)
		{
			return InboundConnectionSettings.Default.AuthServiceSessionPort == port || InboundConnectionSettings.Default.ClientSessionPort == port;
		}

		public bool RequestPassage(ProxySessionType sessionType, IConnectionDetails details)
		{
			//For now we just let everything pass through. 
			//Eventually we will block incoming connections for certain session types that don't match
			//the subset of valid IPs for the service or if it doesn't match the correct port

			//Don't let default sessions pass in.
			if (sessionType == ProxySessionType.Default)
				return false;

			return isValidPort(details.LocalPort);
		}
	}
}
