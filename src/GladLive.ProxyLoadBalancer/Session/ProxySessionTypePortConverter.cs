using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladLive.Server.Common;
using GladLive.ProxyLoadBalancer.Settings;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Maps <see cref="int"/> port values to <see cref="ProxySessionType"/>s.
	/// </summary>
	public class ProxySessionTypePortConverter : IPortToSessionTypeService<ProxySessionType>
	{
		/// <summary>
		/// Maps <see cref="int"/> port values to <see cref="ProxySessionType"/>s.
		/// </summary>
		/// <param name="port">Port value.</param>
		/// <returns>Corresponding <see cref="ProxySessionType"/></returns>
		public ProxySessionType ToSessionType(int port)
		{
			//can't do switch because non-const source values
			if (port == InboundConnectionSettings.Default.GameServiceSessionPort)
				return ProxySessionType.GameServiceSession;
			else if (port == InboundConnectionSettings.Default.ClientSessionPort)
				return ProxySessionType.UserSession;

			return ProxySessionType.Default;
		}
	}
}
