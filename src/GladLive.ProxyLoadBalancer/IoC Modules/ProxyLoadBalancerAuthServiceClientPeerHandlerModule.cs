﻿using Autofac;
using GladLive.Common;
using GladNet.Message;
using GladNet.Message.Handlers;
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
				.AssignableTo<IResponseMessageHandler<AuthServiceClientPeer>>()
				.As<IResponseMessageHandler<AuthServiceClientPeer>>();

			//Register the handler service
			builder.Register(con => con.Resolve<IEnumerable<IResponseMessageHandler<AuthServiceClientPeer>>>().ToChainHandler().ToService())
				.As<IResponseMessageHandlerService<AuthServiceClientPeer>>()
				.SingleInstance();
		}
	}
}
