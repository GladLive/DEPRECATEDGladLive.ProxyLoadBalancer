using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerClientSessionHandlerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//builder.RegisterAssemblyTypes(this.ThisAssembly)
			//	.AsClosedTypesOf(typeof(IElevatedRequestPayloadHandler<>))
		}
	}
}
