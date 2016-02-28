using Autofac;
using Autofac.Features.Variance;
using Common.Logging;
using GladLive.Common;
using GladLive.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerAuthSessionHandlerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Register the elevated request handlers
			builder.RegisterSource(new ContravariantRegistrationSource());

			//This registers all elevated request handlers with the above method call
			//allowing for resolving contravariant collections of them
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IElevatedRequestPayloadHandler<AuthServicePeerSession>>()
				.As<IElevatedRequestPayloadHandler<AuthServicePeerSession>>();

			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IRequestPayloadHandler<AuthServicePeerSession>>()
				.As<IRequestPayloadHandler<AuthServicePeerSession>>();

			builder.Register(con => new RequestPayloadHandlerService<AuthServicePeerSession>(new MultipleChainResponsbilityPayloadHandlerStrategy<AuthServicePeerSession>(new ChainPayloadHandler<AuthServicePeerSession>(con.Resolve<IEnumerable<IRequestPayloadHandler<AuthServicePeerSession>>>()),
				new ElevatedSessionChainPayloadHandlerStrategyDecorator<AuthServicePeerSession>(con.Resolve<ILog>(), con.Resolve<IElevationVerificationService>(), new ChainPayloadHandler<AuthServicePeerSession>(con.Resolve<IEnumerable<IElevatedEventPayloadHandler<AuthServicePeerSession>>>())))))
				.As<IRequestPayloadHandlerService<AuthServicePeerSession>>()
				.SingleInstance();
		}
	}
}
