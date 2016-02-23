using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public enum ProxySessionType : byte
	{
		Default,
		UserSession,
		AuthServiceSession
	}
}
