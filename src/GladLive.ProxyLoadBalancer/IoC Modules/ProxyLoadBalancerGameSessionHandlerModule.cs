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
	public class ProxyLoadBalancerGameSessionHandlerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Register the elevated request handlers
			builder.RegisterSource(new ContravariantRegistrationSource());

			//This registers all elevated request handlers with the above method call
			//allowing for resolving contravariant collections of them
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IElevatedRequestPayloadHandler<GameServicePeerSession>>()
				.As<IElevatedRequestPayloadHandler<GameServicePeerSession>>();

			/*builder.RegisterAssemblyTypes(typeof(AuthenticationRequestHandler<GameServicePeerSession>).Assembly)
				.AssignableTo<IElevatedRequestPayloadHandler<GameServicePeerSession>>()
				.As<IElevatedRequestPayloadHandler<GameServicePeerSession>>();*/

			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IRequestPayloadHandler<GameServicePeerSession>>()
				.As<IRequestPayloadHandler<GameServicePeerSession>>();

			builder.Register(con => new RequestPayloadHandlerService<GameServicePeerSession>(new MultipleChainResponsbilityPayloadHandlerStrategy<GameServicePeerSession>(new ChainPayloadHandler<GameServicePeerSession>(con.Resolve<IEnumerable<IRequestPayloadHandler<GameServicePeerSession>>>()),
				new ElevatedSessionChainPayloadHandlerStrategyDecorator<GameServicePeerSession>(con.Resolve<ILog>(), con.Resolve<IElevationVerificationService>(), new ChainPayloadHandler<GameServicePeerSession>(con.Resolve<IEnumerable<IElevatedEventPayloadHandler<GameServicePeerSession>>>())))))
				.As<IRequestPayloadHandlerService<GameServicePeerSession>>()
				.SingleInstance();
		}
	}
}
