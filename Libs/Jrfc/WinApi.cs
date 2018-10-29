using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Shell32;

namespace Jrfc
{
    public class NativeMethods
    {
        #region Windows Terminal Services Functions
        /****************************************************************************************
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_ADDRESS
        {
            public ADDRESS_FAMILIES AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;
        }

        public enum ADDRESS_FAMILIES : short
        {
            // Unspecified [value = 0].
            AF_UNSPEC = 0,
            // Local to host (pipes, portals) [value = 1].
            AF_UNIX = 1,
            // Internetwork: UDP, TCP, etc [value = 2].
            AF_INET = 2,
            // Arpanet imp addresses [value = 3].
            AF_IMPLINK = 3,
            // Pup protocols: e.g. BSP [value = 4].
            AF_PUP = 4,
            // Mit CHAOS protocols [value = 5].
            AF_CHAOS = 5,
            // XEROX NS protocols [value = 6].
            AF_NS = 6,
            // IPX protocols: IPX, SPX, etc [value = 6].
            AF_IPX = 6,
            // ISO protocols [value = 7].
            AF_ISO = 7,
            // OSI is ISO [value = 7].
            AF_OSI = 7,
            // european computer manufacturers [value = 8].
            AF_ECMA = 8,
            // datakit protocols [value = 9].
            AF_DATAKIT = 9,
            // CCITT protocols, X.25 etc [value = 10].
            AF_CCITT = 10,
            // IBM SNA [value = 11].
            AF_SNA = 11,
            // DECnet [value = 12].
            AF_DECnet = 12,
            // Direct data link interface [value = 13].
            AF_DLI = 13,
            // LAT [value = 14].
            AF_LAT = 14,
            // NSC Hyperchannel [value = 15].
            AF_HYLINK = 15,
            // AppleTalk [value = 16].
            AF_APPLETALK = 16,
            // NetBios-style addresses [value = 17].
            AF_NETBIOS = 17,
            // VoiceView [value = 18].
            AF_VOICEVIEW = 18,
            // Protocols from Firefox [value = 19].
            AF_FIREFOX = 19,
            // Somebody is using this! [value = 20].
            AF_UNKNOWN1 = 20,
            // Banyan [value = 21].
            AF_BAN = 21,
            // Native ATM Services [value = 22].
            AF_ATM = 22,
            // Internetwork Version 6 [value = 23].
            AF_INET6 = 23,
            // Microsoft Wolfpack [value = 24].
            AF_CLUSTER = 24,
            // IEEE 1284.4 WG AF [value = 25].
            AF_12844 = 25,
            // IrDA [value = 26].
            AF_IRDA = 26,
            // Network Designers OSI &amp; gateway enabled protocols [value = 28].
            AF_NETDES = 28,
            // [value = 29].
            AF_TCNPROCESS = 29,
            // [value = 30].
            AF_TCNMESSAGE = 30,
            // [value = 31].
            AF_ICLFXBM = 31
        }

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive = 0,
            WTSConnected = 1,
            WTSConnectQuery = 2,
            WTSShadow = 3,
            WTSDisconnected = 4,
            WTSIdle = 5,
            WTSListen = 6,
            WTSReset = 7,
            WTSDown = 8,
            WTSInit = 9
        }

        public enum WINDOWS_ERROR_CODES : int
        {
            PROCESS_TERMINATED_UNEXPECTEDLY = 1067
        }

        public static string GetConnectStateString(WTS_CONNECTSTATE_CLASS state)
        {
            switch(state)
            {
                case WTS_CONNECTSTATE_CLASS.WTSActive:
                    return "Active";
                case WTS_CONNECTSTATE_CLASS.WTSConnected:
                    return "Connected";
                case WTS_CONNECTSTATE_CLASS.WTSConnectQuery:
                    return "Connect Query";
                case WTS_CONNECTSTATE_CLASS.WTSDisconnected:
                    return "Disconnected";
                case WTS_CONNECTSTATE_CLASS.WTSDown:
                    return "Down";
                case WTS_CONNECTSTATE_CLASS.WTSIdle:
                    return "Idle";
                case WTS_CONNECTSTATE_CLASS.WTSInit:
                    return "Init";
                case WTS_CONNECTSTATE_CLASS.WTSListen:
                    return "Listen";
                case WTS_CONNECTSTATE_CLASS.WTSReset:
                    return "Reset";
                case WTS_CONNECTSTATE_CLASS.WTSShadow:
                    return "Shadow";
            }
            return ""; // This should never happen.
        }
        
        // Contains values that indicate the type of session information to retrieve in a call to the <see cref="WTSQuerySessionInformation"/> function.
        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram = 0,  // A null-terminated string that contains the name of the initial program that Remote Desktop Services runs when the user logs on.
            WTSApplicationName = 1, // A null-terminated string that contains the published name of the application that the session is running.
            WTSWorkingDirectory = 2,// A null-terminated string that contains the default directory used when launching the initial program.
            WTSOEMId = 3,           // This value is not used.
            WTSSessionId = 4,       // A <B>ULONG</B> value that contains the session identifier.
            WTSUserName = 5,        // A null-terminated string that contains the name of the user associated with the session.
            WTSWinStationName = 6,  // A null-terminated string that contains the name of the Remote Desktop Services session. 
                                    //      <B>Note</B>  Despite its name, specifying this type does not return the window station name. 
                                    //      Rather, it returns the name of the Remote Desktop Services session. 
                                    //      Each Remote Desktop Services session is associated with an interactive window station. 
                                    //      Because the only supported window station name for an interactive window station is "WinSta0", 
                                    //      each session is associated with its own "WinSta0" window station. For more information, see 
                                    //      http://msdn.microsoft.com/en-us/library/windows/desktop/ms687096(v=vs.85).aspx">Window Stations
            WTSDomainName = 7,      // A null-terminated string that contains the name of the domain to which the logged-on user belongs.
            WTSConnectState = 8,    // The session's current connection state. For more information, see <see cref="WTS_CONNECTSTATE_CLASS"/>.
            WTSClientBuildNumber = 9,// A <B>ULONG</B> value that contains the build number of the client.
            WTSClientName = 10,     // A null-terminated string that contains the name of the client.
            WTSClientDirectory = 11,// A null-terminated string that contains the directory in which the client is installed.
            WTSClientProductId = 12,// A <B>USHORT</B> client-specific product identifier.
            WTSClientHardwareId = 13,// A <B>ULONG</B> value that contains a client-specific hardware identifier. This option is reserved for future use. 
                                    //      <see cref="WTSQuerySessionInformation"/> will always return a value of 0.
            WTSClientAddress = 14,  // The network type and network address of the client. For more information, see <see cref="WTS_CLIENT_ADDRESS"/>.
                                    //      The IP address is offset by two bytes from the start of the <B>Address</B> member of the <see cref="WTS_CLIENT_ADDRESS"/> structure.</remarks>
            WTSClientDisplay = 15,  // Information about the display resolution of the client. For more information, see <see cref="WTS_CLIENT_DISPLAY"/>.
            WTSClientProtocolType = 16,// A USHORT value that specifies information about the protocol type for the session. This is one of the following values:<BR/>
                                    // 0 - The console session.<BR/>
                                    // 1 - This value is retained for legacy purposes.<BR/>
                                    // 2 - The RDP protocol.<BR/>
            WTSIdleTime = 17,       // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSLogonTime = 18,      // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSIncomingBytes = 19,  // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSOutgoingBytes = 20,  // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSIncomingFrames = 21, // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSOutgoingFrames = 22, // This value returns <B>FALSE</B>. If you call <see cref="GetLastError"/> to get extended error information, <B>GetLastError</B> returns <B>ERROR_NOT_SUPPORTED</B>.
                                    //      <B>Windows Server 2008, Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not used.
            WTSClientInfo = 23,     // Information about a Remote Desktop Connection (RDC) client. For more information, see <see cref="WTSCLIENT"/>.
                                    // <B>Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not supported. 
                                    // This value is supported beginning with Windows Server 2008 and Windows Vista with SP1.
            WTSSessionInfo = 24,    // Information about a client session on an RD Session Host server. For more information, see <see cref="WTSINFO"/>.
                                    // <B>Windows Vista, Windows Server 2003, and Windows XP:</B>  This value is not supported. 
                                    // This value is supported beginning with Windows Server 2008 and Windows Vista with SP1.
            WTSSessionInfoEx = 25,
            WTSConfigInfo = 26,
            WTSValidationInfo = 27,
            WTSSessionAddressV4 = 28,
            WTSIsRemoteSession = 29
        }

#pragma warning disable 0169
        public struct WTS_SESSION_INFO
        {
            public int SessionId;
            public IntPtr pWinStationName; //This is a pointer to string...
            public WTS_CONNECTSTATE_CLASS State;
        }
#pragma warning restore 0169

#pragma warning disable 0649
        public struct WTS_PROCESS_INFO
        {
            public int SessionID;
            public int ProcessID;
            public IntPtr ProcessName; //This is a pointer to string...
            public IntPtr UserSid;
        }
#pragma warning restore 0649

        [DllImport("wtsapi32.dll")]
        public static extern IntPtr WTSOpenServer(string pServerName);

        [DllImport("wtsapi32.dll")]
        public static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSEnumerateServers( String pDomainName, int Reserved, int Version, 
            ref IntPtr ppServerInfo, ref uint pCount );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSEnumerateSessions(System.IntPtr hServer, int Reserved, int Version,
            ref System.IntPtr ppSessionInfo, ref int pCount);

        //[DllImport("wtsapi32.dll", SetLastError = true)]
        //static extern void WTSEnumerateSessions( System.IntPtr hServer, ref System.IntPtr ppSessionInfo, ref int pCount)
        //{
        //    WTSEnumerateSessions(hServer, 0, 1, ppSessionInfo, pCount);
        //}

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSEnumerateProcesses(
            IntPtr serverHandle,        // Handle to a terminal server. 
            Int32 reserved,             // must be 0
            Int32 version,              // must be 1
            ref IntPtr ppProcessInfo,   // pointer to array of WTS_PROCESS_INFO
            ref Int32 pCount);          // pointer to number of processes

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);


        // The WTSQuerySessionInformation function retrieves session information for the specified 
        // session on the specified terminal server. 
        // It can be used to query session information on local and remote terminal servers.
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/termserv/termserv/wtsquerysessioninformation.asp
        //
        // <param name="hServer">Handle to a terminal server. Specify a handle opened by the WTSOpenServer function, 
        // or specify <see cref="WTS_CURRENT_SERVER_HANDLE"/> to indicate the terminal server on which your application is running.</param>
        // <param name="sessionId">A Terminal Services session identifier. To indicate the session in which the calling application is running 
        // (or the current session) specify <see cref="WTS_CURRENT_SESSION"/>. Only specify <see cref="WTS_CURRENT_SESSION"/> when obtaining session information on the 
        // local server. If it is specified when querying session information on a remote server, the returned session 
        // information will be inconsistent. Do not use the returned data in this situation.</param>
        // <param name="wtsInfoClass">Specifies the type of information to retrieve. This parameter can be one of the values from the <see cref="WTSInfoClass"/> enumeration type. </param>
        // <param name="ppBuffer">Pointer to a variable that receives a pointer to the requested information. The format and contents of the data depend on the information class specified in the <see cref="WTSInfoClass"/> parameter. 
        // To free the returned buffer, call the <see cref="WTSFreeMemory"/> function. </param>
        // <param name="pBytesReturned">Pointer to a variable that receives the size, in bytes, of the data returned in ppBuffer.</param>
        //
        // <returns>If the function succeeds, the return value is a nonzero value.
        // If the function fails, the return value is zero. To get extended error information, call GetLastError.
        // </returns>
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(System.IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, 
            out System.IntPtr ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        // The WTSFreeMemory function frees memory allocated by a Terminal Services function.
        public static extern void WTSFreeMemory(IntPtr memory);


        [DllImport("wtsapi32.dll", SetLastError = true)]
        // Retrieve the primary access token for the user associated with the specified session ID. 
        // The caller must be running in the context of the LocalSystem account and have the SE_TCB_NAME privilege.
        // Only highly trusted service should use this function. 
        // The application must not leak tokens, and close the token when it has finished using it.
        public static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        // Can be used to register a window to receive session change notifications, such as when the machine is locked or unlocked.
        public static extern bool WTSRegisterSessionNotification(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] int dwFlags);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        // The WTSSendMessage function displays a message box on the client desktop of a specified Terminal Services session.
        public static extern bool WTSSendMessage(
             IntPtr hServer,
             [MarshalAs(UnmanagedType.I4)] int SessionId,
             String pTitle,
             [MarshalAs(UnmanagedType.U4)] int TitleLength,
             String pMessage,
             [MarshalAs(UnmanagedType.U4)] int MessageLength,
             [MarshalAs(UnmanagedType.U4)] int Style,
             [MarshalAs(UnmanagedType.U4)] int Timeout,
             [MarshalAs(UnmanagedType.U4)] out int pResponse,
             bool bWait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        // The WTSShutdownSystem function shuts down (and optionally restarts) the specified terminal server.
        public static extern int WTSShutdownSystem(IntPtr ServerHandle, long ShutdownFlags);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSTerminateProcess(IntPtr hServer, int processId, int exitCode);

        // [DllImport("wtsapi32.dll", SetLastError = true)]
        // static extern TODO WTSUnRegisterSessionNotification(TODO);
        // [DllImport("wtsapi32.dll", SetLastError = true)]
        // static extern TODO WTSUnRegisterSessionNotification(TODO);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr WTSVirtualChannelOpen(IntPtr hServer, Int32 dwSessionID, IntPtr pChannelName);
        [DllImport("wtsapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WTSVirtualChannelClose(IntPtr hChannelHandle);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelRead(IntPtr channelHandle, byte[] buffer, int length, ref int bytesRead);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelWrite(IntPtr channelHandle, byte[] buffer, int length, ref int bytesWritten);

        #endregion

        #region Event Log Functions
        /****************************************************************************************
        */
        [DllImport("advapi32.dll")]
        public static extern IntPtr RegisterEventSource(string lpUNCServerName, string lpSourceName);
        
