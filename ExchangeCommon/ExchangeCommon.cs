using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace ExchangeCommon
{
    public static class ExchangeCommon
    {
        public static ExchangeConfiguration Config = new ExchangeConfiguration();

        public static void MailEnable(GenericDirectoryObject directoryObject, bool generateFriendlyAddress)
        {
            MailEnable(directoryObject, generateFriendlyAddress, true);
        }

        public static void MailEnable(GenericDirectoryObject directoryObject, bool generateFriendlyAddress,
            bool verifyUniqueness)
        {
            var email = string.Empty;
            var emailDomain = string.Empty;
            var handle = string.Empty;
            try
            {
                handle = directoryObject.SamAccountName;
            }
            catch
            {
            }
            if (string.IsNullOrWhiteSpace(handle))
            {
                handle = directoryObject.Name;
            }
            try
            {
                email = directoryObject.EmailAddress;
            }
            catch
            {
            }
            try
            {
                emailDomain = email.Split(new[] {'@'})[1];
            }
            catch
            {
            }
            try
            {
                if (email.Trim() != directoryObject.EmailAddress)
                {
                    directoryObject.EmailAddress = email.Trim();
                    directoryObject.Save();
                    email = directoryObject.EmailAddress;
                }
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }
            string friendlyEmail;
            if (IsManagedDomain(emailDomain))
            {
                //firstname.lastname@domain
                friendlyEmail = string.Format("smtp:{0}@{1}",
                    directoryObject.DisplayName.Replace(" ", ".").Replace("'", ""), emailDomain);
                if (!verifyUniqueness || !IsEmailUnique(friendlyEmail, directoryObject.DistinguishedName))
                {
                    //flastname@domain
                    friendlyEmail = string.Format("smtp:{0}@{1}",
                        directoryObject.DisplayName.Substring(0, 1) + directoryObject.Surname, emailDomain);
                    if (!verifyUniqueness || !IsEmailUnique(friendlyEmail, directoryObject.DistinguishedName))
                    {
                        //userID@domain
                        friendlyEmail = string.Format("smtp:{0}@{1}", handle, emailDomain);
                    }
                }
            }
            else
            {
                friendlyEmail = string.Format("smtp:{0}", email);
            }
            try
            {
                if (generateFriendlyAddress)
                {
                    directoryObject.TargetAddress = friendlyEmail;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(directoryObject.SamAccountName))
                    {
                        directoryObject.TargetAddress = IsManagedDomain(emailDomain)
                            ? string.Format("smtp:{0}@{1}", handle, emailDomain)
                            : string.Format("smtp:{0}", directoryObject.EmailAddress);
                    }
                    else
                    {
                        directoryObject.TargetAddress = directoryObject.ObjectClass.Contains("contact")
                            ? string.Format("smtp:{0}", directoryObject.EmailAddress)
                            : string.Format("smtp:{0}@{1}", directoryObject.Name, emailDomain);
                    }
                }
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.MailNickname = handle;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                var proxyAddresses = new List<string>();
                try
                {
                    proxyAddresses = directoryObject.ProxyAddresses;
                }
                catch
                {
                }
                proxyAddresses.Add(string.Format("SMTP:{0}", directoryObject.EmailAddress));
                proxyAddresses.AddRange(
                    Config.RoutingDomains.Select(domain => string.Format("smtp:{0}@{1}", handle, domain)));
                proxyAddresses.AddRange(
                    Config.RoutingDomains.Select(
                        domain => string.Format("{0}@{1}", friendlyEmail.Split(new char[1] {'@'})[0], domain)));
                directoryObject.ProxyAddresses = proxyAddresses;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                var showInAddressBook = new List<string>();
                try
                {
                    showInAddressBook = directoryObject.ShowInAddressBook;
                }
                catch
                {
                }
                showInAddressBook.AddRange(Config.ShowInAddressBookDefault);
                directoryObject.ShowInAddressBook = showInAddressBook;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                //will require editing for your environment or modifying this to be configurable:
                directoryObject.LegacyExchangeDn = string.Format("/o=Company/ou=Legacy/cn=Recipients/cn={0}",
                    directoryObject.SamAccountName);
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.UserPrincipalName = string.Format("{0}@{1}",
                    directoryObject.UserPrincipalName.Split(new[] {'@'})[0], Config.UpnSuffix);
                directoryObject.Save();
            }
            catch
            {
            }
        }

        public static void AddRoomAttributes(GenericDirectoryObject directoryObject)
        {
            try
            {
                var showInAddressBook = directoryObject.ShowInAddressBook;
                showInAddressBook.AddRange(Config.ShowInAddressBookRooms);
                directoryObject.ShowInAddressBook = showInAddressBook;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.MsExchResourceDisplay = "Room";
                directoryObject.MsExchResourceDisplayType = 7;

                var MsExchResourceMetaData = directoryObject.MsExchResourceMetaData;
                MsExchResourceMetaData.Add("ResourceType:Room");
                directoryObject.MsExchResourceMetaData = MsExchResourceMetaData;

                var MsExchResourceSearchProperties = directoryObject.MsExchResourceSearchProperties;
                MsExchResourceSearchProperties.Add("Room");
                directoryObject.MsExchResourceSearchProperties = MsExchResourceSearchProperties;

                directoryObject.Save();
            }
            catch
            {
            }
        }

        public static void AddResourceAttributes(GenericDirectoryObject directoryObject)
        {
            try
            {
                var showInAddressBook = directoryObject.ShowInAddressBook;
                showInAddressBook.AddRange(Config.ShowInAddressBookResources);
                directoryObject.ShowInAddressBook = showInAddressBook;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.MsExchResourceDisplay = "Equipment";
                directoryObject.MsExchResourceDisplayType = 8;

                var MsExchResourceMetaData = directoryObject.MsExchResourceMetaData;
                MsExchResourceMetaData.Add("ResourceType:Equipment");
                directoryObject.MsExchResourceMetaData = MsExchResourceMetaData;

                var MsExchResourceSearchProperties = directoryObject.MsExchResourceSearchProperties;
                MsExchResourceSearchProperties.Add("Equipment");
                directoryObject.MsExchResourceSearchProperties = MsExchResourceSearchProperties;

                directoryObject.Save();
            }
            catch
            {
            }
        }

        public static void AddAlias(GenericDirectoryObject directoryObject, string address, bool makeDefault)
        {
            var handle = address.Split(new[] {'@'})[0];
            var proxyAddresses = new List<string>();
            var nicknameProxies = false;
            if (makeDefault)
            {
                try
                {
                    directoryObject.EmailAddress = address.Trim();
                    directoryObject.TargetAddress = string.Format("smtp:{0}", address);
                    if (string.IsNullOrWhiteSpace(directoryObject.MailNickname))
                    {
                        directoryObject.MailNickname = handle;
                        nicknameProxies = true;
                    }
                    directoryObject.Save();
                }
                catch
                {
                    directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                        directoryObject.DistinguishedName);
                }
            }
            try
            {
                proxyAddresses = directoryObject.ProxyAddresses;
            }
            catch
            {
            }
            var newProxyAddresses = new List<string>();
            foreach (var proxyAddress in proxyAddresses)
            {
                if (proxyAddress.Substring(0, 5).ToLowerInvariant() == "smtp:")
                {
                    var s = proxyAddress.Split(new[] {':'});
                    if (s[1].ToLowerInvariant() == address.ToLowerInvariant())
                    {
                        newProxyAddresses.Add(makeDefault
                            ? string.Format("SMTP:{0}", address)
                            : string.Format("smtp:{0}", address));
                    }
                    else
                    {
                        newProxyAddresses.Add(proxyAddress.Replace("SMTP", "smtp"));
                    }
                }
                else
                {
                    newProxyAddresses.Add(proxyAddress);
                }
            }
            if (nicknameProxies)
            {
                newProxyAddresses.AddRange(
                    Config.RoutingDomains.Select(domain => string.Format("smtp:{0}@{1}", handle, domain)));
            }
            try
            {
                directoryObject.SetAttribute("proxyAddresses", null);
                directoryObject.Save();
            }
            catch
            {
            }
            finally
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            directoryObject.ProxyAddresses = newProxyAddresses;
            directoryObject.Save();
        }

        public static void MailDisable(GenericDirectoryObject directoryObject)
        {
            try
            {
                directoryObject.EmailAddress = null;
                directoryObject.TargetAddress = null;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.MailNickname = null;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            var proxyAddresses = new List<string>();
            try
            {
                proxyAddresses.AddRange(
                    directoryObject.ProxyAddresses.Where(
                        address => address.Substring(0, 5).ToLowerInvariant() != "smtp:"));
            }
            catch
            {
            }
            try
            {
                if (proxyAddresses.Count > 0)
                {
                    directoryObject.ProxyAddresses = proxyAddresses;
                    directoryObject.Save();
                }
                else
                {
                    directoryObject.SetAttribute("proxyAddresses", null);
                    directoryObject.Save();
                }
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.SetAttribute("showInAddressBook", null);
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
            try
            {
                directoryObject.LegacyExchangeDn = null;
                directoryObject.Save();
            }
            catch
            {
                directoryObject = GenericDirectoryObject.FindByIdentity(directoryObject.Context,
                    directoryObject.DistinguishedName);
            }
        }

        public static List<GenericDirectoryObject> FixGalCollision(string identity, string address, bool updateManager)
        {
            var domains = new List<Domain>();
            var ad = new PrincipalContextFull(ContextType.Domain);
            foreach (Domain d in Forest.GetCurrentForest().Domains)
            {
                try
                {
                    ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    if (!string.IsNullOrWhiteSpace(ad.ConnectedServer))
                    {
                        domains.Add(d);
                    }
                }
                catch
                {
                }
            }
            var authorizedIdentity = new GenericDirectoryObject(ad);
            foreach (var d in domains)
            {
                try
                {
                    ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    authorizedIdentity = GenericDirectoryObject.FindByIdentity(ad, identity);
                    if (!string.IsNullOrWhiteSpace(authorizedIdentity.DistinguishedName))
                    {
                        break;
                    }
                }
                catch
                {
                }
            }
            if (string.IsNullOrWhiteSpace(authorizedIdentity.DistinguishedName))
            {
                throw new ArgumentException("Invalid identity - not found: {0}", identity);
            }
            IEnumerable<Principal> results = null;
            foreach (var d in domains)
            {
                try
                {
                    ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    var filter = new GenericDirectoryObject(ad) {EmailAddress = address};
                    var ps = new PrincipalSearcher(filter);
                    results = results == null ? ps.FindAll() : results.Concat(ps.FindAll());
                    filter = new GenericDirectoryObject(ad)
                    {
                        ProxyAddresses = new List<string>
                        {
                            string.Format("smtp:{0}", address)
                        }
                    };
                    ps = new PrincipalSearcher(filter);
                    results = results.Concat(ps.FindAll());
                    filter = new GenericDirectoryObject(ad)
                    {
                        TargetAddress = string.Format("smtp:{0}", address)
                    };
                    ps = new PrincipalSearcher(filter);
                    results = results.Concat(ps.FindAll());
                }
                catch
                {
                }
            }

            var matches = new List<GenericDirectoryObject>();
            foreach (var result in results)
            {
                var gdo = new GenericDirectoryObject(ad);
                foreach (var d in domains)
                {
                    try
                    {
                        ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                        gdo = GenericDirectoryObject.FindByIdentity(ad, result.DistinguishedName);
                        if (!string.IsNullOrWhiteSpace(gdo.DistinguishedName))
                        {
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
                matches.Add(gdo);
            }
            matches.Sort();
            var prevDN = string.Empty;
            var sortedMatches = new List<GenericDirectoryObject>();
            foreach (
                var _gdo in
                    matches.Where(_gdo => prevDN.ToLowerInvariant() != _gdo.DistinguishedName.ToLowerInvariant()))
            {
                prevDN = _gdo.DistinguishedName;
                sortedMatches.Add(_gdo);
            }
            IEnumerable<GenericDirectoryObject> iMatches = sortedMatches;
            iMatches.Distinct().OrderBy(x => x);

            foreach (var gdo in iMatches)
            {
                if (gdo.DistinguishedName.ToLowerInvariant() == authorizedIdentity.DistinguishedName.ToLowerInvariant())
                {
                    MailEnable(gdo, false, false);
                }
                else
                {
                    MailDisable(gdo);
                    if (updateManager)
                    {
                        try
                        {
                            gdo.Manager = authorizedIdentity.DistinguishedName;
                            gdo.Save();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return AddressSearch(address);
        }

        public static List<string> GetSmtpAliases(GenericDirectoryObject directoryObject)
        {
            var Addresses = new List<string> {directoryObject.EmailAddress};
            foreach (var proxyAddress in directoryObject.ProxyAddresses)
            {
                if (proxyAddress.Substring(0, 5).ToLowerInvariant() == "smtp:".ToLowerInvariant())
                {
                    Addresses.Add(proxyAddress.Substring(5));
                }
            }
            IEnumerable<string> unsorted = Addresses;
            unsorted = unsorted.Distinct();
            return unsorted.ToList();
        }

        public static bool IsEmailUnique(string emailQuery)
        {
            return IsEmailUnique(emailQuery, true);
        }

        public static bool IsEmailUnique(string emailQuery, bool searchOtherDomains)
        {
            var domains = new List<Domain>();
            if (searchOtherDomains)
            {
                domains.AddRange(Forest.GetCurrentForest().Domains.Cast<Domain>());
            }
            else
            {
                domains.Add(Domain.GetCurrentDomain());
            }
            foreach (var d in domains)
            {
                try
                {
                    var ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    var filter = new UserPrincipalFull(ad) {TargetAddress = emailQuery};
                    var ps = new PrincipalSearcher(filter);
                    var results = ps.FindAll();
                    if (results.Any())
                    {
                        return false;
                    }
                    filter = new UserPrincipalFull(ad) {ProxyAddresses = new List<string> {emailQuery}};
                    ps = new PrincipalSearcher(filter);
                    results = ps.FindAll();
                    if (results.Any())
                    {
                        return false;
                    }
                }
                catch
                {
                }
            }
            return true;
        }

        public static bool IsEmailUnique(string emailQuery, string dn)
        {
            return IsEmailUnique(emailQuery, dn, true);
        }

        public static bool IsEmailUnique(string emailQuery, string dn, bool searchOtherDomains)
        {
            var domains = new List<Domain>();
            if (searchOtherDomains)
            {
                domains.AddRange(Forest.GetCurrentForest().Domains.Cast<Domain>());
            }
            else
            {
                domains.Add(Domain.GetCurrentDomain());
            }
            foreach (var d in domains)
            {
                try
                {
                    var ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    var filter = new UserPrincipalFull(ad) {TargetAddress = emailQuery};
                    var ps = new PrincipalSearcher(filter);
                    var results = ps.FindAll();
                    if (results.Any())
                    {
                        if (results.Any(result => result.DistinguishedName.ToLowerInvariant() != dn.ToLowerInvariant()))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        filter = new UserPrincipalFull(ad) {ProxyAddresses = new List<string> {emailQuery}};
                        ps = new PrincipalSearcher(filter);
                        results = ps.FindAll();
                        if (results.Any())
                        {
                            foreach (var result in results)
                            {
                                if (result.DistinguishedName.ToLowerInvariant() == dn.ToLowerInvariant())
                                {
                                    continue;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return true;
        }

        public static List<GenericDirectoryObject> AddressSearch(string emailQuery)
        {
            var _results = new List<GenericDirectoryObject>();
            foreach (Domain d in Forest.GetCurrentForest().Domains)
            {
                try
                {
                    var ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    var filter = new GenericDirectoryObject(ad) {TargetAddress = string.Format("smtp:{0}", emailQuery)};
                    var ps = new PrincipalSearcher(filter);
                    var results = ps.FindAll();
                    _results.AddRange(
                        results.Select(result => GenericDirectoryObject.FindByIdentity(ad, result.DistinguishedName)));
                    filter = new GenericDirectoryObject(ad)
                    {
                        ProxyAddresses = new List<string> {string.Format("smtp:{0}", emailQuery)}
                    };
                    ps = new PrincipalSearcher(filter);
                    results = ps.FindAll();
                    _results.AddRange(
                        results.Select(result => GenericDirectoryObject.FindByIdentity(ad, result.DistinguishedName)));
                    filter = new GenericDirectoryObject(ad) {EmailAddress = emailQuery};
                    ps = new PrincipalSearcher(filter);
                    results = ps.FindAll();
                    _results.AddRange(
                        results.Select(result => GenericDirectoryObject.FindByIdentity(ad, result.DistinguishedName)));
                }
                catch
                {
                }
            }
            IEnumerable<GenericDirectoryObject> unsorted = _results;
            unsorted = unsorted.Distinct();
            return unsorted.ToList();
        }

        public static bool IsManagedDomain(string domain)
        {
            domain = domain.ToLowerInvariant();
            return Config.ManagedDomains.Any(d => d.ToLowerInvariant() == domain);
        }
    }

    public class ExchangeConfiguration
    {
        public List<string> ManagedDomains = new List<string>
        {
            "company.com",
        };

        public List<string> RoutingDomains = new List<string>
        {
            "company.com",
            "service.company.com",
        };

        public List<string> ShowInAddressBookDefault = new List<string>
        {
            "CN=All Mail Users(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Recipients(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=Default Global Address List,CN=All Global Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Users,CN=All Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
        };

        public List<string> ShowInAddressBookResources = new List<string>
        {
            "CN=All Mail Users(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Recipients(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=Default Global Address List,CN=All Global Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Users,CN=All Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=Mailboxes(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
        };

        public List<string> ShowInAddressBookRooms = new List<string>
        {
            "CN=All Mail Users(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Recipients(VLV),CN=All System Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=Default Global Address List,CN=All Global Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Users,CN=All Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
            "CN=All Rooms,CN=All Address Lists,CN=Address Lists Container,CN=company,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=company,DC=com",
        };

        public string UpnSuffix = "company.com";
    }
}