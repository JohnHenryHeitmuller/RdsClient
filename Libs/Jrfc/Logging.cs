using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jrfc
{
    public class Logging
    {
        public static string WindowsEventLog_SourceName
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            }
            set { WindowsEventLog_SourceName = value; }
        }
        public static string WindowsEventLog_LogName
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Name+"Log";
            }
            set { WindowsEventLog_LogName = value; }
        }

        public enum EventLogWriteResult
        {
            Success,
            ArgumentException,
            InvalidOperationException,
            Win32Exception,
            OtherException
        }

        public static System.Exception WriteToWindowsEventLog(string _Message)
        {
            // return null on success

            System.Exception result = null;
            try
            {
                System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                appLog.Source = WindowsEventLog_SourceName;
                appLog.Log = WindowsEventLog_LogName;
                appLog.WriteEntry(_Message);
            }
            catch(ArgumentException ax)
            {
                result = ax;
            }
            catch(InvalidOperationException ioe)
            {
                result = ioe;
            }
            catch(Win32Exception w32e )
            {
                result = w32e;
            }
            catch(System.Exception x)
            {
                result = x;
            }
            return result;
        }
    }
}
