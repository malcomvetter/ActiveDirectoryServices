namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("computer")]
    public class ComputerPrincipalFull : ComputerPrincipal, IComparable
    {
        #region Constructors

        public ComputerPrincipalFull(PrincipalContext context)
            : base(context)
        {
        }

        public ComputerPrincipalFull(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        #endregion

        #region ExtendedAttributes

        // Summary:
        //     Gets or Sets the info - notes field for this Principal
        //
        // Returns:
        //     The info - notes field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("info")]
        public string Info
        {
            get
            {
                if (ExtensionGet("info").Length != 1)
                {
                    return null;
                }
                return (string) ExtensionGet("info")[0];
            }
            set { ExtensionSet("info", value); }
        }

        // Summary:
        //     Gets or Sets the email address for this Principal
        //
        // Returns:
        //     The email address of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("mail")]
        public string EmailAddress
        {
            get
            {
                if (ExtensionGet("mail").Length != 1)
                {
                    return null;
                }
                return (string) ExtensionGet("mail")[0];
            }
            set { ExtensionSet("mail", value); }
        }

        // Summary:
        //     Gets when this Principal was created.
        //
        // Returns:
        //     The date when this Principal was created.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("WhenCreated")]
        public DateTime WhenCreated
        {
            get
            {
                return ExtensionGet("WhenCreated").Length != 1
                    ? DateTime.MaxValue
                    : DateTime.ParseExact(ExtensionGet("whenCreated")[0].ToString(), "M/d/yyyy h:mm:ss tt", null);
            }
        }

        // Summary:
        //     Gets the modified date for this UserPrincipalFull
        //
        // Returns:
        //     The modified date of the UserPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("WhenChanged")]
        public DateTime WhenChanged
        {
            get
            {
                return ExtensionGet("WhenChanged").Length < 1
                    ? DateTime.MaxValue
                    : DateTime.ParseExact(ExtensionGet("WhenChanged")[0].ToString(), "M/d/yyyy h:mm:ss tt", null);
            }
        }

        // Summary:
        //     Gets or Sets the email address for this Principal
        //
        // Returns:
        //     The email address of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("operatingSystem")]
        public string OperatingSystem
        {
            get
            {
                if (ExtensionGet("operatingSystem").Length != 1)
                {
                    return null;
                }
                return (string) ExtensionGet("operatingSystem")[0];
            }
            set { ExtensionSet("operatingSystem", value); }
        }

        #endregion

        #region ExtendedLdapFunctions

        // Summary:
        //     Gets the root container for this Principal
        //
        // Returns:
        //     The root container of this Principal.
        //
        // Exceptions:
        //
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

        // Summary:
        //     Gets the container for this Principal
        //
        // Returns:
        //     The container of this Principal.
        //
        // Exceptions:
        //
        public string Container
        {
            get
            {
                var cn = DistinguishedName.Split(',')[0];
                cn += ",";
                return DistinguishedName.Replace(cn, string.Empty);
            }
        }

        // Summary:
        //     Gets the DNS name of the PrincipalContext for this Principal
        //
        // Returns:
        //     The DNS name of the PrincipalContext of this Principal.
        //
        // Exceptions:
        //
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

        // Summary:
        //     Performs an LDAP rename on this Principal
        //
        // Returns:
        //     The string value of the distinguished name of the renamed Principal.
        //
        // Exceptions:
        //
        public string Rename(string newName)
        {
            var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            de.Rename(string.Format("CN={0}", newName));
            return de.Path.Substring(7);
        }

        // Summary:
        //     Performs and LDAP move on this Principal
        //
        // Returns:
        //     Void
        //
        // Exceptions:
        // 
        public void Move(string ldapPath, string newName)
        {
            /* This whole method is needed because AccountManagement does not provide a Principal.Move() method.  
             * So we have to peel back layers of the onion and do it ourselves.
             */
            //For brevity, removed existence checks
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

        // Summary:
        //     Performs an LDAP move on this Principal
        //
        // Returns:
        //     Void
        //
        // Exceptions:
        //   
        public void Move(string ldapPath)
        {
            Move(ldapPath, string.Empty);
        }

        // Summary:
        //     Performs a recursive delete on this Principal
        //
        // Returns:
        //     Void
        //
        // Exceptions:
        //
        public void DeleteTree()
        {
            /* This whole method is needed because AccountManagement does not provide a Principal.DeleteTree() method.  
             * So we have to peel back layers of the onion and do it ourselves.
             */
            //For brevity, removed existence checks
            var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            if (ContextType == ContextType.ApplicationDirectory)
            {
                de = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Context.ConnectedServer, DistinguishedName));
            }
            de.DeleteTree();
            de.CommitChanges();
        }

        #endregion

        #region Overloads

        // Implement the overloaded search method FindByIdentity.
        public new static ComputerPrincipalFull FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (ComputerPrincipalFull) FindByIdentityWithType(context,
                typeof (ComputerPrincipalFull),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static ComputerPrincipalFull FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (ComputerPrincipalFull) FindByIdentityWithType(context,
                typeof (ComputerPrincipalFull),
                identityType,
                identityValue);
        }

        public class ComputerPrincipalFullSearchFilter : AdvancedFilters
        {
            public ComputerPrincipalFullSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var full = obj as ComputerPrincipalFull;
            if (full != null)
            {
                var c = full;
                return Sid.CompareTo(c.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var full = obj as ComputerPrincipalFull;
            if (full != null)
            {
                var c = full;
                return (c.Sid == Sid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}