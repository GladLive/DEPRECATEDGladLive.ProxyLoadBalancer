using Autofac;
using GladLive.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerClientSessionHandlerModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Registers the User specific, and general Peer, request handlers.
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IRequestPayloadHandler<UserClientPeerSession>>()
				.As<IRequestPayloadHandler<UserClientPeerSession>>();

			//Register the handler service
			builder.Register(con => new RequestPayloadHandlerService<UserClientPeerSession>(new ChainPayloadHandler<UserClientPeerSession>(con.Resolve<IEnumerable<IRequestPayloadHandler<UserClientPeerSession>>>())))
				.As<IRequestPayloadHandlerService<UserClientPeerSession>>()
				.SingleInstance();
		}
	}
}
