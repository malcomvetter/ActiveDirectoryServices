using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Xml.Serialization;

namespace AdLdsSync
{
    [XmlRoot("AdLdsSync")]
    public class AdLdsSyncConfig
    {
        public ActiveDirectoryConnection ActiveDirectoryConnection = new ActiveDirectoryConnection
        {
            Name = "company.com",
        };

        public List<AdObject> AdContractorGroups = new List<AdObject>();
        public List<string> AdEmployeeOUs = new List<string>();
        public List<string> Attributes = new List<string>();
        public int DaysSinceModified = 3;
        public List<AllReportsRule> DirRptRules = new List<AllReportsRule>();
        public string LdsContractorOu = "OU=Contractors,OU=Roles,DC=Customer,DC=Customer,DC=Com";
        public List<AdObject> LdsEmployeeGroups = new List<AdObject>();
        public string LdsEmployeeOu = "OU=Employees,OU=Roles,DC=Customer,DC=Customer,DC=Com";

        public LightweightDirectoryConnection LightweightDirectoryConnection = new LightweightDirectoryConnection
        {
            ServerFqdn = "customers.company.com:50000",
            BaseDn = "DC=customers,DC=company,DC=com",
        };

        public int MaxRecords = 100;
        public int MaxTries = 3;
        public bool ReportChangesOnly = false;
        public int WorkerThreadCount = 0;

        public AdLdsSyncConfig()
        {
        }

        public AdLdsSyncConfig(bool reset)
        {
            if (!reset)
            {
                return;
            }
            if (AdEmployeeOUs.Count == 0)
            {
                AdEmployeeOUs.Add("OU=Employees,DC=Customer,DC=com");
                AdEmployeeOUs.Add("OU=Admins,DC=Customer,DC=com");
                AdEmployeeOUs.Add("OU=TestAccounts,DC=Customer,DC=com");
                AdEmployeeOUs.Add("OU=Executives,DC=Customer,DC=com");
                DirRptRules.Add(new AllReportsRule("RetailVP", "RetailDivision"));
                DirRptRules.Add(new AllReportsRule("SalesVP", "SalesDivision"));
            }
            if (AdContractorGroups.Count == 0)
            {
                AdContractorGroups.Add(new AdObject(string.Empty, "LDSProxyAccess", new Sid()));
            }
            if (LdsEmployeeGroups.Count == 0)
            {
                LdsEmployeeGroups.Add(new AdObject("CN=Employees,OU=Groups,DC=customer,DC=Customer,DC=com", "Employees",
                    new Sid()));
            }
            if (Attributes.Count == 0)
            {
                Attributes.Add("c");
                Attributes.Add("co");
                Attributes.Add("comment");
                Attributes.Add("company");
                Attributes.Add("countryCode");
                Attributes.Add("department");
                Attributes.Add("departmentNumber");
                Attributes.Add("description");
                Attributes.Add("displayName");
                Attributes.Add("division");
                Attributes.Add("employeeID");
                Attributes.Add("employeeNumber");
                Attributes.Add("employeeType");
                Attributes.Add("facsimileTelephoneNumber");
                Attributes.Add("givenName");
                Attributes.Add("initials");
                Attributes.Add("l");
                Attributes.Add("mail");
                Attributes.Add("middleName");
                Attributes.Add("mobile");
                Attributes.Add("o");
                Attributes.Add("pager");
                Attributes.Add("postalAddress");
                Attributes.Add("postalCode");
                Attributes.Add("postOfficeBox");
                Attributes.Add("sn");
                Attributes.Add("st");
                Attributes.Add("street");
                Attributes.Add("streetAddress");
                Attributes.Add("telephoneNumber");
                Attributes.Add("title");
            }
        }
    }

    public class AllReportsRule
    {
        public string Group;
        public string Mgr;

        public AllReportsRule()
        {
        }

        public AllReportsRule(string mgr, string group)
        {
            Mgr = mgr;
            Group = group;
        }
    }

    [XmlRoot("DirectoryConnection")]
    [XmlInclude(typeof (ActiveDirectoryConnection))]
    public class ActiveDirectoryConnection
    {
        private string name;

        private string serverFqdn;

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = new PrincipalContextFull(ContextType.Domain).ContextDns;
                }
                return name;
            }
            set { name = value; }
        }

        [XmlIgnore]
        public string ServerFqdn
        {
            get
            {
                if (string.IsNullOrWhiteSpace(serverFqdn))
                {
                    serverFqdn = new PrincipalContext(ContextType.Domain).ConnectedServer;
                }
                return serverFqdn;
            }
            set { serverFqdn = value; }
        }
    }

    [XmlRoot("DirectoryConnection")]
    [XmlInclude(typeof (LightweightDirectoryConnection))]
    public class LightweightDirectoryConnection
    {
        public string BaseDn;
        public string ServerFqdn;
    }
}