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
	/// <summary>
	/// Guardian that prevents or allows connections to the proxy server.
	/// </summary>
	public class ProxyLoadBalancerConnectionGateKeeper : IConnectionGateKeeper<ProxySessionType>
	{
		/// <summary>
		/// Indicates if a port is valid to connect on.
		/// </summary>
		/// <param name="port">The port to check.</param>
		/// <returns>True if the port is valid and can connect.</returns>
		public bool isValidPort(int port)
		{
			return InboundConnectionSettings.Default.AuthServiceSessionPort == port || InboundConnectionSettings.Default.ClientSessionPort == port;
		}

		/// <summary>
		/// Determines if the session can pass through the gate.
		/// </summary>
		/// <param name="sessionType">Type of the session.</param>
		/// <param name="details">Details about the connection.</param>
		/// <returns>True if the session can pass through the gate.</returns>
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
