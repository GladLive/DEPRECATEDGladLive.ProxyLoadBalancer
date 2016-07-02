using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Registration module for session types.
	/// </summary>
	public class ProxyLoadBalancerSessionModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Register the session types
			builder.RegisterType<UserClientPeerSession>()
				.AsSelf()
				.InstancePerDependency();

			builder.RegisterType<GameServicePeerSession>()
				.AsSelf()
				.InstancePerDependency();
		}
	}
}
