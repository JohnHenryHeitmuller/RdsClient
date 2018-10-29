using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Jrfc;

namespace RdsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if(Logging.WriteToWindowsEventLog("Application Startup") != null)
            {
                System.Windows.MessageBox.Show("Not yet implemented (some notification about elevation to create Eventlog folder");
                ProcessStartInfo psi = new ProcessStartInfo(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase,
                    "performadminsetuptasks");
                psi.Verb = "runas";
                System.Diagnostics.Process.Start(psi);
            }

            if (e.Args.Length == 0)
                return; // no arguments to parse

            foreach(string arg in e.Args)
            {
                switch(arg.ToLower())
                {
                    case "performadminsetuptasks":
                        this.PerformAdminSetupTasks();
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void PerformAdminSetupTasks()
        {
            try
            {
                EventLog.CreateEventSource(Jrfc.Logging.WindowsEventLog_SourceName, Jrfc.Logging.WindowsEventLog_LogName);
            }
            catch(System.Exception x)
            {
                if (x.Message != "Source " + Jrfc.Logging.WindowsEventLog_SourceName + " already exists on the local computer.")
                {
                    System.Windows.MessageBox.Show(x.Message);
                }
            }
        }
    }
}
