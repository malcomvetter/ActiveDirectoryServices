using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Xml.Serialization;

namespace AdLdsSync
{
    public class AdLdsSync
    {
        private const string ProjectName = "ALDSSync";
        private static AdLdsSyncConfig _config = new AdLdsSyncConfig();
        public static List<AdObject> AdEmployees = new List<AdObject>();
        public static List<AdObject> AdContractors = new List<AdObject>();
        public static List<AdObject> LdsUsers = new List<AdObject>();
        public static List<AdObject> Errors = new List<AdObject>();
        private static readonly DateTime StartTime = DateTime.Now;
        private static FileStream _fslog;
        private static StreamWriter _logOut;

        private static void Main(string[] args)
        {
            try
            {
                try
                {
                    if (!args.Any())
                    {
                        Console.Clear();
                    }
                }
                catch
                {
                }
                Console.WriteLine();
                Console.WriteLine();

                #region Logging

                var logFile = ProjectName + String.Format("{0:yyyyMMdd}", DateTime.Today) + ".log";
                Console.WriteLine(logFile);
                try
                {
                    _fslog = new FileStream(logFile, FileMode.Create, FileAccess.Write);
                    _logOut = new StreamWriter(_fslog);
                }
                catch (IOException exc)
                {
                    Console.WriteLine(exc.Message + "Cannot open log file.");
                    return;
                }
                var then = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));
                Console.WriteLine(ProjectName + String.Format("{0:yyyyMMdd}", then) + ".log");
                if (File.Exists(ProjectName + String.Format("{0:yyyyMMdd}", then) + ".log"))
                {
                    try
                    {
                        File.Delete(ProjectName + String.Format("{0:yyyyMMdd}", then) + ".log");
                        Console.WriteLine("Deleted old log: " + ProjectName + String.Format("{0:yyyyMMdd}", then) +
                                          ".log");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not delete old log file: " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Could not find: " + ProjectName + String.Format("{0:yyyyMMdd}", then) + ".log");
                }
                Console.WriteLine();

                #endregion

                #region GetConfig

