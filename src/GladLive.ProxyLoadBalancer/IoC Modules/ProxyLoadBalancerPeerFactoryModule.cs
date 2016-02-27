using Autofac;
using Autofac.Core;
using GladNet.Common;
using GladNet.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerPeerFactoryModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Register the peer factory
			builder.RegisterType<ProxyLoadBalancerClientPeerSessionFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			//The below code is complex. It involves: delegates, lambdas, captures and autofac
			//While it's not possible to explain the whole process basically it creates a strongly typed factory delegate
			//that takes in basic session parameters and resolves the other parameters.
			//The reason a context is created inside the lambda is explained here: http://stackoverflow.com/questions/20583339/autofac-and-func-factories

			//We use clever lambda capture to 'steal' the content so we can build the session
			builder.Register(con =>
			{
				IComponentContext capturableContext = con.Resolve<IComponentContext>();

				return new PeerFactory<AuthServicePeerSession>((s, d, ss, dh) => SessionResolve<AuthServicePeerSession>(capturableContext, s, d, ss, dh));
			})
				.AsSelf()
				.SingleInstance();


			//We use clever lambda capture to 'steal' the content so we can build the session
			builder.Register(con =>
			{
				IComponentContext capturableContext = con.Resolve<IComponentContext>();

				return new PeerFactory<UserClientPeerSession>((s, d, ss, dh) => SessionResolve<UserClientPeerSession>(capturableContext, s, d, ss, dh));
			})
				.AsSelf()
				.SingleInstance();
		}

		//Resolves a TSessionType from the context provided
		private TSessionType SessionResolve<TSessionType>(IComponentContext context, INetworkMessageSender sender, IConnectionDetails details, 
			INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
				where TSessionType : ClientPeerSession
		{
			return context.Resolve<TSessionType>(GenerateTypedParameter(sender), GenerateTypedParameter(details), GenerateTypedParameter(subService), GenerateTypedParameter(disconnectHandler));
		}


		private Parameter GenerateTypedParameter<TParameterType>(TParameterType parameter)
		{
			return new TypedParameter(typeof(TParameterType), parameter);
		}
	}
}
