using GladLive.ProxyLoadBalancer.Settings;
using GladNet.Common;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer.Tests
{
	[TestFixture]
	public static class ProxyLoadBalancerConnectionGateKeeperTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new ProxyLoadBalancerConnectionGateKeeper());
		}

		[Test]
		public static void Test_Accepts_Expected_Ports()
		{
			//can't do test cases because it's not const

			//arrange
			ProxyLoadBalancerConnectionGateKeeper keeper = new ProxyLoadBalancerConnectionGateKeeper();

			//assert
			Assert.True(keeper.isValidPort(InboundConnectionSettings.Default.AuthServiceSessionPort));
			Assert.True(keeper.isValidPort(InboundConnectionSettings.Default.ClientSessionPort));
		}

		[Test]
		[TestCase(-1)]
		[TestCase(80)]
		[TestCase(0)]
		[TestCase(int.MinValue)]
		[TestCase(int.MaxValue)]
		public static void Test_Rejects_Unexpected_Ports(int port)
		{
			//can't do test cases because it's not const

			//arrange
			ProxyLoadBalancerConnectionGateKeeper keeper = new ProxyLoadBalancerConnectionGateKeeper();

			//assert
			Assert.IsFalse(keeper.isValidPort(port), "Found an unexpected accepted port {0}", port);
		}

		[Test]
		public static void Test_Gatekeeper_Lets_Client_Connect_On_Client_Port()
		{
			//arrange
			ProxyLoadBalancerConnectionGateKeeper keeper = new ProxyLoadBalancerConnectionGateKeeper();

			Mock<IConnectionDetails> details = new Mock<IConnectionDetails>();
			details.SetupGet(x => x.LocalPort).Returns(InboundConnectionSettings.Default.ClientSessionPort);

			//act
			bool result = keeper.RequestPassage(ProxySessionType.UserSession, details.Object);

			//assert
			Assert.IsTrue(result);
		}

		[Test]
		public static void Test_Gatekeeper_Lets_AuthService_Connect_On_Auth_Port()
		{
			//arrange
			ProxyLoadBalancerConnectionGateKeeper keeper = new ProxyLoadBalancerConnectionGateKeeper();

			Mock<IConnectionDetails> details = new Mock<IConnectionDetails>();
			details.SetupGet(x => x.LocalPort).Returns(InboundConnectionSettings.Default.AuthServiceSessionPort);

			//act
			bool result = keeper.RequestPassage(ProxySessionType.AuthServiceSession, details.Object);

			//assert
			Assert.IsTrue(result);
		}
	}
}
