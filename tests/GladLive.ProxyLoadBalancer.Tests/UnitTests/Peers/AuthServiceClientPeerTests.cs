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

namespace GladLive.ProxyLoadBalancer.Tests
{
	[TestFixture]
	public static class AuthServiceClientPeerTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Parameters()
		{
			//These sorts of tests might seem stupid but it just caught two faults
			//I checked the wrong object for null.
			//assert
			Assert.DoesNotThrow(() => new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<IResponsePayloadHandlerService<AuthServiceClientPeer>>()));
		}

		[Test]
		public static void Test_Ctor_Throws_On_Null_Handler_Service()
		{
			//These sorts of tests might seem stupid but it just caught two faults
			//I checked the wrong object for null.
			//assert
			Assert.Throws<ArgumentNullException>(() => new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), null));
		}

		//TODO: When message parameters are implemented in the GladNet.ASP.Client implementation we should start checking if the messages were encrypted
		/*[Test]
		public static void Test_Session_Doesnt_Process_Unencrypted_Messages()
		{
			//arrange
			Mock<IResponsePayloadHandlerService<AuthServiceClientPeer>> handler = new Mock<IResponsePayloadHandlerService<AuthServiceClientPeer>>(MockBehavior.Loose);
			AuthServiceClientPeer session = new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), handler.Object);
			Mock<IMessageParameters> messageParameters = new Mock<IMessageParameters>();
			messageParameters.SetupGet(mp => mp.Encrypted).Returns(false);

			//act
			//Generate and bind a method to a delegate
			Action<PacketPayload, IMessageParameters> method = GrabProtectedOnRecieveRequestMethod(session);
			method.Invoke(Mock.Of<PacketPayload>(), messageParameters.Object);

			//assert
			//Make sure the message didn't make it to the handler
			handler.Verify(x => x.TryProcessPayload(It.IsAny<PacketPayload>(), It.IsAny<IMessageParameters>(), session), Times.Never());
		}*/

		[Test]
		public static void Test_Session_Does_Process_Encrypted_Messages()
		{
			//arrange
			Mock<IResponsePayloadHandlerService<AuthServiceClientPeer>> handler = new Mock<IResponsePayloadHandlerService<AuthServiceClientPeer>>(MockBehavior.Loose);
			AuthServiceClientPeer session = new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), handler.Object);
			Mock<IMessageParameters> messageParameters = new Mock<IMessageParameters>();
			messageParameters.SetupGet(mp => mp.Encrypted).Returns(true);

			//act
			//Generate and bind a method to a delegate
			Action<PacketPayload, IMessageParameters> method = GrabProtectedOnRecieveRequestMethod(session);
			method.Invoke(Mock.Of<PacketPayload>(), messageParameters.Object);

			//assert
			//Make sure the message didn't make it to the handler
			handler.Verify(x => x.TryProcessPayload(It.IsAny<PacketPayload>(), It.IsAny<IMessageParameters>(), session), Times.Once());
		}

		[Test]
		public static void Test_Session_Throws_Null_Arg_OnRequestRecieve_When_Payload_Is_Null()
		{
			//arrange
			AuthServiceClientPeer session = new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<IResponsePayloadHandlerService<AuthServiceClientPeer>>());

			//assert
			Assert.Throws<ArgumentNullException>(() => GrabProtectedOnRecieveRequestMethod(session).Invoke(null, Mock.Of<IMessageParameters>()));
		}

		//TODO: When message parameters are implemented in the GladNet.ASP.Client implementation we should start null checking them.
		/*[Test]
		public static void Test_Session_Throws_Null_Arg_OnRequestRecieve_When_MessageParameters_Is_Null()
		{
			//arrange
			AuthServiceClientPeer session = new AuthServiceClientPeer(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(),
				new Mock<IConnectionDetails>(MockBehavior.Loose).Object, Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<IResponsePayloadHandlerService<AuthServiceClientPeer>>());

			//assert
			Assert.Throws<ArgumentNullException>(() => GrabProtectedOnRecieveRequestMethod(session).Invoke(Mock.Of<PacketPayload>(), null));
		}*/

		public static Action<PacketPayload, IMessageParameters> GrabProtectedOnRecieveRequestMethod(AuthServiceClientPeer session)
		{
			return Delegate.CreateDelegate(typeof(Action<PacketPayload, IMessageParameters>), session, session.GetType().GetMethod("OnReceiveResponse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(PacketPayload), typeof(IMessageParameters) }, null)) as Action<PacketPayload, IMessageParameters>;
		}
	}
}
