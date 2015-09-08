using System.Collections.Generic;
using System.Linq;

namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("group")]
    public class GroupPrincipalFull : GroupPrincipal, IComparable
    {
        #region Constructors

        public GroupPrincipalFull(PrincipalContext context)
            : base(context)
        {
        }

        public GroupPrincipalFull(PrincipalContext context, string samAccountName)
            : base(context, samAccountName)
        {
        }

        #endregion

        #region ExtendedAttributes

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
        //     Gets or sets the string value of the DistinguishedName of the Manager for this UserPrincipalFull
        //
        // Returns:
        //     The string value of the DistinguishedName of the Manager of the UserPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("manager")]
        public string Manager
        {
            get
            {
                if (ExtensionGet("manager").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("manager")[0];
            }
            set { ExtensionSet("manager", value); }
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
        //     Gets or sets the string value of the DistinguishedName of the managedBy for this GroupPrincipalFull
        //
        // Returns:
        //     The string value of the DistinguishedName of the managedBy of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("managedBy")]
        public string ManagedBy
        {
            get
            {
                if (ExtensionGet("managedBy").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("managedBy")[0];
            }
            set { ExtensionSet("managedBy", value); }
        }

        // Summary:
        //     Gets or sets the gidNumber for this GroupPrincipalFull
        //
        // Returns:
        //     The gidNumber of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("gidNumber")]
        public int GidNumber
        {
            get { return (int) ExtensionGet("gidNumber")[0]; }
            set { ExtensionSet("gidNumber", value); }
        }

        // Summary:
        //     Gets or sets the targetAddress for this GroupPrincipalFull
        //
        // Returns:
        //     The targetAddress of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("targetAddress")]
        public string TargetAddress
        {
            get
            {
                if (ExtensionGet("targetAddress").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("targetAddress")[0];
            }
            set { ExtensionSet("targetAddress", value); }
        }

        // Summary:
        //     Gets or sets the mailNickname for this GroupPrincipalFull
        //
        // Returns:
        //     The mailNickname of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("mailNickname")]
        public string MailNickname
        {
            get
            {
                if (ExtensionGet("mailNickname").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("mailNickname")[0];
            }
            set { ExtensionSet("mailNickname", value); }
        }

        // Summary:
        //     Gets or sets the Proxy Addresses for this GroupPrincipalFull
        //
        // Returns:
        //     The Proxy Addresses of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("proxyAddresses")]
        public List<string> ProxyAddresses
        {
            get
            {
                var proxyAddresses = new List<string>();
                var i = 0;
                do
                {
                    try
                    {
                        proxyAddresses.Add((string) ExtensionGet("proxyAddresses")[i]);
                        i++;
                    }
                    catch
                    {
                        i = -1;
                    }
                } while (i >= 0);
                return proxyAddresses;
            }
            set
            {
                var o = new object[value.Count];
                var i = 0;
                foreach (var s in value)
                {
                    o[i] = s;
                    i++;
                }
                ExtensionSet("proxyAddresses", o);
            }
        }

        // Summary:
        //     Gets or sets the showInAddressBook for this GroupPrincipalFull
        //
        // Returns:
        //     The showInAddressBook of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("showInAddressBook")]
        public List<string> ShowInAddressBook
        {
            get
            {
                var showInAddressBook = new List<string>();
                var i = 0;
                do
                {
                    try
                    {
                        showInAddressBook.Add((string) ExtensionGet("showInAddressBook")[i]);
                        i++;
                    }
                    catch
                    {
                        i = -1;
                    }
                } while (i >= 0);
                return showInAddressBook;
            }
            set
            {
                var o = new object[value.Count];
                var i = 0;
                foreach (var s in value)
                {
                    o[i] = s;
                    i++;
                }
                ExtensionSet("showInAddressBook", o);
            }
        }

        // Summary:
        //     Gets or sets the legacyExchangeDN for this GroupPrincipalFull
        //
        // Returns:
        //     The legacyExchangeDN of the GroupPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("legacyExchangeDN")]
        public string LegacyExchangeDn
        {
            get
            {
                if (ExtensionGet("legacyExchangeDN").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("legacyExchangeDN")[0];
            }
            set { ExtensionSet("legacyExchangeDN", value); }
        }

        // Summary:
        //     Gets or sets the adminSdHolderLock value for this UserPrincipalFull
        //
        // Returns:
        //     The adminSdHolderLock value of the UserPrincipalFull.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("groupType")]
        public GroupType GroupType
        {
            get
            {
                try
                {
                    return (GroupType) ExtensionGet("groupType")[0];
                }
                catch (NullReferenceException)
                {
                    return GroupType.Global & GroupType.Security;
                }
            }
            set { ExtensionSet("groupType", value); }
        }

        #endregion

        #region ExtendedLdapFunctions

        // Summary:
        //     Gets a List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     The List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> MemberDNs
        {
            get { return GetMemberDNs(); }
        }

        public List<string> MembershipDNs
        {
            get { return GetMembershipDNs(); }
        }

        public bool HasMembers
        {
            get
            {
                var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
                var searcher = new DirectorySearcher(de)
                {
                    Filter = "(objectClass=*)"
                };
                var attribute = "member";
                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add(attribute);
                var results = searcher.FindOne();

                if (results.Properties[attribute].Count > 0)
                {
                    return true;
                }
                //double check to make sure it's not a large group
                attribute = String.Format("member;range={0}-{1}", 0, 0);
                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add(attribute);
                results = searcher.FindOne();
                return results.Properties[attribute].Count > 0;
            }
        }

        // Summary:
        //     Determines whether this GroupPrincipalFull has 3 or fewer members
        //
        // Returns:
        //     True if this GruopPrincipalFull has 3 or fewer members
        //
        // Exceptions:
        // 
        public bool HasFewMembers
        {
            get
            {
                const int few = 3;
                var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
                var searcher = new DirectorySearcher(de)
                {
                    Filter = "(objectClass=*)"
                };
                var attribute = "member";
                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add(attribute);
                var results = searcher.FindOne();

                if (results.Properties[attribute].Count > 0)
                {
                    return results.Properties[attribute].Count <= few;
                }
                //double check to make sure it's not a large group
                attribute = String.Format("member;range={0}-{1}", 0, 0);
                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add(attribute);
                results = searcher.FindOne();
                if (results.Properties[attribute].Count == 0)
                {
                    return false;
                }
                return results.Properties[attribute].Count <= few;
            }
        }

        // Summary:
        //     Gets the root container of this principal
        //
        // Returns:
        //     The root container of this principal
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
        //     Gets the container of this principal
        //
        // Returns:
        //     The container of this principal
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
        //     The DNS name of the PrincipalContext for this Principal
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

        // Summary:
        //     Gets a List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     The List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> GetMemberDNs()
        {
            var members = ExtensionGet("member").Cast<string>().ToList();
            if (members.Count == 0)
            {
                members.AddRange(GetMemberDNs(false));
            }
            return members;
        }

        // Summary:
        //     Gets a List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     The List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> GetMemberDNs(bool recursive)
        {
            return GetMemberDNs(recursive, false, 1000);
        }

        // Summary:
        //     Gets a List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     The List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> GetMemberDNs(bool recursive, bool searchOtherDomains, uint rangeStep)
        {
            var de = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            var searcher = new DirectorySearcher(de)
            {
                SearchRoot = de,
                Filter = "(objectClass=*)"
            };
            var members = new List<string>();
            //uint rangeStep = 1000;
            uint rangeLow = 0;
            var rangeHigh = rangeLow + (rangeStep - 1);
            var lastQuery = false;
            var quitLoop = false;
            uint count = 0;

            do
            {
                string attributeWithRange;
                attributeWithRange = !lastQuery
                    ? String.Format("member;range={0}-{1}", rangeLow, rangeHigh)
                    : String.Format("member;range={0}-*", rangeLow);
                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add(attributeWithRange);
                var results = searcher.FindOne();
                if (results.Properties.Contains(attributeWithRange))
                {
                    foreach (var obj in results.Properties[attributeWithRange])
                    {
                        count++;
                        var s = obj as string;
                        if (s != null)
                        {
                            members.Add(s);
                        }
                        else if (obj is int)
                        {
                        }
                    }
                    if (lastQuery)
                    {
                        quitLoop = true;
                    }
                }
                else
                {
                    lastQuery = true;
                }
                if (lastQuery)
                {
                    continue;
                }
                rangeLow = rangeHigh + 1;
                rangeHigh = rangeLow + (rangeStep - 1);
            } while (!quitLoop);

            var membersToAdd = new List<string>();
            foreach (var obj in members)
            {
                if (!(obj.ToLowerInvariant().Contains(RootContainer.ToLowerInvariant())) && !searchOtherDomains)
                {
                    continue;
                }
                if (!recursive)
                {
                    continue;
                }
                var gpf = FindByIdentity(new PrincipalContext(ContextType.Domain), IdentityType.DistinguishedName, obj);
                try
                {
                    if (!string.IsNullOrEmpty(gpf.DistinguishedName))
                    {
                        membersToAdd.AddRange(gpf.GetMemberDNs(recursive, searchOtherDomains, rangeStep));
                    }
                }
                catch
                {
                    continue;
                }
            }
            members.AddRange(membersToAdd);
            return members;
        }

        // Summary:
        //     Gets a List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     The List<string> of distinguished names of members belonging to this GroupPrincipalFull
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> GetMembershipDNs()
        {
            var memberships = ExtensionGet("memberOf").Cast<string>().ToList();
            if (memberships.Count == 0)
            {
                memberships.AddRange(GetMemberDNs(false));
            }
            return memberships;
        }

        // Summary:
        //     Adds the distinguished name of the supplied Principal object from the member attribute belonging to this GroupPrincipalFull
        //
        // Returns:
        //     void
        //
        // Exceptions:
        //
        public void AddMember(Principal principal)
        {
            var groupDe = new DirectoryEntry();
            if (Context.ContextType == ContextType.Domain)
            {
                groupDe = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            }
            if (Context.ContextType == ContextType.ApplicationDirectory)
            {
                groupDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Context.ConnectedServer, DistinguishedName));
            }
            groupDe.Properties["member"].Add(principal.DistinguishedName);
            groupDe.CommitChanges();
            groupDe.Close();
        }

        // Summary:
        //     Removes the distinguished name of the supplied Principal object from the member attribute belonging to this GroupPrincipalFull
        //
        // Returns:
        //     void
        //
        // Exceptions:
        //
        public void RemoveMember(Principal principal)
        {
            var groupDe = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            groupDe.Properties["member"].Remove(principal.DistinguishedName);
            groupDe.CommitChanges();
            groupDe.Close();
        }

        // Summary:
        //     Removes all members belonging to this GroupPrincipalFull
        //
        // Returns:
        //     void
        //
        // Exceptions:
        //
        public void RemoveAllMembers()
        {
            var groupDe = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            groupDe.Properties["member"].Clear();
            groupDe.CommitChanges();
            groupDe.Close();
        }

        // Summary:
        //     Determines whether this GroupPrincipalFull has members
        //
        // Returns:
        //     True if this GruopPrincipalFull has members
        //
        // Exceptions:
        // 

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
            var DE = new DirectoryEntry(string.Format("LDAP://{0}", DistinguishedName));
            if (ContextType == ContextType.ApplicationDirectory)
            {
                DE = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Context.ConnectedServer, DistinguishedName));
            }
            DE.DeleteTree();
            DE.CommitChanges();
        }

        #endregion

        #region Overloads

        // Implement the overloaded search method FindByIdentity.
        public new static GroupPrincipalFull FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (GroupPrincipalFull) FindByIdentityWithType(context,
                typeof (GroupPrincipalFull),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static GroupPrincipalFull FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (GroupPrincipalFull) FindByIdentityWithType(context,
                typeof (GroupPrincipalFull),
                identityType,
                identityValue);
        }

        public class GroupPrincipalFullSearchFilter : AdvancedFilters
        {
            public GroupPrincipalFullSearchFilter(Principal p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var full = obj as GroupPrincipalFull;
            if (full != null)
            {
                var g = full;
                return Sid.CompareTo(g.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var full = obj as GroupPrincipalFull;
            if (full != null)
            {
                var g = full;
                return (g.Sid == Sid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}