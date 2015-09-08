using System.Security.Principal;

namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("userProxy")]
    public class UserProxyPrincipal : GenericDirectoryObject, IComparable
    {
        #region Constructors

        public UserProxyPrincipal(PrincipalContext context)
            : base(context)
        {
        }

        public UserProxyPrincipal(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        #endregion

        #region ExtendedLdapFunctions

        //work around way to create a new userProxyFull object
        public static UserProxyFullPrincipal CreateProxy(PrincipalContext context, string name, SecurityIdentifier sid)
        {
            var sidInBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidInBytes, 0);
            var ouDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", context.ConnectedServer, context.Container));
            var proxyDe = ouDe.Children.Add(String.Format("CN={0}", name), "userProxy");
            proxyDe.Properties["objectSid"].Clear();
            proxyDe.Properties["objectSid"].Value = sidInBytes;
            proxyDe.Properties["userPrincipalName"].Value = name;
            proxyDe.CommitChanges();
            return UserProxyFullPrincipal.FindByIdentity(context, name);
        }

        //work around way to create a new userProxyFull object
        public static void CreateProxy(string server, string path, string name, SecurityIdentifier sid)
        {
            var sidInBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidInBytes, 0);
            var ouDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", server, path));
            var proxyDe = ouDe.Children.Add(String.Format("CN={0}", name), "userProxy");
            proxyDe.Properties["objectSid"].Clear();
            proxyDe.Properties["objectSid"].Value = sidInBytes;
            proxyDe.Properties["userPrincipalName"].Value = name;
            proxyDe.CommitChanges();
        }

        #endregion

        #region Overrides

        public void SetPassword(string newPassword)
        {
            throw new NotSupportedException("SetPassword() not supported on UserProxyFullPrincipal objects.");
        }

        // Implement the overloaded search method FindByIdentity.
        public new static UserProxyPrincipal FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (UserProxyPrincipal) FindByIdentityWithType(context,
                typeof (ComputerPrincipalFull),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static UserProxyPrincipal FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (UserProxyPrincipal) FindByIdentityWithType(context,
                typeof (ComputerPrincipalFull),
                identityType,
                identityValue);
        }

        public class UserProxyPrincipalSearchFilter : AdvancedFilters
        {
            public UserProxyPrincipalSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var principal = obj as UserProxyPrincipal;
            if (principal != null)
            {
                var u = principal;
                return Sid.CompareTo(u.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var principal = obj as UserProxyPrincipal;
            if (principal != null)
            {
                var u = principal;
                return (u.Sid == Sid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}