using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Reflection;
using System.Net;
using System.Windows.Input;

namespace Jrfc.Wts
{ 
    public class WTSUniqueSessionId
    {
        public WTSUniqueSessionId()
        {
        }
        public WTSUniqueSessionId(string _Hostname, int _SessionId)
        {
            this.Hostname = _Hostname;
            this.SessionId = _SessionId;
        }
        public string Hostname { get; set; }
        public int SessionId { get; set; }

        public static bool operator ==(WTSUniqueSessionId _usid_a, WTSUniqueSessionId _usid_b)
        {
            if(_usid_a.SessionId == _usid_b.SessionId && _usid_a.Hostname.ToLower() == _usid_b.Hostname.ToLower())
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(WTSUniqueSessionId _usid_a, WTSUniqueSessionId _usid_b)
        {
            return !(_usid_a == _usid_b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class WTSUniqueSessionIdList : List<WTSUniqueSessionId>
    {

    }
    public class WTSSession
    {
        public const int SessionIdNull = -1;
        public Jrfc.NativeMethods.WTS_SESSION_INFO WtsSessionInfo { get; set; }
        //public WTSSession(string _Hostname, Jrfc.WinApi.WTS_SESSION_INFO _si)
        //{
        //    this.SessionId = _si.SessionId;
        //    this.WinStationname = Marshal.PtrToStringAnsi(this.WtsSessionInfo.pWinStationName);
        //    this.UniqueSessionId = new WTSUniqueSessionId(_Hostname, _si.SessionId);
        //    this.State = _si.State;
        //}
        public WTSSession(IntPtr _ServerHandle, string _Hostname, Jrfc.NativeMethods.WTS_SESSION_INFO _si)
        {
            this.SessionId = _si.SessionId;
            this.State = _si.State;
            this.WinStationName = (string)Marshal.PtrToStringAnsi(_si.pWinStationName);
            this.UniqueSessionId = new WTSUniqueSessionId(_Hostname, _si.SessionId);
            this.Username = (string)this.QuerySessionInformation(_ServerHandle, Jrfc.NativeMethods.WTS_INFO_CLASS.WTSUserName);
            this.ClientHostname = (string)this.QuerySessionInformation(_ServerHandle, Jrfc.NativeMethods.WTS_INFO_CLASS.WTSClientName);
            this.DomainName = (string)this.QuerySessionInformation(_ServerHandle, Jrfc.NativeMethods.WTS_INFO_CLASS.WTSDomainName);
            this.WtsClientAddress = (Jrfc.NativeMethods.WTS_CLIENT_ADDRESS)this.QuerySessionInformation(_ServerHandle, Jrfc.NativeMethods.WTS_INFO_CLASS.WTSClientAddress);
        }
        public WTSUniqueSessionId UniqueSessionId { get; set; }
        public string Hostname
        {
            get { return this.UniqueSessionId.Hostname; }
            set { this.UniqueSessionId.Hostname = value; }
        }
        public int SessionId  { get; set; }
        private string m_ClientHostname;
        public string ClientHostname
        {
            get { return m_ClientHostname; }
            set
            {
                if(string.IsNullOrWhiteSpace(this.WinStationName) || string.IsNullOrWhiteSpace(this.UniqueSessionId.Hostname))
                {
                    m_ClientHostname = value;
                }
                else if(this.WinStationName == "Console")
                {
                    m_ClientHostname = this.UniqueSessionId.Hostname;
                }
                else
                {
                    m_ClientHostname = value;
                }
            }
        }
        public string WinStationName { get; set; }
        public string DomainName { get; set; }
        private string m_UserName;
        public string Username
        {
            get { return this.m_UserName; }
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    this.m_UserName = value;
                }
                else if(this.WinStationName == "Console")
                {
                    this.m_UserName = "[Console]";
                }
            }
        }
        public Jrfc.NativeMethods.WTS_CLIENT_ADDRESS WtsClientAddress { get; set;  }
        public string ClientIpAddressAsString
        {
            get
            {
                string ip_string = "";
                if(this.WinStationName == "Console")
                {
                    try
                    {
                        IPAddress[] ips = System.Net.Dns.GetHostAddresses(this.UniqueSessionId.Hostname);
                        if (ips == null)
                            return "";
                        for(int i=0; i < ips.Count(); i++)
                        {
                            if (i != 0)
                                ip_string += ", ";
                            ip_string += ips[i].ToString();
                        }
                        return ip_string;
                    }
                    catch(System.Exception x)
                    {
                        return x.Message;
                    }
                }
                if (this.WtsClientAddress.AddressFamily == Jrfc.NativeMethods.ADDRESS_FAMILIES.AF_INET)
                {
                    ip_string = this.WtsClientAddress.Address[4] + "." + this.WtsClientAddress.Address[5] + "." + this.WtsClientAddress.Address[6] + "." + this.WtsClientAddress.Address[7];
                    return ip_string;
                }
                if (this.State == NativeMethods.WTS_CONNECTSTATE_CLASS.WTSDisconnected)
                    return "";
                if (this.State == NativeMethods.WTS_CONNECTSTATE_CLASS.WTSListen)
                    return "";
                return this.WtsClientAddress.AddressFamily.ToString();
            }
        }
        public string ClientAddress { get; set; }
        public Jrfc.NativeMethods.WTS_CONNECTSTATE_CLASS State { get; set; }
        public string ConnectionState { get { return Jrfc.NativeMethods.GetConnectStateString(this.State); }}
        public object QuerySessionInformation(IntPtr _ServerHandle, Jrfc.NativeMethods.WTS_INFO_CLASS _Property)
        {
            IntPtr out_data = IntPtr.Zero;
            uint out_bytes = 0;
            Object return_object = null;

            bool rtn = Jrfc.NativeMethods.WTSQuerySessionInformation( _ServerHandle, this.SessionId, _Property, out out_data, out out_bytes);
            if(rtn == false)
            {
                return ("ErrorNbr=" + Marshal.GetLastWin32Error().ToString());
            }

            switch(_Property)
            {
                case NativeMethods.WTS_INFO_CLASS.WTSClientName:
                case NativeMethods.WTS_INFO_CLASS.WTSDomainName:
                case NativeMethods.WTS_INFO_CLASS.WTSUserName:
                    return_object = (string)Marshal.PtrToStringAnsi(out_data);
                    break;
                case NativeMethods.WTS_INFO_CLASS.WTSClientAddress:
                    return_object = (Jrfc.NativeMethods.WTS_CLIENT_ADDRESS)Marshal.PtrToStructure(out_data, typeof(Jrfc.NativeMethods.WTS_CLIENT_ADDRESS));
                    break;
            }
            if(out_data != IntPtr.Zero)
                Jrfc.NativeMethods.WTSFreeMemory(out_data);

            return return_object;
        }

        public bool LogoffSession()
        {
            return LogoffSession(this.Hostname, this.SessionId);
        }
        public static bool LogoffSession(string _Hostname, int _SessionId)
        {
            Mouse.OverrideCursor = Cursors.Wait;    
            bool rtn = false;
            IntPtr hServer = IntPtr.Zero;
            try
            {
                hServer = Jrfc.NativeMethods.WTSOpenServer(_Hostname);
                rtn = NativeMethods.WTSLogoffSession(hServer, _SessionId, true); // true = wait for session to logoff before returning
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            finally
            {
                if (hServer != IntPtr.Zero)
                    Jrfc.NativeMethods.WTSCloseServer(hServer);
            }
            Mouse.OverrideCursor = null;
            return rtn;
        }
    }

    public class WTSSessionList : ObservableCollection<WTSSession>
    {
        public StringCollection WTSServerHostnames { get; set; }
        private BackgroundWorker m_RefreshThread = new BackgroundWorker();

        public WTSSessionList()
        {
            this.WTSServerHostnames = null;
            InitializeRefreshBackgroundWorker();
        }

        public WTSSessionList(StringCollection _WTSServerHostNames)
        {
            this.WTSServerHostnames = _WTSServerHostNames;
            InitializeRefreshBackgroundWorker();
            this.RefreshList();
        }

        public WTSSessionList(string _WTSServerHostName)
        {
            this.WTSServerHostnames = new StringCollection();
            this.WTSServerHostnames.Add(_WTSServerHostName);
            InitializeRefreshBackgroundWorker();
            this.RefreshList();
        }

        public WTSSessionList(IEnumerable<WTSSession> collection) : base(collection)
        {
        }

        private void InitializeRefreshBackgroundWorker()
        {
            this.m_RefreshThread.WorkerReportsProgress = true;
            this.m_RefreshThread.WorkerSupportsCancellation = true;
            this.m_RefreshThread.DoWork += new DoWorkEventHandler(this.RefreshThread_DoWork);
            this.m_RefreshThread.ProgressChanged += new ProgressChangedEventHandler(RefreshThread_ProgressChanged);
            this.m_RefreshThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshThread_RunWorkerCompleted);
        }

        private void RefreshThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void RefreshThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void RefreshThread_DoWork(object sender, DoWorkEventArgs e)
        {
            WTSUniqueSessionIdList valid_session_ids = new WTSUniqueSessionIdList();

            foreach (string server in this.WTSServerHostnames)
            {
                IntPtr hServer = IntPtr.Zero;
                try
                {
                    IntPtr SessionInfoPtr = IntPtr.Zero;
                    Int32 sessionCount = 0;
                    Int32 dataSize = Marshal.SizeOf(typeof(Jrfc.NativeMethods.WTS_SESSION_INFO));

                    hServer = Jrfc.NativeMethods.WTSOpenServer(server);
                    Int32 retVal = Jrfc.NativeMethods.WTSEnumerateSessions(hServer, 0, 1, ref SessionInfoPtr, ref sessionCount);
                    if (retVal != 0)
                    {
                        Int32 currentSession = (int)SessionInfoPtr;
                        for (int i = 0; i < sessionCount; i++)
                        {
                            Jrfc.NativeMethods.WTS_SESSION_INFO si = (Jrfc.NativeMethods.WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)currentSession, typeof(Jrfc.NativeMethods.WTS_SESSION_INFO));
                            currentSession += dataSize;

                            Jrfc.Wts.WTSSession session = new Jrfc.Wts.WTSSession(hServer, server, si);

                            bool filter = false;
                            if(session.State == NativeMethods.WTS_CONNECTSTATE_CLASS.WTSListen)
                            {
                                filter = true;
                            }
                            else if (session.State == NativeMethods.WTS_CONNECTSTATE_CLASS.WTSDisconnected && string.IsNullOrWhiteSpace(session.Username))
                            {
                                filter = true;
                            }
                            if (!filter)
                            {
                                this.AddOrUpdate(session);
                                valid_session_ids.Add(new WTSUniqueSessionId(server, session.SessionId)); // keep a list of existing sessions
                            }
                        }
                        Jrfc.NativeMethods.WTSFreeMemory(SessionInfoPtr);
                    }
                }
                catch (System.Exception x)
                {   Jrfc.Exception.HandleException(x);
                }
                finally
                {   if (hServer != IntPtr.Zero)
                        Jrfc.NativeMethods.WTSCloseServer(hServer);
                }
            }
            DeleteAllSessionIdsNotInList(valid_session_ids); // delete (old) sessions that are in the currect session list,
                                                             // but were not in the existing sessions just retreived from the 
                                                             // RDS server. Presumably any session not in the valid_session_ids 
                                                             // list no longer exists.
            this.m_RefreshThread.ReportProgress(100);
        }

        public void DeleteAllSessionIdsNotInList(WTSUniqueSessionIdList _valid_session_ids)
        {
            WTSUniqueSessionIdList kill_list = new WTSUniqueSessionIdList();
            foreach(WTSSession s in this)
            {
                bool kill = true;
                for(int i=0; i<_valid_session_ids.Count;i++)
                {
                    if(s.UniqueSessionId == _valid_session_ids[i])
                    {
                        kill = false;
                        break;
                    }
                }
                if (kill)
                {
                    kill_list.Add(s.UniqueSessionId);
                }
            }
            if(kill_list.Count > 0)
                this.RemoveSessions(kill_list);
        }

        public void RemoveSessions(WTSUniqueSessionIdList _SessionIds)
        {
            if (_SessionIds.Count == 0)
                return;

            foreach (WTSUniqueSessionId id in _SessionIds)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].UniqueSessionId == id)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            this.RemoveAt(i);
                        });
                        break;
                    }
                }
            }
        }

        public void RefreshList()
        {
            if (m_RefreshThread.IsBusy)
                return; // let thread finish before rerunning.

            if (this.WTSServerHostnames == null)
            {
                return;
            }
            this.m_RefreshThread.RunWorkerAsync();
        }

        public int AddOrUpdate(WTSSession _session)
        {
            int ndx = this.IndexOfUniqueSessionId(_session.UniqueSessionId);
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (ndx == -1)
                {
                    this.Add(_session);
                }
                else
                {
                    this[ndx] = _session;
                }
            });
            return ndx;
        }

        public void Add(WTSSession[] _SessionArray, bool _ClearBeforeAdd = false)
        {
            if (_ClearBeforeAdd)
                this.Clear();
            for(int i=0; i < _SessionArray.Count(); i++)
            {
                this.Add(_SessionArray[i]);
            }
        }

        public WTSSessionList SessionsSortedByUser
        {
            get
            {
                return new WTSSessionList(from i in this orderby i.Username select i);
            }
        }
        public int IndexOfUniqueSessionId(WTSUniqueSessionId _UniqueSessionId)
        {
            for( int i = 0; i < this.Count; i++)
            {
                if(this[i].UniqueSessionId == _UniqueSessionId)
                {
                    return i;
                }
            }
            return -1; // non found 
        }

        //public override event NotifyCollectionChangedEventHandler CollectionChanged;
        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    var eh = CollectionChanged;
        //    if (eh != null)
        //    {

        //        Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
        //                                 let dpo = nh.Target as DispatcherObject
        //                                 where dpo != null
        //                                 select dpo.Dispatcher).FirstOrDefault();

        //        if (dispatcher != null && dispatcher.CheckAccess() == false)
        //        {
        //            dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
        //        }
        //        else
        //        {
        //            foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
        //                nh.Invoke(this, e);
        //        }
        //    }
        //}

        public WTSSession this[WTSUniqueSessionId _UniqueSessionId]
        {
            get
            {
                foreach (WTSSession s in this)
                {
                    if (s.UniqueSessionId == _UniqueSessionId)
                        return (s);
                }
                return null;
            }
        }

        public bool LogoffAllSessionsForUser(string _Username)
        {
            bool rtn = true;
            string username_lc = _Username.ToLower();
            WTSSessionList killlist = new WTSSessionList();

            string msg = "Are you sure want to logoff these sessions?" + Environment.NewLine + Environment.NewLine;
            foreach(WTSSession s in this)
            {
                if(s.Username.ToLower() == username_lc)
                {
                    msg += "   " + s.Hostname + ", " + s.Username + " (" + s.SessionId + ");" + Environment.NewLine;
                    killlist.Add(s);
                }
            }
            MessageBoxResult result = System.Windows.MessageBox.Show(msg, 
                "Confirm Logoff", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                foreach(WTSSession s_kill in killlist)
                {
                    if(s_kill.LogoffSession() == false)
                    {
                        rtn = false;
                    }
                }
            }
            this.RefreshList();
            return rtn;
        }

        public bool LogoffSessions(WTSSessionList _Sessions)
        {
            bool rtn = true;

            string msg = "Are you sure want to logoff these sessions?" + Environment.NewLine + Environment.NewLine;
            foreach (WTSSession s in _Sessions)
            {
                msg += "   " + s.Hostname + ", " + s.Username + " (" + s.SessionId + ");" + Environment.NewLine;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show(msg,
                "Confirm Logoff", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (WTSSession s_kill in _Sessions)
                {
                    if (s_kill.LogoffSession() == false)
                    {
                        rtn = false;
                    }
                }
            }
            this.RefreshList();
            return rtn;
        }
    }

}
