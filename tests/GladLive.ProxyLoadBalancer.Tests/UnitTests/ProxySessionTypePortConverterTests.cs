using GladLive.ProxyLoadBalancer.Settings;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer.Tests
{
	[TestFixture]
	public static class ProxySessionTypePortConverterTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new ProxySessionTypePortConverter());
		}

		[Test]
		public static void Test_Ports_Map_To_Expected_SessionTypes()
		{
			//arrange
			ProxySessionTypePortConverter converter = new ProxySessionTypePortConverter();

			//assert
			Assert.AreEqual(ProxySessionType.AuthServiceSession, converter.ToSessionType(InboundConnectionSettings.Default.AuthServiceSessionPort));
			Assert.AreEqual(ProxySessionType.UserSession, converter.ToSessionType(InboundConnectionSettings.Default.ClientSessionPort));
		}
	}
}