        [DllImport("advapi32.dll")]
        public static extern bool DeregisterEventSource(IntPtr hEventLog);
        
        [DllImport("advapi32.dll", EntryPoint = "ReportEventW", CharSet = CharSet.Unicode)]
        public static extern bool ReportEvent(
                    IntPtr hEventLog,
                    ushort wType,
                    ushort wCategory,
                    int dwEventID,
                    IntPtr lpUserSid,
                    ushort wNumStrings,
                    uint dwDataSize,
                    string[] lpStrings,
                    byte[] lpRawData
                    );

        public const ushort EVENTLOG_INFORMATION_TYPE = 0x0004;
        public const ushort EVENTLOG_WARNING_TYPE = 0x0002;
        public const ushort EVENTLOG_ERROR_TYPE = 0x0001;

        public static void WriteEventLog(string text, ushort logType, int logEventId, byte[] rawData)
        {
            //Temporary registry of eventsource
            string exeAssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            IntPtr hEventLog = RegisterEventSource(null, exeAssemblyName);

            uint dataSize = (uint)(rawData != null ? rawData.Length : 0);

            //Write event to eventlog
            ReportEvent(hEventLog, logType, 0, logEventId, IntPtr.Zero, 1, dataSize, new string[] { text }, rawData);

            //Remove temporary registration
            DeregisterEventSource(hEventLog);
        }
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr BeginUpdateResource(string pFileName, [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, ushort wLanguage, IntPtr lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("Netapi32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern int NetGetJoinInformation(string server, out IntPtr domain, out NetJoinStatus status);

        [DllImport("Netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        public enum NetJoinStatus
        {
            NetSetupUnknownStatus = 0,
            NetSetupUnjoined,
            NetSetupWorkgroupName,
            NetSetupDomainName
        }

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        //public enum SHELL32_STRING : uint
        //{
        //    SHELL32_STRING_ID_PIN_TO_TASKBAR = 5386,
        //    SHELL32_STRING_ID_PIN_TO_STARTMENU = 5381,
        //    SHELL32_STRING_ID_UNPIN_FROM_TASKBAR = 5387,
        //    SHELL32_STRING_ID_UNPIN_FROM_STARTMENU = 5382
        //}

        //public static bool TryGetVerbName(uint _id, out string _VerbName)
        //{
        //    bool rtn = false;
        //    StringBuilder buffer = new StringBuilder(255);
        //    _VerbName = "";

        //    IntPtr hdl_shell32 = LoadLibrary("Shell32.dll");
        //    if(hdl_shell32 != IntPtr.Zero)
        //    {
        //        try
        //        {
        //            int buflen = LoadString(hdl_shell32, _id, buffer, 255);
        //            if(buflen > 0)
        //            {
        //                _VerbName = buffer.ToString();
        //                rtn = true;
        //            }
        //        }
        //        finally
        //        {
        //            FreeLibrary(hdl_shell32);
        //        }
        //    }
        //    return rtn;
        //}

        //public static bool ExecVerb(string _FileName, string _VerbName)
        //{
        //    bool rtn = false;
        //    Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
        //    dynamic shell = Activator.CreateInstance(shellAppType);

        //    var folder = shell.NameSpace(Path.GetDirectoryName(_FileName));

        //    //Shell32.Folder folder = (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
        //    //    System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { Path.GetDirectoryName(_FileName) });


        //    Shell32.FolderItem folder_item = folder.ParseName(Path.GetFileName(_FileName));

        //    FolderItemVerbs verbs = folder_item.Verbs();

        //    for (int i=0; i < verbs.Count; i++)
        //    {
        //        FolderItemVerb verb = verbs.Item(i);
        //        if(verb.Name == _VerbName)
        //        {
        //            verb.DoIt();
        //            rtn = true;
        //        }
        //    }
        //    return rtn;
        //}

        //public static bool PinApp(string _FileName, SHELL32_STRING _PinTo)
        //{
        //    bool rtn = false;
        //    string VerbName;
        //    if( TryGetVerbName((uint)_PinTo, out VerbName ))
        //    {
        //        rtn = ExecVerb(_FileName, VerbName);
        //    }
        //    return rtn;
        //}
        //public static bool UnpinApp(string _FileName, SHELL32_STRING _PinTo)
        //{
        //    bool rtn = false;
        //    string VerbName;
        //    if (TryGetVerbName((uint)_PinTo, out VerbName))
        //    {
        //        rtn = ExecVerb(_FileName, VerbName);
        //    }
        //    return rtn;
        //}
    }
}
