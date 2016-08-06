using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Indicates the status of the AuthService.
	/// </summary>
	public enum AuthServiceState : int
	{
		Unavailable = 0,
		Available,
		Busy,
	}
}
