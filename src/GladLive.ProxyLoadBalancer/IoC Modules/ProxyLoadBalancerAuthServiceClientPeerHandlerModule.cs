using Autofac;
using GladLive.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerAuthServiceClientPeerHandlerModule : Module
	{
		//TODO: Extract this out using generics and provide it as a service throughout GladLive
		protected override void Load(ContainerBuilder builder)
		{
			//Registers the User specific, and general Peer, request handlers.
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.AssignableTo<IResponsePayloadHandler<AuthServiceClientPeer>>()
				.As<IResponsePayloadHandler<AuthServiceClientPeer>>();

			//Register the handler service
			builder.Register(con => new RequestPayloadHandlerService<AuthServiceClientPeer>(new ChainPayloadHandler<AuthServiceClientPeer>(con.Resolve<IEnumerable<IResponsePayloadHandler<AuthServiceClientPeer>>>())))
				.As<IResponsePayloadHandlerService<AuthServiceClientPeer>>()
				.SingleInstance();
		}
	}
}
