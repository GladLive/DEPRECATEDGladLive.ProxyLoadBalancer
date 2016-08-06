using Autofac;
using Autofac.Features.Variance;
using Common.Logging;
using GladLive.Common;
using GladLive.Server.Common;
using GladNet.Message;
using GladNet.Message.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerGameSessionHandlerModule : Module
	{
		//TODO: Extract this out using generics and provide it as a service throughout GladLive
		protected override void Load(ContainerBuilder builder)
		{
			//Register the elevated request handlers
			builder.RegisterSource(new ContravariantRegistrationSource());

			//This registers all elevated request handlers with the above method call
			//allowing for resolving contravariant collections of them
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IRequestMessageHandler<GameServicePeerSession>>()
				.Where(t => t.GetCustomAttribute<ElevatedSessionHandlingRequiredAttribute>(true) != null) //finds only elevated
				.Named<IRequestMessageHandler<GameServicePeerSession>>(nameof(ElevatedSessionHandlingRequiredAttribute)); //register under name

			/*builder.RegisterAssemblyTypes(typeof(AuthenticationRequestHandler<GameServicePeerSession>).Assembly)
				.AssignableTo<IElevatedRequestPayloadHandler<GameServicePeerSession>>()
				.As<IElevatedRequestPayloadHandler<GameServicePeerSession>>();*/

			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IRequestMessageHandler<GameServicePeerSession>>()
				.Where(t => t.GetCustomAttribute<ElevatedSessionHandlingRequiredAttribute>(true) == null)
				.Named<IRequestMessageHandler<GameServicePeerSession>>("default");


			builder.Register(con => new MultipleChainResponsbilityMessageHandlerStrategy<GameServicePeerSession, IRequestMessage>(con.ResolveNamed<IEnumerable<IRequestMessageHandler<GameServicePeerSession>>>("default").ToChainHandler(),
				new ElevatedSessionMessageHandlerStrategyDecorator<GameServicePeerSession, IRequestMessage>(con.Resolve<ILog>(), con.Resolve<IElevationVerificationService>(), con.ResolveNamed<IEnumerable<IRequestMessageHandler<GameServicePeerSession>>>(nameof(ElevatedSessionHandlingRequiredAttribute)).ToChainHandler())).ToService())
				.As<IRequestMessageHandlerService<GameServicePeerSession>>()
				.SingleInstance();
		}
	}
}
