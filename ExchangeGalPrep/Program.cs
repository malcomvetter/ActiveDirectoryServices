using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace ExchangeGalPrep
{
    internal class GalPrep
    {
        private static void Main(string[] args)
        {
            try
            {
                var max = int.MaxValue;
                try
                {
                    max = int.Parse(args[0]);
                }
                catch
                {
                }
                var domain = string.Empty;
                try
                {
                    domain = args[1];
                }
                catch
                {
                }
                foreach (Domain d in Forest.GetCurrentForest().Domains)
                {
                    var directory = new PrincipalContextFull(ContextType.Domain, d.Name);
                    if (!string.IsNullOrWhiteSpace(domain))
                    {
                        directory = new PrincipalContextFull(ContextType.Domain, domain);
                    }
                    Console.WriteLine("Fetching {0} objects from {1}", max, directory.ConnectedServer);
                    foreach (var gdo in directory.GetAllChildObjects(max))
                    {
                        try
                        {
                            Console.WriteLine("Found {0} ...", gdo.Name);
                            var email = string.Empty;
                            try
                            {
                                email = gdo.EmailAddress;
                            }
                            catch
                            {
                            }
                            if (string.IsNullOrWhiteSpace(email))
                            {
                                ExchangeCommon.ExchangeCommon.MailDisable(gdo);
                            }
                            else
                            {
                                ExchangeCommon.ExchangeCommon.MailEnable(gdo, false);
                            }
                        }
                        catch (Exception _ex)
                        {
                            Console.WriteLine("Exception Processing User: {0}", _ex);
                        }
                        finally
                        {
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: {0}", ex);
            }
        }
    }
}