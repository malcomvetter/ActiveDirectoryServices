using System.Text.RegularExpressions;

namespace System.DirectoryServices.AccountManagement
{
    public class PrincipalUtility
    {
        //
        // Summary:
        //     Takes a string and produces and LDAP safe and friendly variant of the original string.
        //
        // Returns:
        //     An LDAP safe and friendly variant of the original string
        //
        // Exceptions:
        //
        public static string MakeSafeForLdap(string original)
        {
            var s = Regex.Replace(original, "[(]", "");
            s = Regex.Replace(s, "[)]", "");
            s = Regex.Replace(s, "&", "-");
            s = Regex.Replace(s, ",", "");
            s = Regex.Replace(s, "[.]", "");
            s = Regex.Replace(s, " ", "_");
            s = Regex.Replace(s, "__", "_");
            s = Regex.Replace(s, "__", "_");
            return s;
        }

        //
        // Summary:
        //     Takes a string and validates whether it is safe for LDAP
        //
        // Returns:
        //     True if the string is safe for LDAP
        //
        // Exceptions:
        //
        public static bool IsSafeLdapParameter(string queryParameter)
        {
            var safe = new Regex(@"^[a-zA-Z0-9.@_\-\' $&#]*$");
            string[] symbols = {" ", "'", "[.]"};
            var isSafe = true;
            foreach (var s in symbols)
            {
                var reg = s + s + "+";
                var doubleup = new Regex(reg);
                isSafe = (isSafe && !doubleup.IsMatch(queryParameter));
            }
            return (isSafe && safe.IsMatch(queryParameter));
        }

        //
        // Summary:
        //     Takes a string password and validates against a hard-coded password policy
        //
        // Returns:
        //     True if the string password meets a hard-coded password policy
        //
        // Exceptions:
        //
        public static bool PasswordMeetsComplexity(string password)
        {
            var criteria = 0;
            //check lowercase
            var lowercase = new Regex("[a-z]");
            if (lowercase.IsMatch(password))
            {
                criteria++;
            }

            //check uppercase
            var uppercase = new Regex("[A-Z]");
            if (uppercase.IsMatch(password))
            {
                criteria++;
            }

            //check numbers
            var numeric = new Regex("[0-9]");
            if (numeric.IsMatch(password))
            {
                criteria++;
            }

            //check symbols
            var symbolic = new Regex(@"[.,@_\-\'$#!%^Z&*()+=?<>{}\[\]/\\:;|~`]");
            if (symbolic.IsMatch(password))
            {
                criteria++;
            }

            return (criteria >= 3) && (password.Length >= 7);
        }
    }
}