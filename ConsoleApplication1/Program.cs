using System;
using System.DirectoryServices.AccountManagement;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = "userid";
            var group = "groupname";

            var result = IsInGroup(user, group);
            Console.WriteLine(result);
        }

        public static bool IsInGroup(string user, string group)
        {
            var activeDirectory = new PrincipalContext(ContextType.Domain, "company.com");
            var adGroup = GroupPrincipal.FindByIdentity(activeDirectory, group);
            var adUser = UserPrincipal.FindByIdentity(activeDirectory, user);

            if (adGroup == null || adUser == null || string.IsNullOrWhiteSpace(adGroup.DistinguishedName) || string.IsNullOrWhiteSpace(adUser.DistinguishedName))
            {
                return false;
            }

            return adUser.IsMemberOf(adGroup);
        }
    }
}
