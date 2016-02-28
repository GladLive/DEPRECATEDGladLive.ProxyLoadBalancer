using Autofac;
using Autofac.Builder;
using Autofac.Features.Variance;
using Common.Logging;
using GladLive.Common;
using GladLive.Server.Common;
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
		public static void Test_AuthSessionHandlerModule_Registers_Contravariant_Handlers()
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterModule<ProxyLoadBalancerAuthSessionHandlerModule>();
			builder.Register(con => Mock.Of<ILog>());

			Assert.Contains(typeof(PayloadDebugLoggingHandler), (ICollection)builder.Build().Resolve<IEnumerable<IRequestPayloadHandler<AuthServicePeerSession>>>().Select(x => x.GetType()).ToArray());
		}
	}
}
