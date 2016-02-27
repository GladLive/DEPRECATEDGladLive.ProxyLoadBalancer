using Autofac;
using Autofac.Core;
using GladNet.Serializer;
using GladNet.Serializer.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Serialization dependency module.
	/// </summary>
	public class ProxyLoadBalancerSerializationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			//Registers the protobuf serializer type for serialization
			builder.RegisterType<ProtobufnetSerializerStrategy>()
				.As<ISerializerStrategy>()
				.InstancePerDependency();

			//Registers the protobuf deserializer type for deserialization
			builder.RegisterType<ProtobufnetDeserializerStrategy>()
				.As<IDeserializerStrategy>()
				.InstancePerDependency();
		}
	}
}
