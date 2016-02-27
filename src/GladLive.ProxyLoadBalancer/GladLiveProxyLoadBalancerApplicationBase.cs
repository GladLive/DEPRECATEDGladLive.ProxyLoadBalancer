using GladNet.PhotonServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet.Common;
using GladNet.Serializer;
using GladNet.Server.Common;
using Autofac;
using System.IO;
using GladLive.ProxyLoadBalancer.Settings;
using GladLive.Server.Common;
using Autofac.Core;

namespace GladLive.ProxyLoadBalancer
{
	public class GladLiveProxyLoadBalancerApplicationBase : GladNetAppBase
	{
		/// <summary>
		/// Deserialization strategy for incoming messages.
		/// </summary>
		public override IDeserializerStrategy Deserializer { get { return appBaseContainer.Resolve<IDeserializerStrategy>(); } protected set { } }

		/// <summary>
		/// Serialization strategy for outgoing messages.
		/// </summary>
		public override ISerializerStrategy Serializer { get { return appBaseContainer.Resolve<ISerializerStrategy>(); } protected set { } }

		/// <summary>
		/// Application logging service.
		/// </summary>
		protected override ILog AppLogger { get; set; }

		/// <summary>
		/// IoC container type resolver.
		/// </summary>
		private IContainer appBaseContainer;

		/// <summary>
		/// Security protection service for incoming connections.
		/// </summary>
		private IConnectionGateKeeper<ProxySessionType> connectionGateKeeper;

		/// <summary>
		/// Conversion service for port to <see cref="ProxySessionType"/>.
		/// </summary>
		private IPortToSessionTypeService<ProxySessionType> portToSessionTypeConverter;

		public override ServerPeer CreateServerPeer(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			//This shouldn't be called by the ProxyLoadBalancing server
			AppLogger.ErrorFormat("Outgoing connection attempt on Proxy to IP {0} Port {1}. Proxy should not be connecting to other peers", details.RemoteIP, details.RemotePort);

			return null;
		}

		public override ServerPeerSession CreateServerPeerSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			throw new NotImplementedException("This is deprecated and will be removed. Somehow a someone tried to create a peer session.");
		}

		protected override ClientPeerSession CreateClientSession(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			AppLogger.DebugFormat("Client trying to create session on Port: {0}", details.LocalPort);


			switch (portToSessionTypeConverter.ToSessionType(details.LocalPort))
			{
				case ProxySessionType.UserSession:
					AppLogger.Debug("Creating client session.");
					appBaseContainer.Resolve<UserClientPeerSession>(GenerateTypedParameter(sender), GenerateTypedParameter(details), GenerateTypedParameter(subService), GenerateTypedParameter(disconnectHandler));
					return new UserClientPeerSession(AppLogger, sender, details, subService, disconnectHandler);

				case ProxySessionType.AuthServiceSession:
					AppLogger.Debug("Creating new un-authenticated authservice session.");
					return appBaseContainer.Resolve<AuthServicePeerSession>(GenerateTypedParameter(sender), GenerateTypedParameter(details), GenerateTypedParameter(subService), GenerateTypedParameter(disconnectHandler));

				case ProxySessionType.Default:
				default:
					AppLogger.ErrorFormat("An invalid {0} was generated from Port: {1}", nameof(ProxySessionType), details.LocalPort);
					return null;
			}
		}

		private Parameter GenerateTypedParameter<TParameterType>(TParameterType parameter)
		{
			return new TypedParameter(typeof(TParameterType), parameter);
		}

		protected override void Setup()
		{
			//Setup logging first
			AppLogger = new PhotonServerLog4NetCommonLoggingILogAdapter(this.ApplicationRootPath, this.ApplicationName, this.BinaryPath);

			//We should setup AutoFac IoC with the dependencies it'll need to be resolving.
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterModule<ProxyLoadBalancerSerializationModule>();
			builder.RegisterModule<ProxyLoadBalancerIncomingConnectionDependencyModule>();
			builder.RegisterModule<ProxyLoadBalancerSessionModule>();

			//Register the app-wide logging instance
			builder.RegisterInstance(AppLogger)
				.As<ILog>()
				.SingleInstance();

			//Builds out the IoC container
			appBaseContainer = builder.Build();

			AppLogger.DebugFormat("GLADLIVE: {0} {1} complete.", nameof(GladLiveProxyLoadBalancerApplicationBase), nameof(Setup));

			InitServices();
		}

		public void InitServices()
		{
			this.portToSessionTypeConverter = appBaseContainer.Resolve<IPortToSessionTypeService<ProxySessionType>>();
			this.connectionGateKeeper = appBaseContainer.Resolve<IConnectionGateKeeper<ProxySessionType>>();

			AppLogger.DebugFormat("GLADLIVE: {0} {1} complete.", nameof(GladLiveProxyLoadBalancerApplicationBase), nameof(InitServices));
		}

		protected override bool ShouldServiceIncomingPeerConnect(IConnectionDetails details)
		{
			//Check the gate if this is a valid port
			if(connectionGateKeeper.isValidPort(details.LocalPort))
			{
				//If it's a valid port more needs to be done to protect the server
				//AuthServices may have only a subset of valid remote IPs or something
				//Because of this the gate keeper is delegated with the task of determining
				//if we can connect. Could also prevent application-level DDOS by rejecting clients
				//that are constantly connectiong
				if (connectionGateKeeper.RequestPassage(portToSessionTypeConverter.ToSessionType(details.LocalPort), details))
				{
					return true;
				}
				else
					//This means we had a potential malicious or miscongifured connection attempt
					AppLogger.WarnFormat("{0} rejected connection for SessionType: {1} with IP {2}", 
						nameof(IConnectionGateKeeper<ProxySessionType>), portToSessionTypeConverter.ToSessionType(details.LocalPort).ToString(), details.RemoteIP.ToString());
			}

			return false;
		}

		protected override void TearDown()
		{
			AppLogger.InfoFormat("{0} is tearing down server instance.", nameof(GladLiveProxyLoadBalancerApplicationBase));
		}
	}
}
