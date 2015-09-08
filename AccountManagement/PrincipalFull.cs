using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using S.DS.AM.Extensions;

namespace System.DirectoryServices.AccountManagement
{
    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("top")]
    public class PrincipalFull : Principal, IComparable
    {
        #region ExtendedAttributes

        // Summary:
        //     Gets or sets the MobilePhone for this Principal
        //
        // Returns:
        //     The MobilePhone of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("mobile")]
        public string MobilePhone
        {
            get
            {
                if (ExtensionGet("mobile").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("mobile")[0];
            }
            set { ExtensionSet("mobile", value); }
        }

        // Summary:
        //     Gets or sets the mail for this Principal
        //
        // Returns:
        //     The mail of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("mail")]
        public string EmailAddress
        {
            get
            {
                if (ExtensionGet("mail").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("mail")[0];
            }
            set { ExtensionSet("mail", value); }
        }

        // Summary:
        //     Gets or sets the surname for this Principal
        //
        // Returns:
        //     The surname of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("sn")]
        public string Surname
        {
            get
            {
                if (ExtensionGet("sn").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("sn")[0];
            }
            set { ExtensionSet("sn", value); }
        }

        // Summary:
        //     Gets or sets the objectClass for this Principal
        //
        // Returns:
        //     The objectClass of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("objectClass")]
        public string ObjectClass
        {
            get
            {
                if (ExtensionGet("objectClass").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("objectClass")[0];
            }
            set { ExtensionSet("objectClass", value); }
        }

        // Summary:
        //     Gets or sets the SidHistory for this Principal
        //
        // Returns:
        //     The SidHistory of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("SidHistory")]
        public List<SecurityIdentifier> SidHistory
        {
            get { return ExtensionGet("SidHistory").Select(o => new SecurityIdentifier((byte[]) o, 0)).ToList(); }
        }

        // Summary:
        //     Gets or sets the ipPhone for this Principal
        //
        // Returns:
        //     The ipPhone of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("ipPhone")]
        public string IpPhone
        {
            get
            {
                if (ExtensionGet("ipPhone").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("ipPhone")[0];
            }
            set { ExtensionSet("ipPhone", value); }
        }

        // Summary:
        //     Gets or sets the string value of the DistinguishedName of the Manager for this Principal
        //
        // Returns:
        //     The string value of the DistinguishedName of the Manager of the Principal.
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
        //     Gets a List<string> of the distinguished names of direct reports for this Principal
        //
        // Returns:
        //     The a List<string> of the distinguished names of direct reports for the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("directReports")]
        public List<string> DirectReports
        {
            get { return ExtensionGet("directReports").Cast<string>().ToList(); }
        }

        // Summary:
        //     Gets a List<string> of distinguished names of all reports (recursive direct reports) for this Principal
        //
        // Returns:
        //     The List<string of distinguished names of all reports (recursive direct reports) for the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public List<string> AllReports
        {
            get
            {
                var reports = new List<string>();
                foreach (var o in ExtensionGet("directReports"))
                {
                    reports.Add((string) o);
                    try
                    {
                        var upf = FindByIdentity(Context, (string) o);
                        reports.AddRange(upf.AllReports);
                    }
                    catch
                    {
                    }
                }
                return reports;
            }
        }

        // Summary:
        //     Gets or sets the employeeNumber for this Principal
        //
        // Returns:
        //     The employeeNumber of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("employeeNumber")]
        public string EmployeeNumber
        {
            get
            {
                if (ExtensionGet("employeeNumber").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("employeeNumber")[0];
            }
            set { ExtensionSet("employeeNumber", value); }
        }

        // Summary:
        //     Gets or sets the title for this Principal
        //
        // Returns:
        //     The title of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if (ExtensionGet("title").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("title")[0];
            }
            set { ExtensionSet("title", value); }
        }

        // Summary:
        //     Gets or sets the Department for this Principal
        //
        // Returns:
        //     The Department of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if (ExtensionGet("department").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("department")[0];
            }
            set { ExtensionSet("department", value); }
        }

        // Summary:
        //     Gets or sets the initials for this Principal
        //
        // Returns:
        //     The initials of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("initials")]
        public string Initials
        {
            get
            {
                if (ExtensionGet("initials").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("initials")[0];
            }
            set { ExtensionSet("initials", value); }
        }

        // Summary:
        //     Gets or sets the telephone number for this Principal
        //
        // Returns:
        //     The telephone number of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("telephoneNumber")]
        public string TelephoneNumber
        {
            get
            {
                if (ExtensionGet("telephoneNumber").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("telephoneNumber")[0];
            }
            set { ExtensionSet("telephoneNumber", value); }
        }

        // Summary:
        //     Gets or sets the street for this Principal
        //
        // Returns:
        //     The street of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("street")]
        public string Street
        {
            get
            {
                if (ExtensionGet("street").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("street")[0];
            }
            set { ExtensionSet("street", value); }
        }

        // Summary:
        //     Gets or sets the street address for this Principal
        //
        // Returns:
        //     The street address of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("streetAddress")]
        public string StreetAddress
        {
            get
            {
                if (ExtensionGet("streetAddress").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("streetAddress")[0];
            }
            set { ExtensionSet("streetAddress", value); }
        }

        // Summary:
        //     Gets or sets the city for this Principal
        //
        // Returns:
        //     The city of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("l")]
        public string City
        {
            get
            {
                if (ExtensionGet("l").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("l")[0];
            }
            set { ExtensionSet("l", value); }
        }

        // Summary:
        //     Gets or sets the state for this Principal
        //
        // Returns:
        //     The state of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("st")]
        public string State
        {
            get
            {
                if (ExtensionGet("st").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("st")[0];
            }
            set { ExtensionSet("st", value); }
        }

        // Summary:
        //     Gets or sets the postal or zip code for this Principal
        //
        // Returns:
        //     The postal or zip code of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("postalCode")]
        public string PostalCode
        {
            get
            {
                if (ExtensionGet("postalCode").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("postalCode")[0];
            }
            set { ExtensionSet("postalCode", value); }
        }

        // Summary:
        //     Gets or sets the country for this Principal
        //
        // Returns:
        //     The country of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("co")]
        public string Country
        {
            get
            {
                if (ExtensionGet("co").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("co")[0];
            }
            set { ExtensionSet("co", value); }
        }

        // Summary:
        //     Gets or sets the country code for this Principal
        //
        // Returns:
        //     The country code of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("c")]
        public string CountryCode
        {
            get
            {
                if (ExtensionGet("c").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("c")[0];
            }
            set { ExtensionSet("c", value); }
        }

        // Summary:
        //     Gets or sets the fax number for this Principal
        //
        // Returns:
        //     The fax number of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("facsimileTelephoneNumber")]
        public string FaxNumber
        {
            get
            {
                if (ExtensionGet("facsimileTelephoneNumber").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("facsimileTelephoneNumber")[0];
            }
            set { ExtensionSet("facsimileTelephoneNumber", value); }
        }

        // Summary:
        //     Gets or sets the pager number for this Principal
        //
        // Returns:
        //     The pager number of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("pager")]
        public string PagerNumber
        {
            get
            {
                if (ExtensionGet("pager").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("pager")[0];
            }
            set { ExtensionSet("pager", value); }
        }

        // Summary:
        //     Gets or sets the employee type for this Principal
        //
        // Returns:
        //     The employee type of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("employeeType")]
        public string EmployeeType
        {
            get
            {
                if (ExtensionGet("employeeType").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("employeeType")[0];
            }
            set { ExtensionSet("employeeType", value); }
        }

        // Summary:
        //     Gets or sets the company for this Principal
        //
        // Returns:
        //     The company of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("company")]
        public string Company
        {
            get
            {
                if (ExtensionGet("company").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("company")[0];
            }
            set { ExtensionSet("company", value); }
        }

        // Summary:
        //     Gets or sets the login script path for this Principal
        //
        // Returns:
        //     The login script path of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("scriptPath")]
        public string LoginScript
        {
            get
            {
                if (ExtensionGet("scriptPath").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("scriptPath")[0];
            }
            set { ExtensionSet("scriptPath", value); }
        }

        // Summary:
        //     Gets or sets the Department number for this Principal
        //
        // Returns:
        //     The Department number of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("departmentNumber")]
        public int DepartmentNumber
        {
            get { return (int) ExtensionGet("departmentNumber")[0]; }
            set { ExtensionSet("departmentNumber", value); }
        }

        // Summary:
        //     Gets or sets the info or notes field for this Principal
        //
        // Returns:
        //     The info or notes field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("info")]
        public string Info
        {
            get
            {
                if (ExtensionGet("info").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("info")[0];
            }
            set { ExtensionSet("info", value); }
        }

        // Summary:
        //     Gets or sets the administrative description for this Principal
        //
        // Returns:
        //     The administrative description of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("adminDescription")]
        public string AdminDescription
        {
            get
            {
                if (ExtensionGet("adminDescription").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("adminDescription")[0];
            }
            set { ExtensionSet("adminDescription", value); }
        }

        // Summary:
        //     Gets or sets the administrative display name for this Principal
        //
        // Returns:
        //     The administrative display name of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("adminDisplayName")]
        public string AdminDisplayName
        {
            get
            {
                if (ExtensionGet("adminDisplayName").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("adminDisplayName")[0];
            }
            set { ExtensionSet("adminDisplayName", value); }
        }

        // Summary:
        //     Gets or sets the UID for this Principal
        //
        // Returns:
        //     The UID of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("uid")]
        public string Uid
        {
            get
            {
                if (ExtensionGet("uid").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("uid")[0];
            }
            set { ExtensionSet("uid", value); }
        }

        // Summary:
        //     Gets or sets the UID Number for this Principal
        //
        // Returns:
        //     The UID Number of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("uidNumber")]
        public int UidNumber
        {
            get { return (int) ExtensionGet("uidNumber")[0]; }
            set { ExtensionSet("uidNumber", value); }
        }

        // Summary:
        //     Gets or sets the GID Number for this Principal
        //
        // Returns:
        //     The GID Number of the Principal.
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
        //     Gets or sets the Apple UID for this Principal
        //
        // Returns:
        //     The Apple UID of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("AppleUID")]
        public string AppleUid
        {
            get
            {
                if (ExtensionGet("AppleUID").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("AppleUID")[0];
            }
            set { ExtensionSet("AppleUID", value); }
        }

        // Summary:
        //     Gets or sets the other mailbox field for this Principal
        //
        // Returns:
        //     The other mailbox field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("otherMailbox")]
        public List<string> OtherMailbox
        {
            get
            {
                var otherMailboxes = new List<string>();
                var i = 0;
                do
                {
                    try
                    {
                        otherMailboxes.Add((string) ExtensionGet("otherMailbox")[i]);
                        i++;
                    }
                    catch
                    {
                        i = -1;
                    }
                } while (i >= 0);
                return otherMailboxes;
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
                ExtensionSet("otherMailbox", o);
            }
        }

        // Summary:
        //     Gets the created date for this Principal
        //
        // Returns:
        //     The created date of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("WhenCreated")]
        public DateTime WhenCreated
        {
            get
            {
                if (ExtensionGet("WhenCreated").Length < 1)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.ParseExact(ExtensionGet("whenCreated")[0].ToString(), "M/d/yyyy h:mm:ss tt", null);
            }
        }

        // Summary:
        //     Gets the modified date for this Principal
        //
        // Returns:
        //     The modified date of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("WhenChanged")]
        public DateTime WhenChanged
        {
            get
            {
                if (ExtensionGet("WhenChanged").Length < 1)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.ParseExact(ExtensionGet("WhenChanged")[0].ToString(), "M/d/yyyy h:mm:ss tt", null);
            }
        }

        // Summary:
        //     Gets or sets the unixHomeDirectory for this Principal
        //
        // Returns:
        //     The unixHomeDirectory of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("unixHomeDirectory")]
        public string UnixHomeDirectory
        {
            get
            {
                if (ExtensionGet("unixHomeDirectory").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("unixHomeDirectory")[0];
            }
            set { ExtensionSet("unixHomeDirectory", value); }
        }

        // Summary:
        //     Gets or sets the loginShell for this Principal
        //
        // Returns:
        //     The loginShell of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("loginShell")]
        public string LoginShell
        {
            get
            {
                if (ExtensionGet("loginShell").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("loginShell")[0];
            }
            set { ExtensionSet("loginShell", value); }
        }

        // Summary:
        //     Gets or sets the gecos for this Principal
        //
        // Returns:
        //     The gecos of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("gecos")]
        public string Gecos
        {
            get
            {
                if (ExtensionGet("gecos").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("gecos")[0];
            }
            set { ExtensionSet("gecos", value); }
        }

        // Summary:
        //     Gets or sets the targetAddress for this Principal
        //
        // Returns:
        //     The targetAddress of the Principal.
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
        //     Gets or sets the mailNickname for this Principal
        //
        // Returns:
        //     The mailNickname of the Principal.
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
        //     Gets or sets the Proxy Addresses for this Principal
        //
        // Returns:
        //     The Proxy Addresses of the Principal.
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
        //     Gets or sets the showInAddressBook for this Principal
        //
        // Returns:
        //     The showInAddressBook of the Principal.
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
        //     Gets or sets the legacyExchangeDN for this Principal
        //
        // Returns:
        //     The legacyExchangeDN of the Principal.
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
        //     Gets or sets the msExchResourceMetaData for this Principal
        //
        // Returns:
        //     The msExchResourceMetaData of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msExchResourceMetaData")]
        public List<string> MsExchResourceMetaData
        {
            get
            {
                var msExchResourceMetaData = new List<string>();
                var i = 0;
                do
                {
                    try
                    {
                        msExchResourceMetaData.Add((string) ExtensionGet("msExchResourceMetaData")[i]);
                        i++;
                    }
                    catch
                    {
                        i = -1;
                    }
                } while (i >= 0);
                return msExchResourceMetaData;
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
                ExtensionSet("msExchResourceMetaData", o);
            }
        }

        // Summary:
        //     Gets or sets the msExchResourceDisplay for this Principal
        //
        // Returns:
        //     The msExchResourceDisplay of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msExchResourceDisplay")]
        public string MsExchResourceDisplay
        {
            get
            {
                if (ExtensionGet("msExchResourceDisplay").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("msExchResourceDisplay")[0];
            }
            set { ExtensionSet("msExchResourceDisplay", value); }
        }

        // Summary:
        //     Gets or sets the msExchResourceSearchProperties for this Principal
        //
        // Returns:
        //     The msExchResourceSearchProperties of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msExchResourceSearchProperties")]
        public List<string> MsExchResourceSearchProperties
        {
            get
            {
                var msExchResourceSearchProperties = new List<string>();
                var i = 0;
                do
                {
                    try
                    {
                        msExchResourceSearchProperties.Add((string) ExtensionGet("msExchResourceSearchProperties")[i]);
                        i++;
                    }
                    catch
                    {
                        i = -1;
                    }
                } while (i >= 0);
                return msExchResourceSearchProperties;
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
                ExtensionSet("msExchResourceSearchProperties", o);
            }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-PrimaryUserAddress field for this Principal
        //
        // Returns:
        //     The msRTCSIP-PrimaryUserAddress field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-PrimaryUserAddress")]
        public string MsRtcSipPrimaryUserAddress
        {
            get
            {
                if (ExtensionGet("msRTCSIP-PrimaryUserAddress").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("msRTCSIP-PrimaryUserAddress")[0];
            }
            set { ExtensionSet("msRTCSIP-PrimaryUserAddress", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-OptionFlags for this Principal
        //
        // Returns:
        //     The msRTCSIP-OptionFlags of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-OptionFlags")]
        public MsRtcSipOptionFlagOptions MsRtcSipOptionFlags
        {
            get { return (MsRtcSipOptionFlagOptions) ExtensionGet("msRTCSIP-OptionFlags")[0]; }
            set { ExtensionSet("msRTCSIP-OptionFlags", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-ArchivingEnabled for this Principal
        //
        // Returns:
        //     The msRTCSIP-ArchivingEnabled of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-ArchivingEnabled")]
        public MsRtcSipArchivingEnabledOptions MsRtcSipArchivingEnabled
        {
            get { return (MsRtcSipArchivingEnabledOptions) ExtensionGet("msRTCSIP-ArchivingEnabled")[0]; }
            set { ExtensionSet("msRTCSIP-ArchivingEnabled", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-FederationEnabled for this Principal
        //
        // Returns:
        //     The msRTCSIP-FederationEnabled of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-FederationEnabled")]
        public bool MsRtcSipFederationEnabled
        {
            get { return (bool) ExtensionGet("msRTCSIP-FederationEnabled")[0]; }
            set { ExtensionSet("msRTCSIP-FederationEnabled", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-InternetAccessEnabled for this Principal
        //
        // Returns:
        //     The msRTCSIP-InternetAccessEnabled of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-InternetAccessEnabled")]
        public bool MsRtcSipInternetAccessEnabled
        {
            get { return (bool) ExtensionGet("msRTCSIP-InternetAccessEnabled")[0]; }
            set { ExtensionSet("msRTCSIP-InternetAccessEnabled", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-UserEnabled field for this Principal
        //
        // Returns:
        //     The msRTCSIP-UserEnabled field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-UserEnabled")]
        public bool MsRtcSipUserEnabled
        {
            get { return (bool) ExtensionGet("msRTCSIP-UserEnabled")[0]; }
            set { ExtensionSet("msRTCSIP-UserEnabled", value); }
        }

        // Summary:
        //     Gets or sets the msRTCSIP-PrimaryHomeServer field for this Principal
        //
        // Returns:
        //     The msRTCSIP-PrimaryHomeServer field of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("msRTCSIP-PrimaryHomeServer")]
        public string MsRtcSipPrimaryHomeServer
        {
            get
            {
                if (ExtensionGet("msRTCSIP-PrimaryHomeServer").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("msRTCSIP-PrimaryHomeServer")[0];
            }
            set { ExtensionSet("msRTCSIP-PrimaryHomeServer", value); }
        }

        // Summary:
        //     Gets or sets the userParameters for this Principal
        //
        // Returns:
        //     The userParameters of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("userParameters")]
        public string UserParameters
        {
            get
            {
                if (ExtensionGet("userParameters").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("userParameters")[0];
            }
            set { ExtensionSet("userParameters", value); }
        }

        // Summary:
        //     Gets or sets the wwwHomePage for this Principal
        //
        // Returns:
        //     The wwwHomePage of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("wwwHomePage")]
        public string WwwHomePage
        {
            get
            {
                if (ExtensionGet("wwwHomePage").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("wwwHomePage")[0];
            }
            set { ExtensionSet("wwwHomePage", value); }
        }

        // Summary:
        //     Gets or sets the RecoveryPasswordHash for this Principal
        //
        // Returns:
        //     The Recovery Password Hash of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public string RecoveryPasswordHash
        {
            get
            {
                if (ExtensionGet("RecoveryPasswordHash").Length < 1)
                {
                    return null;
                }
                return (string) ExtensionGet("RecoveryPasswordHash")[0];
            }
            set { ExtensionSet("RecoveryPasswordHash", value); }
        }

        // Summary:
        //     Gets the Relative ID portion of the SID for this Principal
        //
        // Returns:
        //     The Relative ID portion of the SID of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public int Rid
        {
            get
            {
                var s = Sid.ToString().Split('-');
                return int.Parse(s[s.Count()]);
            }
        }

        // Summary:
        //     Gets or sets the adminSdHolderLock value for this Principal
        //
        // Returns:
        //     The adminSdHolderLock value of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        [DirectoryProperty("adminCount")]
        public AdminSdHolderLockValue AdminSdHolderLock
        {
            get
            {
                try
                {
                    return (AdminSdHolderLockValue) ExtensionGet("adminCount")[0];
                }
                catch (NullReferenceException)
                {
                    return AdminSdHolderLockValue.None;
                }
            }
            set { ExtensionSet("adminCount", value); }
        }

        #endregion

        #region ExtendedLdapFunctions

        // Summary:
        //     Gets extended attributes for this Principal
        //
        // Returns:
        //     The object[] from the specified extended attribute of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.

        // Summary:
        //     Gets the RootContainer for this Principal
        //
        // Returns:
        //     The RootContainer of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
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
        //     Gets or sets the Container for this Principal
        //
        // Returns:
        //     The Container of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
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
        //     The DNS name of the PrincpalContext of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
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
            return ExtensionGet(attribute);
        }

        // Summary:
        //     Gets extended attributes for this Principal
        //
        // Returns:
        //     The object[] from the specified extended attribute of the Principal.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The underlying store does not support this property.
        public void SetAttribute(string attribute, object value)
        {
            ExtensionSet(attribute, value);
        }

        // Summary:
        //     Gets extended attributes for this Principal
        //
        // Returns:
        //     The object[] from the specified extended attribute of the Principal.
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
        //     The string value of the new distinguished name of the renamed Principal.
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
        //     Performs an LDAP move on this Principal
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
            return;
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
        public new static PrincipalFull FindByIdentity(PrincipalContext context,
            string identityValue)
        {
            return (PrincipalFull) FindByIdentityWithType(context,
                typeof (PrincipalFull),
                identityValue);
        }

        // Implement the overloaded search method FindByIdentity. 
        public new static PrincipalFull FindByIdentity(PrincipalContext context,
            IdentityType identityType,
            string identityValue)
        {
            return (PrincipalFull) FindByIdentityWithType(context,
                typeof (PrincipalFull),
                identityType,
                identityValue);
        }

        public class PrincipalSearchFilter : AdvancedFilters
        {
            public PrincipalSearchFilter(PrincipalFull p) : base(p)
            {
            }
        }

        #region IComparable

        //IComparable Overrides:
        public int CompareTo(object obj)
        {
            var full = obj as PrincipalFull;
            if (full != null)
            {
                var p = full;
                return Sid.CompareTo(p.Sid);
            }
            return 1;
        }

        public override bool Equals(Object obj)
        {
            var full = obj as PrincipalFull;
            if (full != null)
            {
                var p = full;
                return (p.Sid == Sid);
            }
            return false;
        }

        #endregion

        #endregion
    }
}