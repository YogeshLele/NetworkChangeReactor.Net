using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Management;
using System.Collections;

namespace NetworkChangeReactor
{
    public class Reactor
    {
        private static string PING_TEST_HOST = "";
        private static Dictionary<DateTime, bool> DailyCatalog = new Dictionary<DateTime, bool>();
        private static object locker = new object();

        public static bool Start()
        {
            Logger.Debug("Starting Network Change Reactor's Core");
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);


            Logger.Debug("Started Reactor Core");
            return true;
        }

        private static void AddressChangedCallback(object sender, EventArgs e)
        {

            PING_TEST_HOST = ConfigurationManager.AppSettings["HOST_TO_CHECK"];
            if (String.IsNullOrEmpty(PING_TEST_HOST))
            {
                Logger.Warn("No host specified to check network change success");
                return;
            }

            // Letting un -hibernation properly establish connectivity first and then ping
            Thread.Sleep(5000);
            if (!CheckIfTargetNetworkIsUp())
            {
                Logger.Info(PING_TEST_HOST + " is not up!");
                return;
            }

            lock (locker)
            {
                if (HasChangeOccuredForTheDay(DateTime.Now))
                {
                    Logger.Info("Reactor has already been invoked earlier today");
                    return;
                }
                UpdateDailyCatalog(DateTime.Now.Date);
            }

            LaunchDependentApplications();

        }

        private static void LaunchDependentApplications()
        {
            try
            {
                //Read the list of applications
                var apps_to_start_string = ConfigurationManager.AppSettings["APPS_TO_START"];
                if (string.IsNullOrEmpty(apps_to_start_string))
                {
                    Logger.Warn("No app(s) found for starting");
                    return;
                }

                var app_list = apps_to_start_string
                                    .Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                    .ToList();

                var running_processes = GetProcessInfo();
                Logger.Info("Application(s) to be launched are: ");
                Logger.Info(string.Join("\n", app_list));

                foreach (var appName in app_list)
                {
                    if (running_processes.Any(x => x.ToLower() == (appName.ToLower())))
                    {
                        Logger.Info("Process: " + appName + " is already running.");
                        continue;
                    }
                    else
                    {
                        Logger.Info("Starting: " + appName);
                        try
                        {
                            ProcessExtensions.StartProcessAsCurrentUser(appName);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Failed to launch " + appName);
                            Logger.Info(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error while launching applications: " + e.Message);
            }
        }

        private static void UpdateDailyCatalog(DateTime date)
        {
            DailyCatalog.Clear();
            DailyCatalog.Add(date, true);
        }

        private static bool HasChangeOccuredForTheDay(DateTime now)
        {
            return (DailyCatalog.ContainsKey(DateTime.Now.Date));
        }

        private static bool CheckIfTargetNetworkIsUp()
        {
            Ping pinger = new Ping();
            try
            {
                var response = pinger.Send(PING_TEST_HOST);
                if (response.Status == IPStatus.Success)
                {
                    Logger.Info(PING_TEST_HOST + " is up !!!");
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static List<string> GetProcessInfo()
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select (string)mo["ExecutablePath"];

                return query.Where(x=>!string.IsNullOrEmpty(x)).ToList();
            }
        }
    }
}
