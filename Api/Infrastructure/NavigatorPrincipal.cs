using System.Security.Principal;

namespace Api.Infrastructure
{
    public class NavigatorPrincipal: GenericPrincipal
    {
        private readonly int _navigatorId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identity">The IIdentity</param>
        /// <param name="roles">The roles</param>
        /// <param name="navigatorId">The navigator api unique ID</param>
        public NavigatorPrincipal(IIdentity identity, string[] roles, int navigatorId)
            : base(identity, roles)
        {
            _navigatorId = navigatorId;
        }

        /// <summary>
        /// Returns the unique navigator api ID.
        /// </summary>
        public int NavigatorId
        {
            get { return _navigatorId; }
        }
    }
}