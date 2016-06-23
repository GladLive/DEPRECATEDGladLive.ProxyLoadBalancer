using Autofac;
using Autofac.Builder;
using Autofac.Features.Variance;
using Common.Logging;
using GladLive.Common;
using GladLive.Server.Common;
using GladNet.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	[TestFixture]
	public static class AutoFacTests
	{
		[Test]
		public static void Test_GameSessionHandlerModule_Registers_Contravariant_Handlers()
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterModule<ProxyLoadBalancerGameSessionHandlerModule>();

			builder.RegisterInstance(new Mock<ISigningService>(MockBehavior.Loose).Object)
				.As<ISigningService>()
				.SingleInstance();

			Mock<IElevationVerificationService> elevationManager = new Mock<IElevationVerificationService>();

			//Setup so the elevation manager always indicates true.
			elevationManager.Setup(x => x.isElevated(It.IsAny<IElevatableSession>()))
				.Returns(true);

			builder.RegisterInstance(elevationManager.Object)
				.As<IElevationVerificationService>()
				//.As<IElevationAuthenticationService>()
				.SingleInstance();

			builder.Register(con => Mock.Of<ILog>());
			IContainer container = builder.Build();

			Assert.Contains(typeof(PayloadDebugLoggingHandler), (ICollection)container.Resolve<IEnumerable<IRequestPayloadHandler<GameServicePeerSession>>>().Select(x => x.GetType()).ToArray());
			//Assert.IsTrue(container.Resolve<IRequestPayloadHandlerService<GameServicePeerSession>>().TryProcessPayload(Mock.Of<PacketPayload>(), Mock.Of<IMessageParameters>(), new GameServicePeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
			//	Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), container.Resolve<IRequestPayloadHandlerService<GameServicePeerSession>>())));
		}

		[Test]
		public static void Test_ClientSessionHandlerModule_Registers_Contravariant_Handlers()
		{
			//arrange
			ContainerBuilder builder = new ContainerBuilder();

			//act
			builder.RegisterModule<ProxyLoadBalancerClientSessionHandlerModule>();
			builder.Register(con => Mock.Of<ILog>());
			IContainer container = builder.Build();

			//assert
			Assert.Contains(typeof(PayloadDebugLoggingHandler), (ICollection)container.Resolve<IEnumerable<IRequestPayloadHandler<UserClientPeerSession>>>().Select(x => x.GetType()).ToArray());

			//Assert.IsTrue(container.Resolve<IRequestPayloadHandlerService<UserClientPeerSession>>().TryProcessPayload(Mock.Of<PacketPayload>(), Mock.Of<IMessageParameters>(), new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(), 
			//	Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), container.Resolve<IRequestPayloadHandlerService<UserClientPeerSession>>())));
		}
	}
}
