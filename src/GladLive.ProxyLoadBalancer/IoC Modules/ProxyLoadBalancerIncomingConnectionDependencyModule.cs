using Autofac;
using GladLive.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Incoming connection management and controlling registeration module.
	/// </summary>
	public class ProxyLoadBalancerIncomingConnectionDependencyModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Register ProxySession services like port convert and connection gatekeeper
			builder.RegisterType<ProxySessionTypePortConverter>()
				.As<IPortToSessionTypeService<ProxySessionType>>()
				.SingleInstance();

			builder.RegisterType<ProxyLoadBalancerConnectionGateKeeper>()
				.As<IConnectionGateKeeper<ProxySessionType>>()
				.SingleInstance();
		}
	}
}
