using Common.Logging;
using GladLive.Common;
using GladNet.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;
using GladNet.Message;
using GladNet.Engine.Common;
using GladNet.Message.Handlers;
using GladNet.Payload;

namespace GladLive.ProxyLoadBalancer.Tests
{
	[TestFixture]
	public static class UserClientPeerSessionTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Parameters()
		{
			//These sorts of tests might seem stupid but it just caught two faults
			//I checked the wrong object for null.
			//assert
			Assert.DoesNotThrow(() => new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), Mock.Of<IRequestMessageHandlerService<UserClientPeerSession>>()));
		}

		[Test]
		public static void Test_Ctor_Throws_On_Null_Handler_Service()
		{
			//These sorts of tests might seem stupid but it just caught two faults
			//I checked the wrong object for null.
			//assert
			Assert.Throws<ArgumentNullException>(() => new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), null));
		}

		[Test]
		public static void Test_Session_Doesnt_Process_Unencrypted_Messages()
		{
			//arrange
			Mock<IRequestMessageHandlerService<UserClientPeerSession>> handler = new Mock<IRequestMessageHandlerService<UserClientPeerSession>>(MockBehavior.Loose);
			UserClientPeerSession session = new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), handler.Object);
			Mock<IMessageParameters> messageParameters = new Mock<IMessageParameters>();
			messageParameters.SetupGet(mp => mp.Encrypted).Returns(false);

			//act
			//Generate and bind a method to a delegate
			Action<IRequestMessage, IMessageParameters> method = GrabProtectedOnRecieveRequestMethod(session);
			method.Invoke(Mock.Of<IRequestMessage>(), messageParameters.Object);

			//assert
			//Make sure the message didn't make it to the handler
			handler.Verify(x => x.TryProcessMessage(It.IsAny<RequestMessage>(), It.IsAny<IMessageParameters>(), session), Times.Never());
		}

		[Test]
		public static void Test_Session_Does_Process_Encrypted_Messages()
		{
			//arrange
			Mock<IRequestMessageHandlerService<UserClientPeerSession>> handler = new Mock<IRequestMessageHandlerService<UserClientPeerSession>>(MockBehavior.Loose);
			UserClientPeerSession session = new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), handler.Object);
			Mock<IMessageParameters> messageParameters = new Mock<IMessageParameters>();
			messageParameters.SetupGet(mp => mp.Encrypted).Returns(true);

			//act
			//Generate and bind a method to a delegate
			Action<IRequestMessage, IMessageParameters> method = GrabProtectedOnRecieveRequestMethod(session);
			method.Invoke(Mock.Of<IRequestMessage>(), messageParameters.Object);

			//assert
			//Make sure the message didn't make it to the handler
			handler.Verify(x => x.TryProcessMessage(It.IsAny<IRequestMessage>(), It.IsAny<IMessageParameters>(), session), Times.Once());
		}

		[Test]
		public static void Test_Session_Throws_Null_Arg_OnRequestRecieve_When_Payload_Is_Null()
		{
			//arrange
			UserClientPeerSession session = new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), Mock.Of<IRequestMessageHandlerService<UserClientPeerSession>>());

			//assert
			Assert.Throws<ArgumentNullException>(() => GrabProtectedOnRecieveRequestMethod(session).Invoke(null, Mock.Of<IMessageParameters>()));
		}

		[Test]
		public static void Test_Session_Throws_Null_Arg_OnRequestRecieve_When_MessageParameters_Is_Null()
		{
			//arrange
			UserClientPeerSession session = new UserClientPeerSession(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>(), Mock.Of<IRequestMessageHandlerService<UserClientPeerSession>>());

			//assert
			Assert.Throws<ArgumentNullException>(() => GrabProtectedOnRecieveRequestMethod(session).Invoke(Mock.Of<IRequestMessage>(), null));
		}

		public static Action<IRequestMessage, IMessageParameters> GrabProtectedOnRecieveRequestMethod(UserClientPeerSession session)
		{
			return Delegate.CreateDelegate(typeof(Action<IRequestMessage, IMessageParameters>), session, session.GetType().GetMethod("OnReceiveRequest", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(IRequestMessage), typeof(IMessageParameters) }, null)) as Action<IRequestMessage, IMessageParameters>;
		}
	}
}
