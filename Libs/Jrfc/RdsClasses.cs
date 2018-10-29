using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Net;

namespace Jrfc.Rds
{
    [Serializable()]
    public class RdsServer
    {
        public RdsServer() {}
        public RdsServer(string _Hostname)
        {
            this.Hostname = _Hostname;
            this.AppList = new RdsAppList(this);
        }

        public System.Guid GUID
        {
            get
            {
                return RdsServer.GetGuidForHostname(this.Hostname);
            }
        }

        public string Hostname { get; set; }
        public RdsAppList AppList { get; set; }

        public static string GetHostnameFromGuid(System.Guid _GUID)
        {
            string hostname = null;
            try
            {
                string url = (string)Registry.GetValue(RdsServer.GetFeedRegistryKeyString(_GUID), "URL", "");
                if (!string.IsNullOrWhiteSpace(url))
                {
                    Uri uri = new Uri(url);
                    hostname = uri.Host;
                }
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            return hostname;
        }

        public static string GetFeedRegistryKeyString(System.Guid _GUID)
        {
            return @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Workspaces\Feeds\{" + _GUID.ToString() + "}";
        }
        public static string GetResourceMapRegistryKeyString(System.Guid _GUID)
        {
            return GetFeedRegistryKeyString(_GUID) + @"\ResourceMap";
        }
        public static bool IsServerConnectionDefinedInLocalRegistry(System.Guid _GUID)
        {
            RegistryKey regkey = null;
            bool rtn = false;
            try
            {
                string rootkey = @"SOFTWARE\Microsoft\Workspaces\Feeds\{" + _GUID.ToString() + @"}";
                regkey = Registry.CurrentUser.OpenSubKey(rootkey);
                if (regkey != null)
                    rtn = true;
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (regkey != null)
                    regkey.Close();
            }
            return rtn;
        }
        public static bool IsServerConnectionDefinedInLocalRegistry(string _Hostname)
        {
            System.Guid test_guid = GetGuidForHostname(_Hostname);
            if (test_guid != System.Guid.Empty)
            {
                return true;
            }
            return false;
        }
        public static System.Guid GetGuidForHostname(string _Hostname)
        {
            System.Guid rtn = System.Guid.Empty;
            string hostname_lc = _Hostname.ToLower();
            RegistryKey workspacessFeedsKey = null;

            try
            {
                workspacessFeedsKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Workspaces\Feeds");
                if (workspacessFeedsKey != null)
                {
                    string[] subkeys = workspacessFeedsKey.GetSubKeyNames();
                    foreach (string subkey in subkeys)
                    {
                        object publisher = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Workspaces\Feeds\" + subkey, "Publisher", "");
                        if (publisher is string)
                        {
                            if (!string.IsNullOrWhiteSpace((string)publisher))
                            {
                                if (((string)publisher).ToLower() == hostname_lc)
                                {
                                    System.Guid test_guid;
                                    if(System.Guid.TryParse(subkey, out test_guid))
                                    {
                                        rtn = test_guid;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (workspacessFeedsKey != null)
                    workspacessFeedsKey.Close();
            }
            return rtn;
        }
        public static string[] GetRdsHostnamesDefinedInLocalRegistry()
        {
            List<string> hostnames = new List<string>();
            RegistryKey workspacessFeedsKey = null;
            try
            {
                workspacessFeedsKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Workspaces\Feeds");
                if (workspacessFeedsKey != null)
                {
                    string[] subkeys = workspacessFeedsKey.GetSubKeyNames();
                    foreach (string subkey in subkeys)
                    {
                        object publisher = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Workspaces\Feeds\" + subkey, "Publisher", "");
                        if (publisher is string)
                        {
                            if (!string.IsNullOrWhiteSpace((string)publisher))
                            {
                                hostnames.Add((string)publisher);
                            }
                        }
                    }
                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (workspacessFeedsKey != null)
                    workspacessFeedsKey.Close();
            }
            if (hostnames.Count > 0)
            {
                return hostnames.ToArray<string>();
            }
            return null;
        }
    }

    [Serializable()]
    public class RdsServerList : ObservableCollection<RdsServer>
    {
        public RdsServerList() { }
        public RdsServerList(bool _RefreshOnCreate = true)
        {
            if (_RefreshOnCreate)
            {
                this.RefreshList();
            }
        }

        public RdsServerList(RdsAppList _RdsAppsList)
        {
            foreach(RdsApp app in _RdsAppsList)
            {
                if (this[app.Hostname] == null)
                {
                    //this.Add(new RdsServer(app.RdsServerGuid));
                    this.Add(new RdsServer(app.Hostname));
                }
            }
        }

        public RdsServerList Copy(RdsServerList _NewList)
        {
            this.Clear();
            foreach(RdsServer s in _NewList)
            {
                this.Add(s);
            }
            return this;
        }
        public void RefreshList()
        {
            //RegistryKey regkey = null;
            try
            {
                this.Clear();

                string[] hostnames = RdsServer.GetRdsHostnamesDefinedInLocalRegistry();

                if (hostnames != null)
                {
                    foreach (string host in hostnames)
                    {
                        this.Add(new RdsServer(host));
                    }
                }
                //regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Workspaces\Feeds");
                //if (regkey != null)
                //{
                //    string[] subkeys = regkey.GetSubKeyNames();

                //    foreach (string subkey in subkeys)
                //    {
                //        Guid server_guid;
                //        if (System.Guid.TryParse(subkey, out server_guid))
                //        {
                //            this.Add(new RdsServer(server_guid));
                //            //string host = this[server_guid].Hostname;
                //        }
                //    }
                //}
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            //finally
            //{
            //    if (regkey != null)
            //        regkey.Close();
            //}
        }

        public RdsServer this[System.Guid index]
        {
            get
            {
                foreach(RdsServer server in this)
                {
                    if(server.GUID == index)
                    {
                        return (server);
                    }
                }
                return null;
            }
        }

        public RdsServer this[string _Hostname]
        {
            get
            {
                _Hostname = _Hostname.ToLower();
                foreach (RdsServer server in this)
                {
                    if (server.Hostname.ToLower() == _Hostname)
                    {
                        return (server);
                    }
                }
                return null;
            }
        }

        public string ToXmlString()
        {
            return Jrfc.Utility.ObjectSerializer<RdsServerList>.ToXml(this);
        }
    }

    [Serializable()]
    public class RdsServerId
    {
        public string Hostname;
        public System.Guid GUID
        {
            get
            {
                return RdsServer.GetGuidForHostname(this.Hostname);
            }
        }

        public RdsServerId()
        {
            this.Hostname = string.Empty;
        }
        public RdsServerId(string _Hostname)
        {
            this.Hostname = _Hostname;
        }
    }

    [Serializable()]
    public class RdsServerIdList: List<RdsServerId>
    {
        public RdsServerIdList() { }
        public string this[System.Guid index]
        {
            get
            {
                foreach (RdsServerId id in this)
                {
                    if (id.GUID == index)
                    {
                        return (id.Hostname);
                    }
                }
                return null;
            }
        }

        public void Add(string _Hostname)
        {
            RdsServerId id = new RdsServerId(_Hostname);
            base.Add(id);
        }
        public System.Guid this[string _Hostname]
        {
            get
            {
                _Hostname = _Hostname.ToLower();
                foreach (RdsServerId id in this)
                {
                    if (id.Hostname.ToLower() == _Hostname)
                    {
                        return (id.GUID);
                    }
                }
                return System.Guid.Empty;
            }
        }
    }

    [Serializable()]
    public class RdsApp
    {
        public string RegistryFullKeyString
        {
            get { return @"HKEY_CURRENT_USER\" + this.RegistryRootKeyString + @"\" + this.RegistrySubKeyString; }
        }
        public string RegistryRootKeyString { get; set; }
        public string RegistrySubKeyString { get; set; }
        public string DisplayName
        {
            get
            {
                return GetDisplayNameFromRegistrySubKey();
            }
            set { }
        }
        public System.Guid RdsServerGuid { get; set; }

        public Jrfc.MruList RemoteMruList { get; set; }
        public string Hostname { get; set; }

        public bool IsServerConnectionDefinedInLocalRegistry
        {
            get
            {
                return RdsServer.IsServerConnectionDefinedInLocalRegistry(this.Hostname);
            }
        }

        public string ServerIP4AddressAsString
        {
            get
            {
                IPAddress[] iplist = Dns.GetHostAddresses(this.Hostname);
                foreach(IPAddress ip in iplist)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
                }
                return "";
            }
        }
        
        public string IconFile
        {
            get
            {
                if (this.RegistryRootKeyString == null || this.RegistrySubKeyString == null)
                    return null;

                string icon_file_name = (string)Registry.GetValue(this.RegistryFullKeyString, "IconFile", "");
                return icon_file_name;
            }
        }
        public string CachedIconFile
        {
            get
            {
                string cachedicon = Path.Combine(this.GetLocalIconCacheFolder(), this.DisplayName + ".ico");
                if (File.Exists(cachedicon))
                {
                    return cachedicon;
                }
                else if(File.Exists(this.IconFile))
                {
                    return this.IconFile;
                }
                return string.Empty;
            }
        }
        public string RdpFilename { get; set; }
        public string[] FileExtensionAssociations { get; set; }

        public RdsApp()
        {
            this.RegistryRootKeyString = null;
            this.RegistrySubKeyString = null;
            this.Hostname = "";
            this.RdsServerGuid = System.Guid.Empty;
            this.AvailableFromServer = true;
            this.RdpFilename = null;
            this.FileExtensionAssociations = null;
        }
        public RdsApp(string _RegistryRootKeyString, string _RegistrySubKeyString, System.Guid _Guid, string _Hostname)
        {
            this.RegistryRootKeyString = _RegistryRootKeyString;
            this.RegistrySubKeyString = _RegistrySubKeyString;
            this.Hostname = _Hostname;
            this.RdsServerGuid = _Guid;
            this.AvailableFromServer = true;
            this.RdpFilename = null;
            this.FileExtensionAssociations = null;

            if (File.Exists(this.ShortCutLnkFile))
            {
                string rdp_file = Jrfc.Shell.GetWindowsShortcutTargetFileArguments(this.ShortCutLnkFile);
                if (File.Exists(rdp_file))
                {
                    this.RdpFilename = rdp_file;
                    this.FileExtensionAssociations = Jrfc.Shell.GetRdpFileParameter_remoteapplicationfileextensions(rdp_file);
                }
            }

            CopyIconFileToLocalCache();
        }

        public void Refresh()
        {
            if (File.Exists(this.ShortCutLnkFile))
            {
                string rdp_file = Jrfc.Shell.GetWindowsShortcutTargetFileArguments(this.ShortCutLnkFile);
                if (File.Exists(rdp_file))
                {
                    this.RdpFilename = rdp_file;
                    this.FileExtensionAssociations = Jrfc.Shell.GetRdpFileParameter_remoteapplicationfileextensions(rdp_file);
                }
            }
        }
        public string ShortCutLnkFile
        {
            get
            {
                string appDataFolder = Environment.GetEnvironmentVariable("APPDATA");
                return appDataFolder + "\\Microsoft\\Windows\\Start Menu\\Programs\\" + this.Hostname + " (RADC)\\" + this.RegistrySubKeyString + ".lnk";
            }
        }

        public void Launch(string _Arguments = null)
        {
            //
            // C:\Users\John\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\accounting.jrfcorp.net(RADC)\Acrobat Reader DC (accounting.jrfcorp.net).lnk
            //
            try
            {
                ProcessStartInfo psi;
                string psi_filename = null;
                if(_Arguments != null)
                {
                    psi_filename = this.CreateTempRdpFileToPassArguments(_Arguments);
                }
                if (psi_filename == null)
                {
                    psi_filename = this.ShortCutLnkFile;
                }
                psi = new ProcessStartInfo(psi_filename);
                Process.Start(psi);
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }

        public string CreateTempRdpFileToPassArguments(string _Arguments)
        {
            string temp_path = null;
            try
            {
                if (string.IsNullOrWhiteSpace(this.RdpFilename))
                    return null;

                string[] lines = File.ReadAllLines(this.RdpFilename);
                if (lines == null)
                    return null;

                string temp_folder = Path.GetTempPath();
                if (string.IsNullOrWhiteSpace(temp_folder))
                    return null;

                string temp_filename = Path.GetFileName(this.RdpFilename);
                temp_path = Path.Combine(temp_folder, temp_filename);

                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].StartsWith("remoteapplicationcmdline:s:"))
                    {
                        lines[i] = "remoteapplicationcmdline:s:\"" + _Arguments +"\"";
                    }
                    else if (lines[i].StartsWith("signscope:s:"))
                    {
                        lines[i] = "";
                    }
                    else if (lines[i].StartsWith("signature:s:"))
                    {
                        lines[i] = "";
                    }
                }
                File.WriteAllLines(temp_path, lines);
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
                return null;
            }
            return temp_path;
        }

        public Jrfc.MruList GetRemoteMruList()
        {
            if (this.FileExtensionAssociations == null)
                return null;

            return MruList.CreateRemoteHostMruListByExtList(this.Hostname, this.FileExtensionAssociations);
        }
        private string GetDisplayNameFromRegistrySubKey()
        {
            string name = this.RegistrySubKeyString.Replace("(" + this.Hostname + ")", "");
            return name.Trim();
        }

        public enum PinAction
        {
            Pin,
            UnPin
        }
        public void PinToTaskbar(PinAction _PinAction)
        {
            if (!File.Exists(this.ShortCutLnkFile)) throw new FileNotFoundException(this.ShortCutLnkFile);

            try
            {
                Jrfc.Shell.Pin(this.ShortCutLnkFile, Shell.PIN_DESTINATION.Taskbar, this.CachedIconFile);
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }
        public bool IsPinnedToTaskbar()
        {
            return Jrfc.Shell.IsPinned(this.ShortCutLnkFile, Shell.PIN_DESTINATION.Taskbar);
        }
        public void PinToStart(PinAction _action)
        {
            if (!File.Exists(this.ShortCutLnkFile)) throw new FileNotFoundException(this.ShortCutLnkFile);

            try
            {
                Jrfc.Shell.Pin(this.ShortCutLnkFile, Shell.PIN_DESTINATION.Start, this.CachedIconFile);
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }
        public bool IsPinnedToStart()
        {
            return Jrfc.Shell.IsPinned(this.ShortCutLnkFile, Shell.PIN_DESTINATION.Start);
        }

        public bool AvailableFromServer { get; set; }

        private void CopyIconFileToLocalCache()
        {
            try
            {
                string icon_filename = this.IconFile;
                if (string.IsNullOrWhiteSpace(icon_filename))
                    return;

                if (!File.Exists(icon_filename))
                    return;

                string iconcache_folder = this.GetLocalIconCacheFolder();
                if (string.IsNullOrWhiteSpace(iconcache_folder))
                    return;

                string CachedIcon_Filename = Path.Combine(iconcache_folder, this.DisplayName + ".ico");
                try
                {
                    File.Copy(icon_filename, CachedIcon_Filename, true);
                    File.SetAttributes(CachedIcon_Filename, FileAttributes.Normal);
                }
#pragma warning disable 0168    
                catch (System.Exception x)
#pragma warning restore 0168
                {

                }
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }

        private string GetLocalIconCacheFolder()
        {
            string appdata_folder = Jrfc.Shell.GetUserAppdataFolder();
            string local_icon_cache = Path.Combine(appdata_folder, "IconCache", Hostname);
            if (!Directory.Exists(local_icon_cache))
            {
                Directory.CreateDirectory(local_icon_cache);
                if (!Directory.Exists(local_icon_cache))
                    return null;
            }
            return local_icon_cache;
        }
    }

    [Serializable()]
    public class RdsAppList : ObservableCollection<RdsApp>
    {
        //public RdsServerList ServerList { get; set; }
        //public RdsServer HostServer { get; set; }
        public RdsServerIdList RdsServerIds;

        public RdsAppList()
        {
            RdsServerIds = new RdsServerIdList();
            //this.ServerList = null;
            //this.HostServer = null;
        }

        public RdsAppList(RdsServerList _RdsServerList)
        {
            this.RdsServerIds = new RdsServerIdList();
            foreach (RdsServer s in _RdsServerList)
            {
                this.RdsServerIds.Add(s.Hostname);
            }
            this.RefreshList();
        }

        public void Add(RdsServerList _RdsServerList)
        {
            this.RdsServerIds.Clear();
            foreach (RdsServer s in _RdsServerList)
            {
                this.RdsServerIds.Add(s.Hostname);
            }
            this.RefreshList();
        }

        public RdsAppList(RdsServer _HostServer)
        {
            RdsServerIds = new RdsServerIdList();
            //this.ServerList = new RdsServerList(false);
            //this.ServerList.Add(_HostServer);
            this.RefreshList();
        }
        public RdsAppList(StringCollection _RegistryFullKeyStrings, RdsAppList _MasterAppList)
        {
            RdsServerIds = new RdsServerIdList();
            // Creat an AppList made up of every app that can be found using _RegistryFullKeyStrings to 
            // search another source RdsAppList _MasterAppList
            foreach (string regkey in _RegistryFullKeyStrings)
            {
                RdsApp app = _MasterAppList[regkey];
                if(app != null)  // if null then a previous favorite is not in the _MasterAppList. Ignore it.
                {
                    this.Add(app);
                }
            }
        }

        public new void Add(RdsApp _app)
        {
            base.Add(_app);
            
            if(this.RdsServerIds[_app.Hostname] == null)
            {
                this.RdsServerIds.Add(_app.Hostname);
            }
        }

        public RdsAppList Copy(RdsAppList _NewList)
        {
            this.Clear();
            foreach(RdsApp app in _NewList)
            {
                this.Add(app);
            }
            return this;
        }
        public StringCollection ToStringCollection()
        {
            StringCollection sc = new StringCollection();
            foreach(RdsApp app in this)
            {
                sc.Add(app.RegistryFullKeyString);
            }
            return sc;
        }

        //public List<string> Hostnames;
        //{
        //    get
        //    {
        //        List<string> hosts = new List<string>();
        //        string last_host = "";
        //        foreach(RdsApp app in this)
        //        {
        //            if(app.Hostname != last_host)
        //            {
        //                hosts.Add(app.Hostname);
        //                last_host = app.Hostname;
        //            }
        //        }
        //        return hosts;
        //    }
        //}
        //public List<System.Guid> HostGuids;
        //{
        //    get
        //    {
        //        List<System.Guid> hosts = new List<System.Guid>();
        //        System.Guid last_host = System.Guid.Empty;
        //        foreach (RdsApp app in this)
        //        {
        //            if (app.RdsServerGuid != last_host)
        //            {
        //                hosts.Add(app.RdsServerGuid);
        //                last_host = app.RdsServerGuid;
        //            }
        //        }
        //        return hosts;
        //    }
        //}

        public Dictionary<string, string> GetAppLinksAndDisplayNamesForHost(string _Hostname)
        {
            Dictionary<String, string> linkNamePairs = new Dictionary<string, string>();

            foreach (RdsApp app in this)
            {
                if (app.Hostname == _Hostname)
                    linkNamePairs.Add(app.ShortCutLnkFile, app.DisplayName);
            }

            return linkNamePairs;
        }

        public bool TestFalse()
        {
            return false;
        }
        public void RefreshList()
        {
            if (this.RdsServerIds == null)
                return;

            RdsAppList refreshed_apps = new RdsAppList();
            //this.Clear();
            foreach (RdsServerId id in this.RdsServerIds)
            {
                RegistryKey regkey = null;
                try
                {
                    string rootkey = @"SOFTWARE\Microsoft\Workspaces\Feeds\{" + id.GUID.ToString() + @"}\ResourceMap";
                    regkey = Registry.CurrentUser.OpenSubKey(rootkey);
                    if (regkey != null)
                    {
                        string[] subkeys = regkey.GetSubKeyNames();

                        foreach (string subkey in subkeys)
                        {
                            refreshed_apps.Add(new RdsApp(rootkey, subkey, id.GUID, id.Hostname));
                            //this.Add(new RdsApp(rootkey, subkey, id.GUID, id.Hostname));
                        }
                    }
                }
                catch (System.Exception x)
                {
                    Jrfc.Exception.HandleException(x);
                }
                finally
                {
                    if (regkey != null)
                        regkey.Close();
                }
            }
            this.ReconcileWithRefreshedAppList(refreshed_apps);
        }

        private void ReconcileWithRefreshedAppList(RdsAppList _RefreshedAppList)
        {
            this.SetAllAppsUnavailableFromServer();
            foreach(RdsApp refreshed_app in _RefreshedAppList)
            {
                RdsApp current_app = this.Find(refreshed_app.Hostname, refreshed_app.DisplayName);
                if(current_app != null)
                {
                    current_app.AvailableFromServer = true;
                    current_app.Refresh();
                }
                else
                {
                    this.Add(refreshed_app);
                }
            }
        }

        private void SetAllAppsUnavailableFromServer()
        {
            foreach (RdsApp app in this)
                app.AvailableFromServer = false;
        }
        public RdsApp Find(string _Hostname, string _DisplayName)
        {
            foreach (RdsApp app in this)
            {
                if (app.Hostname == _Hostname && app.DisplayName == _DisplayName)
                {
                    return app;
                }
            }
            return null;
        }

        public RdsApp this[string _FullKeyString]
        {
            get
            {
                foreach (RdsApp app in this)
                {
                    if(app.RegistryFullKeyString == _FullKeyString)
                    {
                        return app;
                    }
                }
                return null;
            }
        }

        public string ToXmlString()
        {
            return Jrfc.Utility.ObjectSerializer<RdsAppList>.ToXml(this);
        }
    }
}
