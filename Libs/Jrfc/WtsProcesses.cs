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


namespace Jrfc.Wts
{
    public class WTSProcess
    {
        public string Hostname { get; set; }
        public WTSProcess(string _Hostname, Jrfc.NativeMethods.WTS_PROCESS_INFO _pi)
        {
            this.Hostname = _Hostname;
            this.SessionId = _pi.SessionID;
            this.ProcessId = _pi.ProcessID;
            this.ProcessName = (string)Marshal.PtrToStringAnsi(_pi.ProcessName);
        }

        public int SessionId { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }

        public bool TerminateProcess()
        {
            return TerminateProcess(this.Hostname, this.ProcessId);
        }

        public static bool TerminateProcess(string _Hostname, int _ProcessId)
        {
            IntPtr hServer = IntPtr.Zero;
            bool rtn = false;
            try
            {
                hServer = Jrfc.NativeMethods.WTSOpenServer(_Hostname);
                rtn = NativeMethods.WTSTerminateProcess(hServer, _ProcessId, (int)NativeMethods.WINDOWS_ERROR_CODES.PROCESS_TERMINATED_UNEXPECTEDLY);
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
            return rtn;
        }
    }

    public class WTSProcessList : ObservableCollection<WTSProcess>
    {
        public string Hostname { get; set; }
        public int SessionId { get; set; }

        public WTSProcessList()
        {
            this.Hostname = string.Empty;
            this.SessionId = WTSSession.SessionIdNull;
        }
        public WTSProcessList(string _Hostname, int _SessionId)
        {
            this.Hostname = _Hostname;
            this.SessionId = _SessionId;
            this.RefreshList(_Hostname, SessionId);
        }

        public void RefreshList(string _Hostname, int _SessionId)
        {
            IntPtr hServer = IntPtr.Zero;
            try
            {
                //IntPtr SessionInfoPtr = IntPtr.Zero;
                NativeMethods.WTS_PROCESS_INFO[] processInfos = null;
                this.Clear();
                hServer = Jrfc.NativeMethods.WTSOpenServer(_Hostname);
                IntPtr pProcessInfo = IntPtr.Zero;
                int processCount = 0;

                if(NativeMethods.WTSEnumerateProcesses(hServer, 0, 1, ref pProcessInfo, ref processCount))
                {
                    IntPtr pMemory = pProcessInfo;
                    processInfos = new NativeMethods.WTS_PROCESS_INFO[processCount];
                    for (int i = 0; i < processCount; i++)
                    {
                        processInfos[i] = (NativeMethods.WTS_PROCESS_INFO)Marshal.PtrToStructure(pProcessInfo, typeof(NativeMethods.WTS_PROCESS_INFO));
                        pProcessInfo = (IntPtr)((int)pProcessInfo + Marshal.SizeOf(processInfos[i]));
                    }
                    NativeMethods.WTSFreeMemory(pMemory);
                }
                if(processInfos != null)
                {
                    foreach (NativeMethods.WTS_PROCESS_INFO pi in processInfos)
                    {
                        if (pi.SessionID == _SessionId)
                        {
                            WTSProcess p = new WTSProcess(_Hostname, pi);
                            this.Add(p);
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
                if (hServer != IntPtr.Zero)
                    Jrfc.NativeMethods.WTSCloseServer(hServer);
            }
        }
    }
}
