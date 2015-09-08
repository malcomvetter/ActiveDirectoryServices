namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("user")]
    public class UserPrincipalFull : GenericDirectoryObject, IComparable
    {
        #region Constructors

        public UserPrincipalFull(PrincipalContext context)
            : base(context)
        {
        }

        public UserPrincipalFull(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        #endregion

        #region Overloads

        public new static UserPrincipalFull FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (UserPrincipalFull) FindByIdentityWithType(context,
                typeof (UserPrincipalFull),
                identityValue);
        }

        public new static UserPrincipalFull FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (UserPrincipalFull) FindByIdentityWithType(context,
                typeof (UserPrincipalFull),
                identityType,
                identityValue);
        }

        public class UserPrincipalFullSearchFilter : AdvancedFilters
        {
            public UserPrincipalFullSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var full = obj as UserPrincipalFull;
            if (full != null)
            {
                var u = full;
                return Sid.CompareTo(u.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var full = obj as UserPrincipalFull;
            if (full != null)
            {
                var u = full;
                return (u.Sid == Sid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}