namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("contact")]
    public class ContactPrincipal : GenericDirectoryObject, IComparable
    {
        #region Constructors

        public ContactPrincipal(PrincipalContext context)
            : base(context)
        {
        }

        #endregion

        #region ExtendedLdapFunctions

        //work around way to create a new contact object
        public static ContactPrincipal CreateContact(PrincipalContext context, string name)
        {
            var ouDe = new DirectoryEntry();
            if (context.ContextType == ContextType.Domain)
            {
                ouDe = new DirectoryEntry(string.Format("LDAP://{0}", context.Container));
            }
            if (context.ContextType == ContextType.ApplicationDirectory)
            {
                ouDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", context.ConnectedServer, context.Container));
            }
            var contactDe = ouDe.Children.Add(String.Format("CN={0}", name), "contact");
            contactDe.CommitChanges();
            return FindByIdentity(context, name);
        }

        //work around way to create a new contact object
        public static void CreateContact(string server, string path, string name)
        {
            var ouDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", server, path));
            var contactDe = ouDe.Children.Add(String.Format("CN={0}", name), "contact");
            contactDe.CommitChanges();
        }

        #endregion

        #region Overrides

        public void SetPassword(string newPassword)
        {
            throw new NotSupportedException("SetPassword() not supported on ContactPrincipal objects.");
        }

        // Implement the overloaded search method FindByIdentity.
        public new static ContactPrincipal FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (ContactPrincipal) FindByIdentityWithType(context,
                typeof (ContactPrincipal),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static ContactPrincipal FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (ContactPrincipal) FindByIdentityWithType(context,
                typeof (ContactPrincipal),
                identityType,
                identityValue);
        }

        public class ContactPrincipalSearchFilter : AdvancedFilters
        {
            public ContactPrincipalSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var principal = obj as ContactPrincipal;
            if (principal != null)
            {
                var u = principal;
                return String.Compare(Guid.ToString(), u.Guid.ToString(), StringComparison.Ordinal);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var principal = obj as ContactPrincipal;
            if (principal != null)
            {
                var u = principal;
                return (u.Guid == Guid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}