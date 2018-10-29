using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;

namespace Jrfc
{
    public class Exception
    {
        public enum WriteToEventlog
        {
            Yes,
            No
        }

        public enum DisplayMessage
        {
            Yes,
            No
        }

        //public static class EventID
        //{
        //    public const ushort WTSEnumerateSessions = 2000;
        //    public const ushort RegistryAccess = 3000;
        //    public const ushort AD_ConvertExchangeAddressToSmtp = 3500;
        //    public const ushort AD_GetUserProperty = 3501;
        //    public const ushort AD_IsCurrentUserInGroup = 3502;
        //    public const ushort AD_GetUsersInGroup = 3503;

        //    public const ushort AppLaunchException = 4000;
        //    public const ushort ExternalTool_RdsAddConnectionCmd = 4001;
        //    public const ushort ExternalTool_RdsManageConnectionsCmd = 4002;
        //}

        public static void HandleException(System.Exception _x,
            EventLogEntryType _ExceptionType = EventLogEntryType.Error,
            DisplayMessage _DisplayMessage = DisplayMessage.Yes, 
            WriteToEventlog _WriteToEventLog = WriteToEventlog.Yes, byte[] _RawData = null)
        {
            StringBuilder sb_msg = new StringBuilder(_x.Message, 1024);
            sb_msg.AppendLine();
            sb_msg.AppendLine(_x.StackTrace);
            string exception_source = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            if (_x.InnerException != null)
            {
                sb_msg.AppendLine();
                sb_msg.AppendLine("Inner Exception: " + _x.InnerException.Message);
                sb_msg.AppendLine();
                sb_msg.AppendLine(_x.InnerException.StackTrace);
            }

            if (_WriteToEventLog == WriteToEventlog.Yes)
            {
                Jrfc.Logging.WriteToWindowsEventLog(sb_msg.ToString());
            }
            if(_DisplayMessage == DisplayMessage.Yes)
            {
                System.Windows.MessageBox.Show(sb_msg.ToString(), exception_source);
            }

            return;
        }
    }
}
