using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Enumeration of Session Types for the Proxy.
	/// </summary>
	public enum ProxySessionType : byte
	{
		Default,
		UserSession,
		AuthServiceSession
	}
}