                _config = new AdLdsSyncConfig(true);
                var message = string.Empty;
                var serializer = new XmlSerializer(typeof (AdLdsSyncConfig));
                if (File.Exists(ProjectName + "Config.xml"))
                {
                    message += ("Reading in Configuration from " + ProjectName + "Config.xml ... ");
                    TextReader reader = new StreamReader(ProjectName + "Config.xml");
                    _config = (AdLdsSyncConfig) serializer.Deserialize(reader);
                    reader.Close();
                    message += ("Complete.") + "\r\n";
                    message += ("AD: LDAP://" + _config.ActiveDirectoryConnection.Name) + "\r\n";
                    message += ("LDS: LDAP://" + _config.LightweightDirectoryConnection.ServerFqdn) + "\r\n";
                    message += ("Worker Threads: " + _config.WorkerThreadCount) + "\r\n";
                    message += ("Maximum Records: " + _config.MaxRecords) + "\r\n";
                    _config.AdEmployeeOUs = SortUnique(_config.AdEmployeeOUs);
                    message = _config.AdEmployeeOUs.Aggregate(message,
                        (current, OU) => current + ("AD Employee OU: " + OU + "\r\n"));
                    _config.LdsEmployeeGroups = SortUnique(_config.LdsEmployeeGroups);
                    message = _config.LdsEmployeeGroups.Aggregate(message,
                        (current, @group) => current + ("LDS Employee Group: " + @group + "\r\n"));
                    _config.AdContractorGroups = SortUnique(_config.AdContractorGroups);
                    message = _config.AdContractorGroups.Aggregate(message,
                        (current, @group) => current + ("AD Contractor Group: " + @group + "\r\n"));
                    message += "LDS Employee OU: " + _config.LdsEmployeeOu + "\r\n";
                    message += "LDS Contractor OU: " + _config.LdsContractorOu + "\r\n";
                    _config.Attributes = SortUnique(_config.Attributes);
                    message = _config.Attributes.Aggregate(message,
                        (current, attribute) => current + ("UserProxyAttribute: " + attribute + "\r\n"));
                    lock (_fslog)
                    {
                        _logOut.Write(message);
                        Console.WriteLine(message);
                        _logOut.Flush();
                        message = string.Empty;
                    }
                }
                else
                {
                    try
                    {
                        message += (ProjectName + "Config.xml does not exist - Creating Sample Configuration") + "\r\n";
                        _logOut.Write(message);
                        Console.WriteLine(message);
                        _logOut.Flush();
                        message = string.Empty;
                        _config = new AdLdsSyncConfig(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    message += ("Writing Sample Configuration") + "\r\n";
                    TextWriter writer = new StreamWriter(ProjectName + "Config.xml");
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    serializer = new XmlSerializer(typeof (AdLdsSyncConfig));
                    serializer.Serialize(writer, _config, ns);
                    writer.Close();
                    message += ("Wrote Sample Configuration to " + ProjectName + "Config.xml") + "\r\n";
                    _logOut.Write(message);
                    Console.WriteLine(message);
                    _logOut.Flush();
                    message = string.Empty;
                    return;
                }

                #endregion

                #region TestConfig

                try
                {
                    if (!string.IsNullOrEmpty(_config.LightweightDirectoryConnection.ServerFqdn))
                    {
                        message = string.Empty;
                        message += "Successfully created reference to " +
                                   _config.LightweightDirectoryConnection.ServerFqdn + "\r\n";
                        var lightweightDirectory = new PrincipalContext(ContextType.ApplicationDirectory,
                            string.Format("{0}", _config.LightweightDirectoryConnection.ServerFqdn),
                            _config.LightweightDirectoryConnection.BaseDn);
                        var example = GroupPrincipalFull.FindByIdentity(lightweightDirectory, "Administrators");
                        try
                        {
                            if (!string.IsNullOrEmpty(example.DistinguishedName))
                            {
                                message += "- Found example AD-LDS object: " + example.DistinguishedName;
                            }
                        }
                        catch
                        {
                            message += "- Could not find example AD-LDS object: Administrators";
                        }
                        lock (_fslog)
                        {
                            Console.WriteLine(message);
                            _logOut.WriteLine(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (_fslog)
                    {
                        message = ("Error creating AD-LDS reference: " + e) + "\r\n";
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                    }
                    return;
                }

                try
                {
                    if (!string.IsNullOrEmpty(_config.ActiveDirectoryConnection.ServerFqdn))
                    {
                        message = string.Empty;
                        message += "Successfully created reference to " + _config.ActiveDirectoryConnection.ServerFqdn +
                                   "\r\n";
                        var activeDirectory = new PrincipalContext(ContextType.Domain);
                        var example = GroupPrincipalFull.FindByIdentity(activeDirectory, "Administrators");
                        try
                        {
                            if (!string.IsNullOrEmpty(example.DistinguishedName))
                            {
                                message += "- Found example Active Directory object: " + example.DistinguishedName;
                            }
                        }
                        catch
                        {
                            message += "- Could not find example Active Directory object: Administrators";
                        }
                        lock (_fslog)
                        {
                            Console.WriteLine(message);
                            _logOut.WriteLine(message);
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (_fslog)
                    {
                        message = ("Error creating Active Directory reference: " + e) + "\r\n";
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                    }
                    return;
                }

                #endregion

                #region FetchUsers

                try
                {
                    message = "\r\n";
                    message += "Starting Sync from AD to LDS" + "\r\n" + "\r\n";
                    lock (_fslog)
                    {
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                    }
                    var threads = new Thread[2];
                    try
                    {
                        var starter0 = new ParameterizedThreadStart(ThreadedFetchAd);
                        threads[0] = new Thread(starter0);
                        try
                        {
                            threads[0].Priority = ThreadPriority.Highest;
                        }
                        catch
                        {
                        }
                        threads[0].Start(0);
                        var starter1 = new ParameterizedThreadStart(ThreadedFetchLds);
                        threads[1] = new Thread(starter1);
                        try
                        {
                            threads[1].Priority = ThreadPriority.Highest;
                        }
                        catch
                        {
                        }
                        threads[1].Start(0);
                    }
                    catch
                    {
                    }
                    foreach (var thread in threads)
                    {
                        thread.IsBackground = true;
                        thread.Join();
                    }
                }
                catch
                {
                }

                #endregion

                #region StartWorkerThreads

                try
                {
                    AdEmployees.Reverse();
                    if ((_config.MaxRecords > 10000) || (_config.MaxRecords < 0))
                    {
                        lock (_fslog)
                        {
                            message = ("Sleeping for 5 seconds to let directory servers recoup from the large query") +
                                      "\r\n";
                            Console.WriteLine(message);
                            _logOut.Write(message);
                            _fslog.Flush();
                        }
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                    }
                    if (_config.WorkerThreadCount == 0)
                    {
                        lock (_fslog)
                        {
                            message = "Running in current thread, not starting new worker threads" + "\r\n";
                            Console.WriteLine(message);
                            _logOut.WriteLine(message);
                            _fslog.Flush();
                        }
                        var threadObj = new ThreadObject
                        {
                            ThreadNumber = 0,
                            Objects = AdEmployees
                        };
                        threadObj.Objects.AddRange(AdContractors);
                        threadObj.OuDn = _config.LdsEmployeeOu;
                        threadObj.Start = StartTime;
                        ThreadedLdsPush(threadObj);
                        threadObj = new ThreadObject
                        {
                            ThreadNumber = 0,
                            Objects = LdsUsers,
                            OuDn = string.Empty,
                            Start = StartTime
                        };
                        ThreadedAdPull(threadObj);
                        lock (_fslog)
                        {
                            message = ("Mapping Direct Reports to LDS Groups by Rule") + "\r\n";
                            Console.WriteLine(message);
                            _logOut.Write(message);
                            _fslog.Flush();
                        }
                        MapDirectReports();
                        lock (_fslog)
                        {
                            _fslog.Flush();
                            message = "Monolithic thread complete." + "\r\n";
                            Console.WriteLine(message);
                            _logOut.WriteLine(message);
                            _fslog.Flush();
                        }
                    }
                    else
                    {
                        try
                        {
                            var ldsPushThreads = new Thread[_config.WorkerThreadCount];
                            for (var i = 0; i < ldsPushThreads.Count(); i++)
                            {
                                try
                                {
                                    message = ("Starting LDSPush thread " + i) + "\r\n";
                                    var count = (int) Math.Ceiling((double) AdEmployees.Count/_config.WorkerThreadCount);
                                    var start = i*count;
                                    var finish = (start + count) - 1;
                                    if (finish >= AdEmployees.Count)
                                    {
                                        finish = AdEmployees.Count - 1;
                                        count = finish - start;
                                    }
                                    message += (" processing " + count + " from " + start + " to " + finish) + "\r\n";
                                    lock (_fslog)
                                    {
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                    var threadObj = new ThreadObject
                                    {
                                        ThreadNumber = i,
                                        Objects = AdEmployees.GetRange(start, count),
                                        OuDn = _config.LdsEmployeeOu,
                                        Start = StartTime
                                    };
                                    var starter = new ParameterizedThreadStart(ThreadedLdsPush);
                                    ldsPushThreads[i] = new Thread(starter);
                                    ldsPushThreads[i].Start(threadObj);
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    lock (_fslog)
                                    {
                                        _fslog.Flush();
                                    }
                                }
                            }
                            var adPullThreads = new Thread[_config.WorkerThreadCount];
                            for (var i = 0; i < adPullThreads.Count(); i++)
                            {
                                try
                                {
                                    message = ("Starting ADPull thread " + i) + "\r\n";
                                    var count = (int) Math.Ceiling((double) LdsUsers.Count/_config.WorkerThreadCount);
                                    var start = i*count;
                                    var finish = (start + count) - 1;
                                    if (finish >= LdsUsers.Count)
                                    {
                                        finish = LdsUsers.Count - 1;
                                        count = finish - start;
                                    }
                                    message += (" processing " + count + " from " + start + " to " + finish) + "\r\n";
                                    lock (_fslog)
                                    {
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                    var threadObj = new ThreadObject
                                    {
                                        ThreadNumber = i,
                                        Objects = LdsUsers.GetRange(start, count),
                                        OuDn = string.Empty,
                                        Start = StartTime
                                    };
                                    ParameterizedThreadStart starter = ThreadedAdPull;
                                    adPullThreads[i] = new Thread(starter);
                                    adPullThreads[i].Start(threadObj);
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    lock (_fslog)
                                    {
                                        _fslog.Flush();
                                    }
                                }
                            }
                            Thread contractorThread = null;
                            try
                            {
                                lock (_fslog)
                                {
                                    message = ("Starting Contractor thread ") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                    _fslog.Flush();
                                }
                                var threadObj = new ThreadObject
                                {
                                    ThreadNumber = -1,
                                    Objects = AdContractors,
                                    OuDn = _config.LdsContractorOu,
                                    Start = StartTime
                                };
                                ParameterizedThreadStart starter = ThreadedLdsPush;
                                contractorThread = new Thread(starter);
                                contractorThread.Start(threadObj);
                            }
                            catch
                            {
                            }
                            finally
                            {
                                lock (_fslog)
                                {
                                    _fslog.Flush();
                                }
                            }
                            Thread mapReportsThread = null;
                            try
                            {
                                lock (_fslog)
                                {
                                    message = ("Starting MapReports thread ") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                    _fslog.Flush();
                                }
                                var threadObj = new ThreadObject
                                {
                                    ThreadNumber = -1,
                                    Objects = AdContractors,
                                    OuDn = _config.LdsContractorOu,
                                    Start = StartTime
                                };
                                ThreadStart starter = MapDirectReports;
                                mapReportsThread = new Thread(starter);
                                mapReportsThread.Start(threadObj);
                            }
                            catch
                            {
                            }
                            finally
                            {
                                lock (_fslog)
                                {
                                    _fslog.Flush();
                                }
                            }

                            for (var i = 0; i < ldsPushThreads.Count(); i++)
                            {
                                try
                                {
                                    lock (_fslog)
                                    {
                                        message = ("Waiting on LDSPush thread " + i + " to finish ... ") + "\r\n";
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                    ldsPushThreads[i].IsBackground = true;
                                    ldsPushThreads[i].Join();
                                    lock (_fslog)
                                    {
                                        message = ("LDSPush Thread " + i + " is complete.") + "\r\n";
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    lock (_fslog)
                                    {
                                        _fslog.Flush();
                                    }
                                }
                            }
                            for (var i = 0; i < adPullThreads.Count(); i++)
                            {
                                try
                                {
                                    lock (_fslog)
                                    {
                                        message = ("Waiting on ADPull thread " + i + " to finish ... ") + "\r\n";
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                    ldsPushThreads[i].IsBackground = true;
                                    ldsPushThreads[i].Join();
                                    message = ("ADPull Thread " + i + " is complete.") + "\r\n";
                                    lock (_fslog)
                                    {
                                        Console.WriteLine(message);
                                        _logOut.WriteLine(message);
                                        _fslog.Flush();
                                    }
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    lock (_fslog)
                                    {
                                        _fslog.Flush();
                                    }
                                }
                            }
                            try
                            {
                                lock (_fslog)
                                {
                                    message = ("Waiting on Contractor thread to finish ... ") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                    _fslog.Flush();
                                }
                                contractorThread.IsBackground = true;
                                contractorThread.Join();
                                lock (_fslog)
                                {
                                    message = ("Contractor Thread is complete.") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                lock (_fslog)
                                {
                                    _fslog.Flush();
                                }
                            }
                            try
                            {
                                lock (_fslog)
                                {
                                    message = ("Waiting on MapReports thread to finish ... ") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                    _fslog.Flush();
                                }
                                mapReportsThread.IsBackground = true;
                                mapReportsThread.Join();
                                lock (_fslog)
                                {
                                    message = ("MapReports Thread is complete.") + "\r\n";
                                    Console.WriteLine(message);
                                    _logOut.WriteLine(message);
                                    _fslog.Flush();
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                lock (_fslog)
                                {
                                    _fslog.Flush();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            lock (_fslog)
                            {
                                Console.WriteLine(e);
                                _logOut.WriteLine(e);
                            }
                        }
                        finally
                        {
                            lock (_fslog)
                            {
                                _fslog.Flush();
                            }
                        }
                    }
                    Errors = SortUnique(Errors);
                    var msg = (string.Format("The following {0} users resulted in errors: ", Errors.Count)) + "\r\n";
                    msg = Errors.Aggregate(msg, (current, o) => current + (o.Dn + " (" + o.Sid + ")" + "\r\n"));
                    msg += ("Total Runtime: " + (DateTime.Now.Subtract(StartTime).Hours) + " hours, " +
                            (DateTime.Now.Subtract(StartTime).Minutes) + " minutes, and " +
                            (DateTime.Now.Subtract(StartTime).Seconds) + " seconds.") + "\r\n";
                    lock (_fslog)
                    {
                        Console.WriteLine(msg);
                        _logOut.WriteLine(msg);
                        _fslog.Flush();
                        _logOut.WriteLine();
                        _logOut.WriteLine();
                        _fslog.Flush();
                        _logOut.WriteLine();
                        _logOut.WriteLine();
                        _fslog.Flush();
                    }
                }
                catch (Exception e)
                {
                    lock (_fslog)
                    {
                        Console.WriteLine("Error: " + e);
                        _logOut.WriteLine("Error: " + e);
                    }
                }
                finally
                {
                    lock (_fslog)
                    {
                        _logOut.WriteLine();
                        _logOut.WriteLine();
                        _fslog.Flush();
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                lock (_fslog)
                {
                    Console.WriteLine(e);
                    _logOut.WriteLine(e);
                }
            }
            finally
            {
                lock (_fslog)
                {
                    _logOut.WriteLine();
                    _logOut.WriteLine();
                    _logOut.WriteLine();
                    _fslog.Flush();
                }
            }
        }

        private static void ThreadedLdsPush(object o)
        {
            lock (o)
            {
                var message = string.Empty;
                try
                {
                    List<string> attributes;
                    var maxTries = 5;
                    lock (_config)
                    {
                        attributes = _config.Attributes;
                        maxTries = _config.MaxTries;
                    }
                    var to = (ThreadObject) o;
                    var threadNumber = to.ThreadNumber;
                    lock (_fslog)
                    {
                        message += "LDSPush Thread " + threadNumber + " started ... " + "\r\n";
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }

                    #region DirectoryContexts

                    var ad = new PrincipalContextFull(ContextType.Domain);
                    var lds = new PrincipalContextFull(ContextType.ApplicationDirectory,
                        _config.LightweightDirectoryConnection.ServerFqdn, _config.LightweightDirectoryConnection.BaseDn);

                    #endregion

                    for (var i = 0; i < to.Objects.Count; i++)
                    {
                        message =
                            string.Format("LDSPush Thread {0}: #{1} of {2} ({3}) DN:{4}", threadNumber, i,
                                to.Objects.Count, DateTime.Now.ToLongTimeString(), to.Objects[i].Dn) + "\r\n";
                        try
                        {
                            try
                            {
                                var runningTime = DateTime.Now.Subtract(to.Start);
                                var avgTime = new TimeSpan(0, 0, 0, 0, (int) runningTime.TotalMilliseconds/i);
                                var percentComplete = i/(float) to.Objects.Count;
                                //                            message += string.Format("{0} / {1} = {2} avg={3}", i, _to.objects.Count, percentComplete, avgTime) + "\r\n";
                                var estimatedRemaining = new TimeSpan(0, 0, 0, 0,
                                    (int) Math.Round((float) runningTime.TotalMilliseconds/percentComplete));
                                message += String.Format("Running time: {0}", runningTime) + "\r\n";
                                message +=
                                    String.Format("{0}% complete - estimated remaining time: {1}",
                                        Math.Round(percentComplete*100, 1), estimatedRemaining) + "\r\n";
                                //                            message += String.Format("{0}% complete - estimated remaining time: {1}", Math.Round(percentComplete, 3), estimatedRemaining) + "\r\n";
                            }
                            catch (Exception)
                            {
                            }
                            var exists = false;
                            //message += "Trying to process: " + _to.objects[i].DN + "\r\n";                            
                            var upf = UserPrincipalFull.FindByIdentity(ad, to.Objects[i].Dn);
                            var proxy = new UserProxyFullPrincipal(lds);
                            if (!string.IsNullOrEmpty(upf.DistinguishedName))
                            {
                                proxy = UserProxyFullPrincipal.FindByIdentity(lds, upf.Sid.ToString());
                            }
                            try
                            {
                                if (!string.IsNullOrEmpty(proxy.DistinguishedName))
                                {
                                    message +=
                                        string.Format("Found proxy object by SID:{0} {1}", proxy.Sid,
                                            proxy.DistinguishedName) + "\r\n";
                                    if (proxy.Name.ToLowerInvariant() == upf.SamAccountName.ToLowerInvariant())
                                    {
                                        exists = true;
                                        message +=
                                            string.Format("SID ({0}) and UserID ({1}) match.", proxy.Sid, proxy.Name) +
                                            "\r\n";
                                        if (
                                            !proxy.DistinguishedName.ToLowerInvariant()
                                                .Contains(to.OuDn.ToLowerInvariant()))
                                        {
                                            try
                                            {
                                                proxy.Move(to.OuDn);
                                                proxy = UserProxyFullPrincipal.FindByIdentity(lds, proxy.Sid.ToString());
                                                message += string.Format("Moved {0} to {1}", proxy.Name,
                                                    proxy.DistinguishedName);
                                            }
                                            catch (Exception e)
                                            {
                                                message += string.Format("Could not move {0} to {1} : {2}", proxy.Name,
                                                    to.OuDn, e);
                                                try
                                                {
                                                    proxy.Delete();
                                                    message +=
                                                        string.Format(
                                                            "Deleted {0} to recreate in the correct location.",
                                                            proxy.Name);
                                                }
                                                catch (Exception e1)
                                                {
                                                    message += string.Format("Could not delete {0} Exception: {1}",
                                                        proxy.Name, e1);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //SID is correct, UserID is not, so delete and recreate
                                        message +=
                                            string.Format("SID Match ({0}) with mismatched UserID: {1} != {2}",
                                                proxy.Sid, proxy.Name, upf.SamAccountName) + "\r\n";
                                        try
                                        {
                                            proxy.Delete();
                                            message +=
                                                string.Format("Mismatched UserID ({0}) deleted", proxy.DistinguishedName) +
                                                "\r\n";
                                            Console.WriteLine("DELETED: {0}", proxy.DistinguishedName);
                                        }
                                        catch (Exception Ex)
                                        {
                                            message +=
                                                string.Format(
                                                    "Exception attempting to delete mismatched user proxy ({0}): {1}",
                                                    proxy.DistinguishedName, Ex) + "\r\n";
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                message += string.Format("Could not find SID={0}", to.Objects[i].Sid) + "\r\n";
                                exists = false;
                                try
                                {
                                    proxy = UserProxyFullPrincipal.FindByIdentity(lds, to.Objects[i].Sid.ToString());
                                    if (!string.IsNullOrEmpty(proxy.DistinguishedName))
                                    {
                                        message +=
                                            String.Format("Found a SID/CN MisMatch {0} ({1})", proxy.DistinguishedName,
                                                proxy.Sid) + "\r\n";
                                        try
                                        {
                                            proxy.Sid = new SecurityIdentifier(to.Objects[i].Sid.SecurityIdentifier);
                                            proxy.Save();
                                            message += string.Format("Updated the SID/CN mismatch on {0} to {1}",
                                                proxy.DistinguishedName, proxy.Sid);
                                            Console.WriteLine(message);
                                            exists = true;
                                        }
                                        catch (Exception e)
                                        {
                                            message +=
                                                string.Format(
                                                    "Could not correct the SID for {0} from {1} to {2}, Exception: {3}",
                                                    proxy.DistinguishedName, proxy.Sid, to.Objects[i].Sid, e.Message);
                                            lock (Errors)
                                            {
                                                Errors.Add(new AdObject(proxy.DistinguishedName, proxy.Name,
                                                    new Sid(proxy.Sid)));
                                                Errors.Add(to.Objects[i]);
                                            }
                                            Console.WriteLine(message);
                                            _logOut.WriteLine(message);
                                            _fslog.Flush();
                                            message = string.Empty;
                                            continue;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            if (!exists)
                            {
                                lock (to.OuDn)
                                {
                                    message +=
                                        string.Format("Attempting userProxy OU: {0} CN: {1}", to.OuDn, to.Objects[i].Cn) +
                                        "\r\n";
                                    var errorCount = 0;

                                    do
                                    {
                                        try
                                        {
                                            UserProxyFullPrincipal.CreateProxy(
                                                _config.LightweightDirectoryConnection.ServerFqdn, to.OuDn,
                                                upf.SamAccountName, upf.Sid);
                                            proxy = UserProxyFullPrincipal.FindByIdentity(lds, upf.SamAccountName);
                                            message += "Successfully created userProxyFull: " + proxy.DistinguishedName +
                                                       "\r\n";
                                            exists = true;
                                            errorCount = maxTries + 1;
                                        }
                                        catch (Exception e)
                                        {
                                            if (errorCount < (maxTries - 1))
                                            {
                                                var sleep = 3*errorCount;
                                                    //sleep longer and longer, give the directory servers a chance to breathe
                                                lock (_fslog)
                                                {
                                                    message += "Try #" + errorCount + " failed. Sleeping for " + sleep +
                                                               " seconds." + "\r\n";
                                                    Console.WriteLine(message);
                                                    _logOut.Write(message);
                                                    _fslog.Flush();
                                                }
                                                Thread.Sleep(new TimeSpan(0, 0, sleep));
                                            }
                                            else
                                            {
                                                lock (_fslog)
                                                {
                                                    message += "Error creating userProxy for " + to.Objects[i].Cn + ": " +
                                                               e + "\r\n";
                                                    Console.WriteLine(message);
                                                    _logOut.Write(message);
                                                    _fslog.Flush();
                                                }
                                                lock (Errors)
                                                {
                                                    Errors.Add(to.Objects[i]);
                                                }
                                            }
                                            errorCount++;
                                        }
                                    } while (errorCount < maxTries);
                                }
                            }
                            if (exists)
                            {
                                //Now Ensure correct OU, Set Attributes and Group Memberships
                                foreach (var group in _config.LdsEmployeeGroups)
                                {
                                    try
                                    {
                                        var gpf = GroupPrincipalFull.FindByIdentity(lds, group.Cn);
                                        gpf.AddMember(proxy);
                                        message += (string.Format("Added {0} to {1}", to.Objects[i].Cn, group)) + "\r\n";
                                    }
                                    catch (Exception e1)
                                    {
                                        if (
                                            !e1.Message.ToLowerInvariant()
                                                .Contains(
                                                    "The specified directory service attribute or value already exists."
                                                        .ToLowerInvariant()))
                                        {
                                            message +=
                                                (string.Format("Problem adding {0} as member of {1}: {2}", proxy.Name,
                                                    group.Cn, e1)) + "\r\n";
                                        }
                                        else
                                        {
                                            message +=
                                                (string.Format("{0} is already a member of {1}", proxy.Name, group.Cn)) +
                                                "\r\n";
                                        }
                                    }
                                }
                                var daysSinceMod = 2;
                                try
                                {
                                    daysSinceMod = DateTime.Now.Subtract(upf.WhenChanged).Days;
                                }
                                catch
                                {
                                }
                                if (daysSinceMod < _config.DaysSinceModified)
                                {
                                    message +=
                                        string.Format("Updating attributes of {0} since {1} was modified recently",
                                            proxy.DistinguishedName, upf.DistinguishedName) + "\r\n";
                                    foreach (var attribute in attributes)
                                    {
                                        var value = string.Empty;
                                        if (string.IsNullOrEmpty(attribute))
                                        {
                                            continue;
                                        }
                                        try
                                        {
                                            value = upf.GetAttribute(attribute)[0].ToString();
                                            //message += string.Format("Current value in AD of {0} is {1}", attribute, value) + "\r\n";
                                        }
                                        catch (Exception e)
                                        {
                                            //message += string.Format("Could not get value for {0}", attribute) + "\r\n";
                                        }
                                        try
                                        {
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                continue;
                                            }
                                            var ldsValue = string.Empty;
                                            try
                                            {
                                                ldsValue = proxy.GetAttribute(attribute)[0].ToString();
                                                //message += string.Format("Current value in LDS of {0} is {1}", attribute, value) + "\r\n";
                                            }
                                            catch
                                            {
                                            }
                                            if (ldsValue.ToLowerInvariant() == value.ToLowerInvariant())
                                            {
                                                continue;
                                            }
                                            proxy.SetAttribute(attribute, new object[] {value});
                                            proxy.Save();
                                            message += string.Format("Updated {0} to {1}", attribute, value) + "\r\n";
                                        }
                                        catch (Exception e)
                                        {
                                            message +=
                                                string.Format("Problem setting {0} to {1} : {2}", attribute, value, e) +
                                                "\r\n";
                                        }
                                    }
                                }
                            }
                            lock (_fslog)
                            {
                                Console.WriteLine(message);
                                _logOut.WriteLine(message);
                                _fslog.Flush();
                                message = string.Empty;
                            }
                        }
                        catch (Exception e1)
                        {
                            lock (_fslog)
                            {
                                message += string.Format("Error in thread {0}: {1}", to.ThreadNumber, e1);
                                Console.WriteLine(message);
                                _logOut.WriteLine(message);
                                message = string.Empty;
                                _fslog.Flush();
                            }
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (_fslog)
                    {
                        message += "General Exception: " + e;
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }
                }
            }
        }

        private static void ThreadedAdPull(object o)
        {
            lock (o)
            {
                var message = string.Empty;
                try
                {
                    var maxTries = 5;
                    lock (_config)
                    {
                        maxTries = _config.MaxTries;
                    }
                    var threadObj = (ThreadObject) o;
                    var threadNumber = threadObj.ThreadNumber;
                    lock (_fslog)
                    {
                        message += "ADPull Thread " + threadNumber + " started ...\r\n";
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }

                    #region DirectoryContexts

                    var ad = new PrincipalContextFull(ContextType.Domain);
                    var lds = new PrincipalContextFull(ContextType.ApplicationDirectory,
                        _config.LightweightDirectoryConnection.ServerFqdn, _config.LightweightDirectoryConnection.BaseDn);

                    #endregion

                    for (var i = 0; i < threadObj.Objects.Count; i++)
                    {
                        message = string.Format("ADPull Thread {0}: #{1} of {2} ({3}) DN:{4}\r\n", threadNumber, i,
                            threadObj.Objects.Count, DateTime.Now.ToLongTimeString(), threadObj.Objects[i].Dn);
                        try
                        {
                            try
                            {
                                var runningTime = DateTime.Now.Subtract(threadObj.Start);
                                var avgTime = new TimeSpan(0, 0, 0, 0, (int) runningTime.TotalMilliseconds/i);
                                var percentComplete = i/(float) threadObj.Objects.Count;
                                var estimatedRemaining = new TimeSpan(0, 0, 0, 0,
                                    (int) Math.Round((float) runningTime.TotalMilliseconds/percentComplete));
                                message += String.Format("Running time: {0}", runningTime) + "\r\n";
                                message +=
                                    String.Format("{0}% complete - estimated remaining time: {1}",
                                        Math.Round(percentComplete*100, 1), estimatedRemaining) + "\r\n";
                            }
                            catch
                            {
                            }
                            var proxy = UserProxyFullPrincipal.FindByIdentity(lds, threadObj.Objects[i].Sid.ToString());
                            var user = new UserPrincipalFull(ad);
                            if (!string.IsNullOrWhiteSpace(proxy.DistinguishedName))
                            {
                                message += string.Format("Found proxy object by SID:{0} {1}\r\n", proxy.Sid,
                                    proxy.DistinguishedName);
                                try
                                {
                                    user = UserPrincipalFull.FindByIdentity(ad, proxy.Sid.ToString());
                                    if (string.IsNullOrWhiteSpace(user.DistinguishedName))
                                    {
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    message +=
                                        string.Format(
                                            "Cannot locate AD user that matches proxy object SID ({0}): {1}\r\n",
                                            proxy.Sid, proxy.DistinguishedName);
                                    proxy.Delete();
                                    message += string.Format("Deleted orphan proxy object.\r\n");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            message +=
                                string.Format("Thread {0}: #{1} of {2} ({3}) DN:{4}  EXCEPTION: {5}", threadNumber, i,
                                    threadObj.Objects.Count, DateTime.Now.ToLongTimeString(), threadObj.Objects[i].Dn,
                                    ex) + "\r\n";
                        }
                        finally
                        {
                            lock (_fslog)
                            {
                                Console.WriteLine(message);
                                _logOut.WriteLine(message);
                                _fslog.Flush();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (_fslog)
                    {
                        message += "General Exception: " + e;
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }
                }
            }
        }

        private static void ThreadedFetchAd(object o)
        {
            var message = string.Empty;
            try
            {
                var max = -1;
                var OUs = new List<string>();
                lock (_config)
                {
                    max = _config.MaxRecords;
                    OUs = _config.AdEmployeeOUs;
                }
                lock (_fslog)
                {
                    message = ("Fetching users (" + max + ") in AD ... ") + "\r\n";
                    Console.WriteLine(message);
                    _logOut.WriteLine(message);
                    _fslog.Flush();
                }

                #region DirectoryContexts

                var ad = new PrincipalContextFull(ContextType.Domain);

                #endregion

                foreach (var ou in OUs)
                {
                    var _ou = new PrincipalContextFull(ContextType.Domain, ad.ContextDns, ou);
                    ad.GetChildUserObjects(_config.MaxRecords);
                    var results = _ou.GetChildUserObjects(_config.MaxRecords);
                    if (results == null || results.Count <= 0)
                    {
                        continue;
                    }
                    lock (_fslog)
                    {
                        message = ("### AD Search (" + ou + ") - Found " + results.Count + " results. ###");
                        Console.WriteLine(message);
                        _logOut.Write(message);
                        _fslog.Flush();
                    }
                    var counter = 0;
                    foreach (var upf in results)
                    {
                        try
                        {
                            if (max > 0 && counter > max)
                            {
                                break;
                            }
                            counter++;
                            var ado = new AdObject
                            {
                                Cn = upf.Name,
                                Dn = upf.DistinguishedName,
                                Sid = new Sid(upf.Sid)
                            };
                            lock (AdEmployees)
                            {
                                AdEmployees.Add(ado);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                lock (AdEmployees)
                {
                    AdEmployees.Sort();
                    AdEmployees = SortUnique(AdEmployees);
                    AdEmployees.Reverse();
                    lock (_fslog)
                    {
                        message = ("Retrieved " + AdEmployees.Count + " AD employees.") + "\r\n";
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }
                }
                foreach (var group in _config.AdContractorGroups)
                {
                    try
                    {
                        var gpf = GroupPrincipalFull.FindByIdentity(ad, group.Cn);
                        var members = gpf.GetMemberDNs(true);
                        foreach (var member in members)
                        {
                            try
                            {
                                var upf = UserPrincipalFull.FindByIdentity(ad, member);
                                var ado = new AdObject
                                {
                                    Cn = upf.Name,
                                    Dn = upf.DistinguishedName,
                                    Sid = new Sid(upf.Sid)
                                };
                                lock (AdContractors)
                                {
                                    AdContractors.Add(ado);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        lock (_fslog)
                        {
                            message = "AD Search Exception retrieving members from " + group + " " + e + "\r\n";
                            Console.WriteLine(message);
                            _logOut.Write(message);
                            _fslog.Flush();
                        }
                    }
                }
                lock (AdContractors)
                {
                    AdContractors.Sort();
                    AdContractors = SortUnique(AdContractors);
                    AdContractors.Reverse();
                    message = ("Retrieved " + AdContractors.Count + " AD contractors.") + "\r\n";
                }
                lock (_fslog)
                {
                    Console.WriteLine(message);
                    _logOut.WriteLine(message);
                    _fslog.Flush();
                }
            }
            catch (Exception e)
            {
                lock (_fslog)
                {
                    Console.WriteLine(e);
                    message += e;
                    _logOut.WriteLine(message);
                    _fslog.Flush();
                }
            }
            finally
            {
                lock (_fslog)
                {
                    _fslog.Flush();
                }
            }
        }

        private static void ThreadedFetchLds(object o)
        {
            var max = -1;
            lock (_config)
            {
                max = _config.MaxRecords;
            }
            var message = string.Empty;
            lock (_fslog)
            {
                message = ("Fetching user proxies (" + max + ") in LDS ... ") + "\r\n";
                Console.WriteLine(message);
                _logOut.WriteLine(message);
                _fslog.Flush();
            }

            #region DirectoryContexts

            var LDS = new PrincipalContextFull(ContextType.ApplicationDirectory,
                _config.LightweightDirectoryConnection.ServerFqdn, _config.LightweightDirectoryConnection.BaseDn);

            #endregion

            var results = LDS.GetChildUserProxyFullObjects(_config.MaxRecords);
            if (results == null || results.Count <= 0)
            {
                return;
            }
            lock (_fslog)
            {
                message = ("### LDS Search - Found " + results.Count + " results. ###");
                Console.WriteLine(message);
                _logOut.Write(message);
                _fslog.Flush();
            }
            var counter = 0;
            foreach (var proxy in results.TakeWhile(proxy => max <= 0 || counter <= max))
            {
                try
                {
                    var ado = new AdObject
                    {
                        Cn = proxy.Name,
                        Dn = proxy.DistinguishedName,
                        Sid = new Sid(proxy.Sid)
                    };
                    lock (LdsUsers)
                    {
                        LdsUsers.Add(ado);
                    }
                    message = " - LDS Search Found: " + ado;
                }
                catch (Exception e)
                {
                    message = " LDS Search EXCEPTION for: " + proxy.DistinguishedName + " " + e;
                }
                finally
                {
                    lock (_fslog)
                    {
                        Console.WriteLine(message);
                        _logOut.WriteLine(message);
                        _fslog.Flush();
                    }
                }
            }
        }

        private static void MapDirectReports()
        {
            foreach (var rule in _config.DirRptRules)
            {
                var message = string.Empty;
                message = ("Processing new DirectReportRule") + "\r\n";
                message += string.Format("Manager={0}, Group={1}", rule.Mgr, rule.Group);
                lock (_fslog)
                {
                    Console.WriteLine(message);
                    _logOut.Write(message);
                    _fslog.Flush();
                }

                #region DirectoryContexts

                var ad = new PrincipalContext(ContextType.Domain);
                var lds = new PrincipalContext(ContextType.ApplicationDirectory,
                    _config.LightweightDirectoryConnection.ServerFqdn, _config.LightweightDirectoryConnection.BaseDn);

                #endregion

                var manager = UserPrincipalFull.FindByIdentity(ad, rule.Mgr);
                var directReports = manager.AllReports;
                var gpf = GroupPrincipalFull.FindByIdentity(lds, rule.Group);
                foreach (var udn in directReports)
                {
                    try
                    {
                        var upf = UserPrincipalFull.FindByIdentity(ad, udn);
                        message += (string.Format("AD: {0} ({1})", upf.DistinguishedName, upf.Sid)) + "\r\n";
                        var proxy = UserProxyFullPrincipal.FindByIdentity(lds, upf.Sid.ToString());
                        message += (string.Format("LDS: {0} ({1})", proxy.DistinguishedName, proxy.Sid)) + "\r\n";
                        try
                        {
                            gpf.AddMember(proxy);
                            message += "Added to group." + "\r\n";
                            message +=
                                (string.Format("Added {0} to {1}", proxy.DistinguishedName, gpf.DistinguishedName)) +
                                "\r\n";
                        }
                        catch (Exception e)
                        {
                            message += "Exception Adding to group: " + e + "\r\n";
                        }
                    }
                    catch (Exception e1)
                    {
                        message += "Exception Processing " + udn + ": " + e1 + "\r\n";
                    }
                    lock (_fslog)
                    {
                        Console.WriteLine(message);
                        _logOut.Write(message);
                        _fslog.Flush();
                        message = string.Empty;
                    }
                }
            }
        }

        #region SupportMethods

        public static List<string> FilterUnique(List<string> master, List<string> slave)
        {
            master = SortUnique(master);
            slave = SortUnique(slave);
            var sorted = (from s in slave let match = master.Any(m => m == s) where !match select s).ToList();
            return SortUnique(sorted);
        }

        public static List<User> FilterUnique(List<User> master, List<User> slave)
        {
            master = SortUnique(master);
            slave = SortUnique(slave);
            var sorted = (from s in slave let match = master.Any(m => m.Dn == s.Dn) where !match select s).ToList();
            return SortUnique(sorted);
        }

        public static List<string> SortUnique(List<string> unsorted)
        {
            var last = "";
            unsorted.Sort();
            var sorted = new List<string>();
            foreach (var s in unsorted)
            {
                if (s.ToLowerInvariant() != last.ToLowerInvariant())
                {
                    sorted.Add(s);
                }
                last = s;
            }
            return sorted;
        }

        public static List<AdObject> SortUnique(List<AdObject> unsorted)
        {
            var last = new AdObject();
            unsorted.Sort();
            var sorted = new List<AdObject>();
            foreach (var o in unsorted)
            {
                if (!o.Equals(last))
                {
                    sorted.Add(o);
                }
                last = o;
            }
            return sorted;
        }

        public static List<User> SortUnique(List<User> unsorted)
        {
            var last = new User();
            unsorted.Sort();
            var sorted = new List<User>();
            foreach (var u in unsorted)
            {
                if (!u.Equals(last))
                {
                    sorted.Add(u);
                }
                last = u;
            }
            return sorted;
        }

        public static List<Group> SortUnique(List<Group> unsorted)
        {
            var last = new Group();
            unsorted.Sort();
            var sorted = new List<Group>();
            foreach (var g in unsorted)
            {
                if (!g.Equals(last))
                {
                    sorted.Add(g);
                }
                last = g;
            }
            return sorted;
        }

        #endregion

        #region Nested type: ThreadObject

        private struct ThreadObject
        {
            public List<AdObject> Objects;
            public string OuDn;
            public DateTime Start;
            public int ThreadNumber;
        }

        #endregion
    }
}