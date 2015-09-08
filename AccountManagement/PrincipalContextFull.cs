using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace System.DirectoryServices.AccountManagement
{
    public class PrincipalContextFull : PrincipalContext
    {
        #region Constructors

        public PrincipalContextFull(ContextType contextType)
            : base(contextType)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name)
            : base(contextType, name)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name, string container)
            : base(contextType, name, container)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name, string container, ContextOptions options)
            : base(contextType, name, container, options)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name, string userName, string password)
            : base(contextType, name, userName, password)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name, string container, string userName,
            string password)
            : base(contextType, name, container, userName, password)
        {
        }

        public PrincipalContextFull(ContextType contextType, string name, string container, ContextOptions options,
            string userName, string password)
            : base(contextType, name, container, options, userName, password)
        {
        }

        #endregion

        #region ExtendedLdapFunctions

        public List<string> AllChildObjects
        {
            get
            {
                var directoryDe = new DirectoryEntry();
                if (ContextType == ContextType.ApplicationDirectory)
                {
                    if (!string.IsNullOrEmpty(Container) &&
                        !string.IsNullOrEmpty(Name))
                    {
                        directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                    }
                    else
                    {
                        directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                    }
                }
                if (ContextType == ContextType.Domain)
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
                }
                if (ContextType == ContextType.Machine)
                {
                    throw new NotSupportedException(
                        "This functionality is not available for Machine Context Type PrincipalContext objects.");
                }
                var search = new DirectorySearcher(directoryDe)
                {
                    Tombstone = false,
                    Asynchronous = true,
                    PageSize = 100,
                    Filter = "(objectClass=*)"
                };
                var results = search.FindAll();

                var i = 0;
                var children = new List<string>();
                foreach (SearchResult result in results)
                {
                    i++;
                    var delims = new[] {'/'};
                    var pieces = result.Path.Split(delims);
                    var dn = pieces[pieces.Count() - 1];
                    try
                    {
                        children.Add(dn);
                    }
                    catch
                    {
                    }
                }
                return children;
            }
        }

        public string ContextDns
        {
            get
            {
                var server = ConnectedServer.Split('.')[0] + ".";
                return ConnectedServer.Replace(server, string.Empty);
            }
        }

        public List<GenericDirectoryObject> GetAllChildObjects()
        {
            return GetAllChildObjects(-1);
        }

        public List<GenericDirectoryObject> GetAllChildObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=Top)"
            };
            var results = search.FindAll();

            var i = 0;
            var children = new List<GenericDirectoryObject>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(GenericDirectoryObject.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetAllChildDNs()
        {
            return GetAllChildDNs(-1);
        }

        public List<string> GetAllChildDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=Top)"
            };
            var results = search.FindAll();

            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<UserPrincipalFull> GetChildUserObjects()
        {
            return GetChildUserObjects(-1);
        }

        public List<UserPrincipalFull> GetChildUserObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(&(objectCategory=Person)(objectClass=User))"
            };
            var results = search.FindAll();

            var i = 0;
            var children = new List<UserPrincipalFull>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(UserPrincipalFull.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildUserDNs()
        {
            return GetChildUserDNs(-1);
        }

        public List<string> GetChildUserDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(&(objectCategory=Person)(objectClass=User))"
            };
            var results = search.FindAll();

            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<GroupPrincipalFull> GetChildGroupObjects()
        {
            return GetChildGroupObjects(-1);
        }

        public List<GroupPrincipalFull> GetChildGroupObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=group)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<GroupPrincipalFull>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];

                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(GroupPrincipalFull.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildGroupDNs()
        {
            return GetChildGroupDNs(-1);
        }

        public List<string> GetChildGroupDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.Machine)
            {
                throw new NotSupportedException(
                    "This functionality is not available for Machine Context Type PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=group)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<ComputerPrincipalFull> GetChildComputerObjects()
        {
            return GetChildComputerObjects(-1);
        }

        public List<ComputerPrincipalFull> GetChildComputerObjects(int maxRecords)
        {
            DirectoryEntry directoryDe;
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            else
            {
                throw new NotSupportedException(
                    "This functionality is only available for Domain ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=computer)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<ComputerPrincipalFull>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(ComputerPrincipalFull.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildComputerDNs()
        {
            return GetChildComputerDNs(-1);
        }

        public List<string> GetChildComputerDNs(int maxRecords)
        {
            DirectoryEntry directoryDe;
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            else
            {
                throw new NotSupportedException(
                    "This functionality is only available for Domain ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=computer)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<UserProxyFullPrincipal> GetChildUserProxyFullObjects()
        {
            return GetChildUserProxyFullObjects(-1);
        }

        public List<UserProxyFullPrincipal> GetChildUserProxyFullObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Machine ||
                ContextType == ContextType.Domain)
            {
                throw new NotSupportedException(
                    "This functionality is only available for ApplicationDirectory ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=userProxyFull)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<UserProxyFullPrincipal>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(UserProxyFullPrincipal.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildUserProxyFullDNs()
        {
            return GetChildUserProxyFullDNs(-1);
        }

        public List<string> GetChildUserProxyFullDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Machine ||
                ContextType == ContextType.Domain)
            {
                throw new NotSupportedException(
                    "This functionality is only available for ApplicationDirectory ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=userProxyFull)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<UserProxyPrincipal> GetChildUserProxyObjects()
        {
            return GetChildUserProxyObjects(-1);
        }

        public List<UserProxyPrincipal> GetChildUserProxyObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Machine ||
                ContextType == ContextType.Domain)
            {
                throw new NotSupportedException(
                    "This functionality is only available for ApplicationDirectory ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=userProxy)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<UserProxyPrincipal>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(UserProxyPrincipal.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildUserProxyDNs()
        {
            return GetChildUserProxyDNs(-1);
        }

        public List<string> GetChildUserProxyDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.ApplicationDirectory)
            {
                if (!string.IsNullOrEmpty(Container) &&
                    !string.IsNullOrEmpty(Name))
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", Name, Container));
                }
                else
                {
                    directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", Name));
                }
            }
            if (ContextType == ContextType.Machine ||
                ContextType == ContextType.Domain)
            {
                throw new NotSupportedException(
                    "This functionality is only available for ApplicationDirectory ContextType PrincipalContext objects.");
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=userProxy)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public List<ContactPrincipal> GetChildContactObjects()
        {
            return GetChildContactObjects(-1);
        }

        public List<ContactPrincipal> GetChildContactObjects(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.ApplicationDirectory)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", ConnectedServer, Container));
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=contact)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<ContactPrincipal>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(ContactPrincipal.FindByIdentity(this, IdentityType.DistinguishedName, dn));
                }
                catch
                {
                }
            }
            return children;
        }

        public List<string> GetChildContactObjectDNs()
        {
            return GetChildContactObjectDNs(-1);
        }

        public List<string> GetChildContactObjectDNs(int maxRecords)
        {
            var directoryDe = new DirectoryEntry();
            if (ContextType == ContextType.Domain)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}", ConnectedServer));
            }
            if (ContextType == ContextType.ApplicationDirectory)
            {
                directoryDe = new DirectoryEntry(string.Format("LDAP://{0}/{1}", ConnectedServer, Container));
            }
            var search = new DirectorySearcher(directoryDe)
            {
                Tombstone = false,
                Asynchronous = true,
                PageSize = 100,
                Filter = "(objectClass=contact)"
            };
            var results = search.FindAll();
            var i = 0;
            var children = new List<string>();
            foreach (SearchResult result in results)
            {
                i++;
                var delims = new[] {'/'};
                var pieces = result.Path.Split(delims);
                var dn = pieces[pieces.Count() - 1];
                if (maxRecords > 0 && i > maxRecords)
                {
                    break;
                }
                try
                {
                    children.Add(dn);
                }
                catch
                {
                }
            }
            return children;
        }

        public bool ValidateCredentialsAndLogon(string userName, string password)
        {
            var path = Domain.GetComputerDomain().GetDirectoryEntry().Path;
            var domain = Domain.GetComputerDomain().Name;
            var domainAndUsername = domain + @"\" + userName;
            var entry = new DirectoryEntry(path, domainAndUsername, password);
            try
            {
                //Bind to the native AdsObject to force authentication.
                var search = new DirectorySearcher(entry)
                {
                    Filter = "(SAMAccountName=" + userName + ")"
                };
                search.PropertiesToLoad.Add("cn");
                var result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}