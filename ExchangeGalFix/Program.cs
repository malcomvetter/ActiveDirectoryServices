using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace GalFix
{
    public class GalFixTool
    {
        private static void Main(string[] args)
        {
            var ad = new PrincipalContextFull(ContextType.Domain);
            try
            {
                Console.WriteLine();
                var email = args[0];
                if (email.Split(new[] {'@'}).Count() < 2)
                {
                    try
                    {
                        foreach (Domain d in Forest.GetCurrentForest().Domains)
                        {
                            Console.WriteLine("Searching in Domain: {0}", d.Name);
                            try
                            {
                                ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                                var gdo = GenericDirectoryObject.FindByIdentity(ad, email);
                                if (!string.IsNullOrWhiteSpace(gdo.DistinguishedName))
                                {
                                    Console.WriteLine("Search hit: {0}", gdo.DistinguishedName);
                                    if (string.IsNullOrWhiteSpace(gdo.EmailAddress))
                                    {
                                        ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                                        return;
                                    }
                                    email = gdo.EmailAddress;
                                    Console.WriteLine("\nSearching Active Directory Forest for Email: {0}", email);
                                }
                            }
                            catch
                            {
                            }
                            if (email.Split(new[] {'@'}).Count() >= 2)
                            {
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                IEnumerable<Principal> results = null;
                string manager = null;
                foreach (Domain d in Forest.GetCurrentForest().Domains)
                {
                    ad = new PrincipalContextFull(ContextType.Domain, d.Name);
                    var filter = new GenericDirectoryObject(ad)
                    {
                        EmailAddress = email
                    };
                    var ps = new PrincipalSearcher(filter);
                    results = results == null ? ps.FindAll() : results.Concat(ps.FindAll());
                    filter = new GenericDirectoryObject(ad)
                    {
                        ProxyAddresses = new List<string> {string.Format("smtp:{0}", email)}
                    };
                    ps = new PrincipalSearcher(filter);
                    results = results.Concat(ps.FindAll());
                    filter = new GenericDirectoryObject(ad)
                    {
                        TargetAddress = string.Format("smtp:{0}", email)
                    };
                    ps = new PrincipalSearcher(filter);
                    results = results.Concat(ps.FindAll());
                }
                var matches = new List<GenericDirectoryObject>();
                foreach (var result in results)
                {
                    var gdo = new GenericDirectoryObject(ad);
                    foreach (Domain d in Forest.GetCurrentForest().Domains)
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
                var previousDn = string.Empty;
                var sortedMatches = new List<GenericDirectoryObject>();
                foreach (
                    var gdo in
                        matches.Where(gdo => previousDn.ToLowerInvariant() != gdo.DistinguishedName.ToLowerInvariant()))
                {
                    previousDn = gdo.DistinguishedName;
                    sortedMatches.Add(gdo);
                }
                IEnumerable<GenericDirectoryObject> iMatches = sortedMatches;
                iMatches.Distinct().OrderBy(x => x);
                Console.WriteLine("\nFound {0} results", iMatches.Count());
                foreach (var gdo in iMatches)
                {
                    Console.WriteLine(gdo.DistinguishedName);
                }
                Console.WriteLine();
                foreach (var gdo in iMatches)
                {
                    Console.WriteLine("Match: {0} \n - SamID: {4}\n -Display: {1}\n -GUID: {2}\n -DN: {3}", gdo.Name,
                        gdo.DisplayName, gdo.Guid, gdo.DistinguishedName, gdo.SamAccountName);
                    if (manager == null || gdo.ObjectClass.Contains("contact"))
                    {
                        var n = gdo.DistinguishedName.Split(new[] {','})[0].Substring(3).ToLowerInvariant();
                        var e = email.Split(new[] {'@'})[0].ToLowerInvariant();
                        if (n == e)
                        {
                            Console.Write("Correct? (Y) ");
                            try
                            {
                                if (Console.ReadLine().Substring(0, 1).ToLowerInvariant() == "n")
                                {
                                    ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                                }
                                if (Console.ReadLine().Substring(0, 1).ToLowerInvariant() == "y")
                                {
                                    ExchangeCommon.ExchangeCommon.MailEnable(gdo, false);
                                    manager = gdo.DistinguishedName;
                                    Console.WriteLine();
                                    continue;
                                }
                            }
                            catch
                            {
                                ExchangeCommon.ExchangeCommon.MailEnable(gdo, false);
                                manager = gdo.DistinguishedName;
                                Console.WriteLine();
                                continue;
                            }
                        }
                        else
                        {
                            Console.Write("Correct? (N) ");
                            try
                            {
                                if (Console.ReadLine().Substring(0, 1).ToLowerInvariant() == "y")
                                {
                                    ExchangeCommon.ExchangeCommon.MailEnable(gdo, false);
                                    manager = gdo.DistinguishedName;
                                    Console.WriteLine();
                                    continue;
                                }
                                if (Console.ReadLine().Substring(0, 1).ToLowerInvariant() == "n")
                                {
                                    ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                                }
                            }
                            catch
                            {
                                ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                            }
                        }
                    }
                    else
                    {
                        ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                    }
                    Console.WriteLine();
                }
                if (string.IsNullOrWhiteSpace(manager))
                {
                    return;
                }
                foreach (var gdo in iMatches.Where(gdo => gdo.DistinguishedName != manager))
                {
                    try
                    {
                        var existingMgr = string.Empty;
                        try
                        {
                            existingMgr = gdo.Manager;
                        }
                        catch
                        {
                        }
                        if (!string.IsNullOrWhiteSpace(existingMgr))
                        {
                            continue;
                        }
                        gdo.Manager = manager;
                        gdo.Save();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}