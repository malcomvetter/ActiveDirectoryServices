using System;
using System.DirectoryServices.AccountManagement;

namespace PasswordSnoozeButton
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Configure your Directory Connection here:
            const string directoryName = "domain.com";
            const string directoryContainer = "DC=Domain,DC=Com";

            var directory = new PrincipalContextFull(ContextType.Domain, directoryName, directoryContainer);
            Console.WriteLine("Connected to LDAP://{0}/{1}\n...\n", directoryName, directoryContainer);

            var i = 0;
            foreach (var user in directory.GetChildUserObjects())
            {
                i++;
                try
                {
                    Console.WriteLine("{0}. Found {1}", i, user.DistinguishedName);
                    user.ExpirePasswordNow();
                    user.RefreshExpiredPassword();
                    Console.WriteLine("{0}. PwdLastSet is now refreshed for {1}", i, user.DistinguishedName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}