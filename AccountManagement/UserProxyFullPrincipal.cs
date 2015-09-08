using System.Security.Principal;

namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("userProxyFull")]
    public class UserProxyFullPrincipal : UserProxyPrincipal
    {
        #region Constructors

        public UserProxyFullPrincipal(PrincipalContext context)
            : base(context)
        {
        }

        public UserProxyFullPrincipal(PrincipalContext context, string samAccountName, string password, bool enabled)
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
            var ouDE = new DirectoryEntry(string.Format("LDAP://{0}/{1}", context.ConnectedServer, context.Container));
            var proxyDE = ouDE.Children.Add(String.Format("CN={0}", name), "userProxyFull");
            proxyDE.Properties["objectSid"].Clear();
            proxyDE.Properties["objectSid"].Value = sidInBytes;
            proxyDE.Properties["userPrincipalName"].Value = name;
            proxyDE.CommitChanges();
            return FindByIdentity(context, name);
        }

        //work around way to create a new userProxyFull object
        public static void CreateProxy(string server, string path, string name, SecurityIdentifier sid)
        {
            var sidInBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidInBytes, 0);
            var ouDE = new DirectoryEntry(string.Format("LDAP://{0}/{1}", server, path));
            var proxyDE = ouDE.Children.Add(String.Format("CN={0}", name), "userProxyFull");
            proxyDE.Properties["objectSid"].Clear();
            proxyDE.Properties["objectSid"].Value = sidInBytes;
            proxyDE.Properties["userPrincipalName"].Value = name;
            proxyDE.CommitChanges();
        }

        #endregion

        #region Overloads

        public void SetPassword(string newPassword)
        {
            throw new NotSupportedException("SetPassword() not supported on UserProxyFullPrincipal objects.");
        }

        // Implement the overloaded search method FindByIdentity.
        public new static UserProxyFullPrincipal FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (UserProxyFullPrincipal) FindByIdentityWithType(context,
                typeof (UserProxyFullPrincipal),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static UserProxyFullPrincipal FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (UserProxyFullPrincipal) FindByIdentityWithType(context,
                typeof (UserProxyFullPrincipal),
                identityType,
                identityValue);
        }

        public class UserProxyFullPrincipalSearchFilter : AdvancedFilters
        {
            public UserProxyFullPrincipalSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var principal = obj as UserProxyFullPrincipal;
            if (principal != null)
            {
                var u = principal;
                return Sid.CompareTo(u.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var principal = obj as UserProxyFullPrincipal;
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