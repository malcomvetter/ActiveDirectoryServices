using System;
using System.DirectoryServices.AccountManagement;
using System.IO;

namespace ExchangeImportResources
{
    internal class ImportResources
    {
        private const string TargetDomain = "company.com";
        private const string TargetOu = "OU=Imported_Resources,OU=Exchange,DC=company,DC=com";

        private static void Main(string[] args)
        {
            try
            {
                foreach (var line in File.ReadLines(args[0]))
                {
                    try
                    {
                        line.Trim();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        Console.WriteLine("Reading: {0}", line);
                        string name = string.Empty,
                            email = string.Empty,
                            display = string.Empty,
                            first = string.Empty,
                            last = string.Empty,
                            description = string.Empty;
                        var s = line.Split(new[] {','});
                        try
                        {
                            name = s[0];
                        }
                        catch
                        {
                        }
                        try
                        {
                            email = s[1];
                        }
                        catch
                        {
                        }
                        try
                        {
                            display = s[0];
                        }
                        catch
                        {
                        }
                        try
                        {
                            first = s[0];
                        }
                        catch
                        {
                        }
                        try
                        {
                            last = s[0];
                        }
                        catch
                        {
                        }
                        try
                        {
                            description = s[0];
                        }
                        catch
                        {
                        }
                        Console.WriteLine("Name: {0}", name);
                        Console.WriteLine("Email: {0}", email);
                        Console.WriteLine("Display: {0}", display);
                        Console.WriteLine("First: {0}", first);
                        Console.WriteLine("Last: {0}", last);
                        Console.WriteLine("Description: {0}", description);
                        var activeDirectory = new PrincipalContextFull(ContextType.Domain, TargetDomain, TargetOu);
                        try
                        {
                            ContactPrincipal.CreateContact(activeDirectory, name);
                        }
                        catch
                        {
                        }
                        var contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        Console.WriteLine("Processing {0} ...", contact.Name);
                        contact.EmailAddress = email;
                        try
                        {
                            contact.Save();
                        }
                        catch
                        {
                            contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        }
                        contact.DisplayName = display;
                        try
                        {
                            contact.Save();
                        }
                        catch
                        {
                            contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        }
                        contact.Description = description;
                        try
                        {
                            contact.Save();
                        }
                        catch
                        {
                            contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        }
                        contact.GivenName = first;
                        try
                        {
                            contact.Save();
                        }
                        catch
                        {
                            contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        }
                        contact.Surname = last;
                        try
                        {
                            contact.Save();
                        }
                        catch
                        {
                            contact = ContactPrincipal.FindByIdentity(activeDirectory, name);
                        }
                        try
                        {
                            ExchangeCommon.ExchangeCommon.MailEnable(
                                GenericDirectoryObject.FindByIdentity(activeDirectory, contact.DistinguishedName), false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        try
                        {
                            ExchangeCommon.ExchangeCommon.AddResourceAttributes(
                                GenericDirectoryObject.FindByIdentity(activeDirectory, contact.DistinguishedName));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    //Console.Write("Enter to continue");
                    //Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}