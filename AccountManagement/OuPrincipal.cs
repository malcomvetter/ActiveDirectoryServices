namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("OU")]
    [DirectoryObjectClass("organizationalUnit")]
    public class OuPrincipalFull : GroupPrincipalFull, IComparable
    {
        #region Constructors

        public OuPrincipalFull(PrincipalContext context)
            : base(context)
        {
        }

        public OuPrincipalFull(PrincipalContext context, string samAccountName)
            : base(context, samAccountName)
        {
        }

        #endregion

        #region ExtendedLdapFunctions

        public string RootContainer
        {
            get
            {
                var rootDn = string.Empty;
                foreach (var element in DistinguishedName.Split(','))
                {
                    if (element.ToLowerInvariant().StartsWith("DC=".ToLowerInvariant()))
                    {
                        rootDn += element + ",";
                    }
                }
                return rootDn.Substring(0, rootDn.Length - 1);
            }
        }

        public string Container
        {
            get
            {
                var cn = DistinguishedName.Split(',')[0];
                cn += ",";
                return DistinguishedName.Replace(cn, string.Empty);
            }
        }

        public string ContextDns
        {
            get
            {
                var server = Context.ConnectedServer.Split('.')[0] + ".";
                return Context.ConnectedServer.Replace(server, string.Empty);
            }
        }

        public object[] GetAttribute(string attribute)
        {
            return (ExtensionGet(attribute));
        }

        // Summary:
        //     Sets the object[] of values for the extended attribute passed by name on this UserPrincipalFull
        //
        // Returns:
        //     Void
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public void SetAttribute(string attribute, object[] value)
        {
            ExtensionSet(attribute, value);
        }

        public string Rename(string newName)
        {
            var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            de.Rename(string.Format("OU={0}", newName));
            return de.Path.Substring(7);
        }

        public void Move(string ldapPath, string newName)
        {
            //This whole method is needed because AccountManagement does not provide a Principal.Move() method.  
            //So we have to peel back layers of the onion and do it ourselves.
            //For brevity, removed existence checks...
            var eLocation = new DirectoryEntry("LDAP://" + DistinguishedName);
            var nLocation = new DirectoryEntry("LDAP://" + ldapPath);
            if (string.IsNullOrEmpty(newName))
            {
                newName = eLocation.Name;
            }
            eLocation.MoveTo(nLocation, newName);
            nLocation.Close();
            eLocation.Close();
        }

        public void Move(string ldapPath)
        {
            Move(ldapPath, string.Empty);
        }

        #endregion

        #region Overloads

        // Implement the overloaded search method FindByIdentity.
        public new static OuPrincipalFull FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (OuPrincipalFull) FindByIdentityWithType(context,
                typeof (OuPrincipalFull),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static OuPrincipalFull FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (OuPrincipalFull) FindByIdentityWithType(context,
                typeof (OuPrincipalFull),
                identityType,
                identityValue);
        }

        public class OuPrincipalFullSearchFilter : AdvancedFilters
        {
            public OuPrincipalFullSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var full = obj as OuPrincipalFull;
            if (full != null)
            {
                var o = full;
                return DistinguishedName.ToLowerInvariant().CompareTo(o.DistinguishedName.ToLowerInvariant());
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var full = obj as OuPrincipalFull;
            if (full != null)
            {
                var o = full;
                return (o.DistinguishedName.ToLowerInvariant() == DistinguishedName.ToLowerInvariant());
            }
            return false;
        }

        #endregion

        #endregion
    }
}