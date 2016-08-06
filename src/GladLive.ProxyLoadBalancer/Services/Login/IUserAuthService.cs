using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.ProxyLoadBalancer
{
	/// <summary>
	/// Login/Authentication service contract.
	/// </summary>
	public interface IUserAuthService
	{
		/// <summary>
		/// Attempts to handle an authentication request for a user login.
		/// </summary>
		/// <param name="ipOfUser">The <see cref="IPAddress"/> of the requesting user.</param>
		/// <param name="userLoginString">User login string (Ex. Login name, email, accountname or etc)</param>
		/// <param name="userPassword">Encrypted password of the user.</param>
		/// <returns>This indicates only whether the Authservice is available. Not the success of the response.</returns>
		void TryAuthenticateUser(IPAddress ipOfUser, string userLoginString, byte[] userPassword);


		/// <summary>
		/// Checks/verifies the current state of the <see cref="IUserAuthService"/>.
		/// </summary>
		/// <returns>This indicates only whether the Authservice is available.</returns>
		AuthServiceState CheckServiceState();
	}
}
