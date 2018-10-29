using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace TaskbarStub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string LaunchFilename { get; set; }
        public string IconFilename { get; set; }
        public string IniFilename { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                string app_name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
                this.IniFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, app_name + ".ini");
                ReadIniFile();
                ProcessStartInfo psi = new ProcessStartInfo(LaunchFilename);
                Process.Start(psi);
                Application.Current.Shutdown();
            }
            catch(Exception x)
            {
                System.Windows.MessageBox.Show(x.Message + Environment.NewLine + Environment.NewLine + x.StackTrace);
            }
        }

        private void ReadIniFile()
        {
            string[] ini_lines = File.ReadAllLines(this.IniFilename);

            string str = "";
            for(int i=0; i < ini_lines.Count(); i++)
            {
                str += ini_lines[i] + Environment.NewLine;
            }
            foreach (string line in ini_lines)
            {
                string[] tokens = line.Split('=');
                if(tokens.Count() == 2)
                {
                    if(tokens[0].Trim().ToLower() == "iconfilename")
                    {
                        this.IconFilename = tokens[1].Trim();
                    }
                    else if(tokens[0].Trim().ToLower() == "launchfilename")
                    {
                        this.LaunchFilename = tokens[1].Trim();
                    }
                }
            }
            return;
        }
    }
}
//for(int i=0; i < e.Args.Count(); i++)
//{
//    if(e.Args[i].ToLower() == "icon")
//    {

//    }
//}