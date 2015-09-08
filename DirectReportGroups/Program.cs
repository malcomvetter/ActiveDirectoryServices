using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;

namespace DirectReportGroups
{
    internal class AllDirRpts
    {
        private static void Main(string[] args)
        {
            do
            {
                try
                {
                    Console.Write("UserID: ");
                    var managerId = Console.ReadLine();
                    AllDirRptsGroups(managerId, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } while (true);
        }

        public static void AllDirRptsGroups(string userId, bool recursive)
        {
            #region Environment

            const string projectName = "ADManager";
            const string dirRptSuffix = "_DirRpts";
            const string allRptSuffix = "_AllRpts";
            const string mgrRptSuffix = "_MgrRpts";
            var employeesLdapPath = "EMPLOYEES,";
            var dirRptLdapPath = "OU=DirectReports,OU=ORGCHART,";
            var allRptLdapPath = "OU=AllReports,OU=ORGCHART,";
            var mgrRptLdapPath = "OU=ManagerReports,OU=ORGCHART,";
            var directReportEmployees = new List<string>();
            var activeDirectory = new PrincipalContext(ContextType.Domain);
            var dirRptsGroup = new GroupPrincipalFull(activeDirectory);
            var allRptsGroup = new GroupPrincipalFull(activeDirectory);
            try
            {
                var admins = GroupPrincipalFull.FindByIdentity(activeDirectory, "Domain Admins");
                var ldapSuffix = admins.RootContainer;
                dirRptLdapPath += ldapSuffix;
                allRptLdapPath += ldapSuffix;
                mgrRptLdapPath += ldapSuffix;
                employeesLdapPath += ldapSuffix;
            }
            catch
            {
            }

            var managerDn = string.Empty;

            #endregion

            try
            {
                #region FetchUserInfo

                var user = UserPrincipalFull.FindByIdentity(activeDirectory, userId);
                Console.WriteLine("Processing {0}", user.DisplayName);
                var dirRptGrpName = PrincipalUtility.MakeSafeForLdap(user.DisplayName + dirRptSuffix);
                var allRptGrpName = PrincipalUtility.MakeSafeForLdap(user.DisplayName + allRptSuffix);
                var mgrRptGrpName = PrincipalUtility.MakeSafeForLdap(user.DisplayName + mgrRptSuffix);
                try
                {
                    managerDn = user.Manager;
                }
                catch
                {
                }
                try
                {
                    Console.WriteLine("Fetching {0}'s Direct Reports", user.DisplayName);
                    directReportEmployees = user.DirectReports;
                }
                catch
                {
                }

                #endregion

                directReportEmployees =
                    directReportEmployees.Where(
                        dn => dn.ToLowerInvariant().Contains(employeesLdapPath.ToLowerInvariant())).ToList();

                if (directReportEmployees.Count > 0)
                {
                    #region ValidateDirRptGroup

                    try
                    {
                        dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                        try
                        {
                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, allRptGrpName);
                        }
                        catch
                        {
                        }
                        if (!string.IsNullOrEmpty(dirRptsGroup.DistinguishedName))
                        {
                            #region AddMgrToDirRpts

                            try
                            {
                                Console.WriteLine("Adding {0} to {1}", user.DisplayName, dirRptsGroup.SamAccountName);
                                dirRptsGroup.AddMember(user);
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("object already exists"))
                                {
                                    Console.WriteLine(ex);
                                }
                            }

                            #endregion

                            #region AddDirRptToMgrAllRpts

                            try
                            {
                                var manager = new UserPrincipalFull(activeDirectory);
                                try
                                {
                                    manager = UserPrincipalFull.FindByIdentity(activeDirectory, managerDn);
                                }
                                catch
                                {
                                }
                                var mgrAllRpts = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                    PrincipalUtility.MakeSafeForLdap(manager.DisplayName + allRptSuffix));
                                try
                                {
                                    Console.WriteLine("Adding {0} to {1}", dirRptGrpName, mgrAllRpts.SamAccountName);
                                    mgrAllRpts.AddMember(dirRptsGroup);
                                }
                                catch (DirectoryServicesCOMException dsce)
                                {
                                    if (!dsce.Message.Contains("object already exists"))
                                    {
                                        Console.WriteLine(dsce);
                                    }
                                }
                                try
                                {
                                    if (!string.IsNullOrEmpty(allRptsGroup.DistinguishedName))
                                    {
                                        try
                                        {
                                            mgrAllRpts.AddMember(allRptsGroup);
                                            Console.WriteLine("Adding {0} to {1}", allRptsGroup.SamAccountName,
                                                mgrAllRpts.SamAccountName);
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrAllRpts.RemoveMember(dirRptsGroup);
                                            Console.WriteLine("Removing {0} to {1}", dirRptsGroup.SamAccountName,
                                                mgrAllRpts.SamAccountName);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            catch (NullReferenceException)
                            {
                            }
                            catch (Exception ex2)
                            {
                                if (!ex2.ToString().Contains("object already exists"))
                                {
                                    Console.WriteLine(ex2);
                                }
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            #region CreateDirRptGroup

                            Console.WriteLine("Creating {0}", dirRptGrpName);
                            var ou = new PrincipalContext(ContextType.Domain, null, dirRptLdapPath);
                            dirRptsGroup = new GroupPrincipalFull(ou, dirRptGrpName);
                            dirRptsGroup.Save();
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.DisplayName = string.Format("{0} - Direct Reporting Employees",
                                    user.DisplayName);
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.Description = string.Format("Employees who directly report to {0}",
                                    user.DisplayName);
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.EmailAddress = string.Format("{0}@customer.com", dirRptGrpName);
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.GidNumber = int.Parse(user.EmployeeNumber);
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.Info = string.Format("Created by {0} on {1}", projectName,
                                    DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                dirRptsGroup.ManagedBy = user.DistinguishedName;
                                dirRptsGroup.Save();
                            }
                            catch
                            {
                            }
                            try
                            {
                                dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                                Console.WriteLine("Adding {0} to {1}", user.DisplayName, dirRptsGroup.SamAccountName);
                                dirRptsGroup.AddMember(user);
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine(ex2);
                            }

                            #region AddMgrToDirRpts

                            try
                            {
                                Console.WriteLine("Adding {0} to {1}", user.DisplayName, dirRptsGroup.SamAccountName);
                                dirRptsGroup.AddMember(user);
                            }
                            catch (Exception ex3)
                            {
                                if (!ex.Message.Contains("object already exists"))
                                {
                                    Console.WriteLine(ex3);
                                }
                            }

                            #endregion

                            #region AddDirRptToMgrAllRpts

                            try
                            {
                                var manager = UserPrincipalFull.FindByIdentity(activeDirectory, managerDn);
                                var mgrAllRpts = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                    PrincipalUtility.MakeSafeForLdap(manager.DisplayName + allRptSuffix));
                                Console.WriteLine("Adding {0} to {1}", dirRptGrpName, mgrAllRpts.SamAccountName);
                                mgrAllRpts.AddMember(dirRptsGroup);
                            }
                            catch (NullReferenceException)
                            {
                            }
                            catch (Exception ex2)
                            {
                                if (!ex2.ToString().Contains("object already exists"))
                                {
                                    Console.WriteLine(ex2);
                                }
                            }

                            #endregion

                            #endregion
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine(ex1);
                        }
                    }

                    #endregion

                    foreach (var dn in directReportEmployees)
                    {
                        try
                        {
                            var employee = UserPrincipalFull.FindByIdentity(activeDirectory, dn);

                            #region AddEmpToDirRptGroup

                            dirRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, dirRptGrpName);
                            Console.WriteLine("Adding {0} to {1}", employee.DisplayName, dirRptsGroup.SamAccountName);
                            try
                            {
                                dirRptsGroup.AddMember(employee);
                            }
                            catch (Exception ex1)
                            {
                                if (!ex1.ToString().Contains("object already exists"))
                                {
                                    Console.WriteLine(ex1);
                                }
                            }

                            #endregion

                            #region MgrRptGroup

                            var type = string.Empty;
                            try
                            {
                                type = employee.EmployeeType.ToLowerInvariant();
                            }
                            catch
                            {
                            }
                            if (type.Contains("manager") || type.Contains("corporate officer"))
                            {
                                var mgrRpts = new GroupPrincipalFull(activeDirectory);
                                try
                                {
                                    mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                    if (!string.IsNullOrEmpty(mgrRpts.DistinguishedName))
                                    {
                                    }
                                }
                                catch (Exception)
                                {
                                    #region CreateMgrRptGroup

                                    try
                                    {
                                        //group not found, so create the group
                                        Console.WriteLine("Creating {0}", mgrRptGrpName);
                                        var ou = new PrincipalContext(ContextType.Domain, null, mgrRptLdapPath);
                                        mgrRpts = new GroupPrincipalFull(ou, mgrRptGrpName);
                                        mgrRpts.Save();
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.DisplayName = string.Format("{0} - All Reporting Managers",
                                                user.DisplayName);
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.Description =
                                                string.Format(
                                                    "Managers who report both directly and indirectly to {0}",
                                                    user.DisplayName);
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.EmailAddress = string.Format("{0}@customer.com", mgrRptGrpName);
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.GidNumber = int.Parse(user.EmployeeNumber);
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.Info = string.Format("Created by {0} on {1}", projectName,
                                                DateTime.Now);
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            mgrRpts.ManagedBy = user.DistinguishedName;
                                            mgrRpts.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            mgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory, mgrRptGrpName);
                                            Console.WriteLine("Adding {0} to {1}", user.DisplayName,
                                                mgrRpts.SamAccountName);
                                            mgrRpts.AddMember(user);
                                        }
                                        catch (Exception ex2)
                                        {
                                            if (!ex2.ToString().Contains("object already exists"))
                                            {
                                                Console.WriteLine(ex2);
                                            }
                                        }
                                        try
                                        {
                                            var manager = UserPrincipalFull.FindByIdentity(activeDirectory, managerDn);
                                            var mgrMgrRpts = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                PrincipalUtility.MakeSafeForLdap(manager.DisplayName + mgrRptSuffix));
                                            mgrMgrRpts.AddMember(mgrRpts);
                                        }
                                        catch (NullReferenceException)
                                        {
                                        }
                                        catch (Exception ex2)
                                        {
                                            if (!ex2.ToString().Contains("object already exists"))
                                            {
                                                Console.WriteLine(ex2);
                                            }
                                        }
                                    }
                                    catch (Exception ex1)
                                    {
                                        Console.WriteLine(ex1);
                                    }

                                    #endregion
                                }
                                try
                                {
                                    Console.WriteLine("Adding {0} to {1}", employee.DisplayName, mgrRpts.SamAccountName);
                                    mgrRpts.AddMember(employee);
                                }
                                catch (Exception ex1)
                                {
                                    if (!ex1.ToString().Contains("object already exists"))
                                    {
                                        Console.WriteLine(ex1);
                                    }
                                }
                            }

                            #endregion

                            #region RecurseSubDirRpts

                            try
                            {
                                var emplHasDirRpts = false;

                                #region FilterSecondAccounts

                                try
                                {
                                    if (
                                        employee.DirectReports.Any(
                                            rdn => rdn.ToLowerInvariant().Contains(employeesLdapPath.ToLowerInvariant())))
                                    {
                                        emplHasDirRpts = true;
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    Console.WriteLine(ex1);
                                }

                                #endregion

                                if (!emplHasDirRpts || !recursive)
                                {
                                    continue;
                                }

                                #region ValidateAllRptGroup

                                try
                                {
                                    allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory, allRptGrpName);
                                    if (!string.IsNullOrEmpty(allRptsGroup.DistinguishedName))
                                    {
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    #region CreateAllRptGroup

                                    try
                                    {
                                        Console.WriteLine("Creating {0}", allRptGrpName);
                                        var ou = new PrincipalContext(ContextType.Domain, null, allRptLdapPath);
                                        allRptsGroup = new GroupPrincipalFull(ou, allRptGrpName);
                                        allRptsGroup.Save();
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.DisplayName = string.Format("{0} - All Reporting Employees",
                                                user.DisplayName);
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.Description =
                                                string.Format(
                                                    "Employees who report both directly and indirectly to {0}",
                                                    user.DisplayName);
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.EmailAddress = string.Format("{0}@customer.com", allRptGrpName);
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.GidNumber = int.Parse(user.EmployeeNumber);
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.Info = string.Format("Created by {0} on {1}", projectName,
                                                DateTime.Now.ToString(CultureInfo.InvariantCulture));
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            allRptsGroup = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                allRptGrpName);
                                            allRptsGroup.ManagedBy = user.DistinguishedName;
                                            allRptsGroup.Save();
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            Console.WriteLine("Adding {0} to {1}", dirRptGrpName, allRptGrpName);
                                            allRptsGroup.AddMember(dirRptsGroup);
                                        }
                                        catch (Exception ex2)
                                        {
                                            Console.WriteLine(ex2);
                                        }

                                        #region AddAllRptToMgrAllRpts

                                        try
                                        {
                                            var manager = UserPrincipalFull.FindByIdentity(activeDirectory, managerDn);
                                            var mgrAllRpts = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                PrincipalUtility.MakeSafeForLdap(manager.DisplayName + allRptSuffix));
                                            Console.WriteLine("Adding {0} to {1}", allRptGrpName,
                                                mgrAllRpts.SamAccountName);
                                            mgrAllRpts.AddMember(allRptsGroup);
                                        }
                                        catch (NullReferenceException)
                                        {
                                        }
                                        catch (Exception ex2)
                                        {
                                            if (!ex2.ToString().Contains("object already exists"))
                                            {
                                                Console.WriteLine(ex2);
                                            }
                                        }

                                        #endregion

                                        #region RemoveDirRptFromMgrAllRpts

                                        try
                                        {
                                            var manager = UserPrincipalFull.FindByIdentity(activeDirectory, managerDn);
                                            var mgrAllRpts = GroupPrincipalFull.FindByIdentity(activeDirectory,
                                                PrincipalUtility.MakeSafeForLdap(manager.DisplayName + allRptSuffix));
                                            Console.WriteLine("Removing {0} from {1}", dirRptGrpName,
                                                mgrAllRpts.SamAccountName);
                                            mgrAllRpts.RemoveMember(dirRptsGroup);
                                        }
                                        catch (NullReferenceException)
                                        {
                                        }
                                        catch (Exception ex2)
                                        {
                                            if (!ex2.ToString().Contains("exist"))
                                            {
                                                Console.WriteLine(ex2);
                                            }
                                        }

                                        #endregion
                                    }
                                    catch (Exception ex1)
                                    {
                                        Console.WriteLine(ex1);
                                    }

                                    #endregion
                                }

                                #endregion

                                Console.WriteLine("{0} has direct reports, processing recursively.",
                                    employee.DisplayName);
                                AllDirRptsGroups(employee.SamAccountName, recursive);
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine(ex2);
                            }

                            #endregion
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine(ex1);
                        }
                    }

                    #region PurgeOldDirRpts

                    foreach (var dn in dirRptsGroup.MemberDNs)
                    {
                        try
                        {
                            var found = false;
                            foreach (var mdn in user.DirectReports)
                            {
                                if (dn.ToLowerInvariant() == mdn.ToLowerInvariant())
                                {
                                    found = true;
                                    break;
                                }
                                if (dn.ToLowerInvariant() != user.DistinguishedName.ToLowerInvariant())
                                {
                                    continue;
                                }
                                found = true;
                                break;
                            }
                            if (found)
                            {
                                continue;
                            }
                            var oldEmpl = UserPrincipalFull.FindByIdentity(activeDirectory, dn);
                            Console.WriteLine("Removing old object {0} from {1}", oldEmpl.SamAccountName,
                                dirRptsGroup.SamAccountName);
                            dirRptsGroup.RemoveMember(oldEmpl);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}