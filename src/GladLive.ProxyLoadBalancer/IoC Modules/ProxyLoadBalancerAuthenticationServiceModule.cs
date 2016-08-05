using Autofac;
using GladLive.Security.Common;
using GladLive.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerAuthenticationServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterInstance(new RSAX509SigningService("TestCert.pfx"))
				.As<ISigningService>()
				.SingleInstance();

			builder.RegisterType<ElevationManager>()
				.As<IElevationVerificationService>()
				.As<IElevationAuthenticationService>()
				.SingleInstance();
		}
	}
}
