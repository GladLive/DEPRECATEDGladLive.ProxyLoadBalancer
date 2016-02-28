using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common;
using GladNet.Server.Common;
using GladLive.Server.Common;
using Common.Logging;
using Autofac.Core;
using Autofac;

namespace GladLive.ProxyLoadBalancer
{
	public class ProxyLoadBalancerClientPeerSessionFactory : IPeerFactoryService<ClientPeerSession, ProxySessionType>, IClassLogger
	{
		/// <summary>
		/// Security protection service for incoming connections.
		/// </summary>
		private IConnectionGateKeeper<ProxySessionType> connectionGateKeeper { get; }

		/// <summary>
		/// Conversion service for port to <see cref="ProxySessionType"/>.
		/// </summary>
		private IPortToSessionTypeService<ProxySessionType> portToSessionTypeConverter { get; }

		public ILog Logger { get; }

		/// <summary>
		/// Factory delegate for <see cref="AuthServicePeerSession"/>s.
		/// </summary>
		private PeerFactory<AuthServicePeerSession> authPeerFactory { get; }

		/// <summary>
		/// Factory delegates for <see cref="UserClientPeerSession"/>.
		/// </summary>
		private PeerFactory<UserClientPeerSession> userPeerFactory { get; }

		public ProxyLoadBalancerClientPeerSessionFactory(ILog logger, IConnectionGateKeeper<ProxySessionType> gateKeeper, IPortToSessionTypeService<ProxySessionType> portConverter,
			PeerFactory<AuthServicePeerSession> authFactory, PeerFactory<UserClientPeerSession> userFactory)
		{
			connectionGateKeeper = gateKeeper;
			portToSessionTypeConverter = portConverter;
			Logger = logger;

			authPeerFactory = authFactory;
			userPeerFactory = userFactory;
		}

		public bool CanCreate(IConnectionDetails details)
		{
			//Check the gate if this is a valid port
			if (connectionGateKeeper.isValidPort(details.LocalPort))
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
					Logger.WarnFormat("{0} rejected connection for SessionType: {1} with IP {2}",
						nameof(IConnectionGateKeeper<ProxySessionType>), portToSessionTypeConverter.ToSessionType(details.LocalPort).ToString(), details.RemoteIP.ToString());
			}

			return false;
		}

		public ClientPeerSession Create(INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
		{
			Logger.DebugFormat("Client trying to create session on Port: {0}", details.LocalPort);


			switch (portToSessionTypeConverter.ToSessionType(details.LocalPort))
			{
				case ProxySessionType.UserSession:
					Logger.Debug("Creating client session.");
					return userPeerFactory(sender, details, subService, disconnectHandler);
					//return userPeerFactory(GenerateTypedParameter(sender), GenerateTypedParameter(details), GenerateTypedParameter(subService), GenerateTypedParameter(disconnectHandler));

				case ProxySessionType.AuthServiceSession:
					Logger.Debug("Creating new un-authenticated authservice session.");
					return authPeerFactory(sender, details, subService, disconnectHandler);

				case ProxySessionType.Default:
				default:
					Logger.ErrorFormat("An invalid {0} was generated from Port: {1}", nameof(ProxySessionType), details.LocalPort);
					return null;
			}
		}
	}
}
