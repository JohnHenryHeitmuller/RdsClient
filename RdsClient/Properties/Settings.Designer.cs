﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RdsClient.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Remembered size of RDS Client Main Window
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered size of RDS Client Main Window")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0, 0")]
        public global::System.Drawing.Size MainWindow_Size {
            get {
                return ((global::System.Drawing.Size)(this["MainWindow_Size"]));
            }
            set {
                this["MainWindow_Size"] = value;
            }
        }
        
        /// <summary>
        /// Remembered Active Tab in the RDS Client Main Window
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered Active Tab in the RDS Client Main Window")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int MainWindow_SelectedTabIndex {
            get {
                return ((int)(this["MainWindow_SelectedTabIndex"]));
            }
            set {
                this["MainWindow_SelectedTabIndex"] = value;
            }
        }
        
        /// <summary>
        /// Remembered exanded/colapsed state of servers on the RDS Explorer tab
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered exanded/colapsed state of servers on the RDS Explorer tab")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection RDSExplorerTab_ExpandedServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RDSExplorerTab_ExpandedServers"]));
            }
            set {
                this["RDSExplorerTab_ExpandedServers"] = value;
            }
        }
        
        /// <summary>
        /// Remembered Registry Keys of Apps on the user&apos;s favorites list
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered Registry Keys of Apps on the user\'s favorites list")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection FavoritesTab_AppList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FavoritesTab_AppList"]));
            }
            set {
                this["FavoritesTab_AppList"] = value;
            }
        }
        
        /// <summary>
        /// Remembered exanded/colapsed state of servers on Favorite Apps tab
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered exanded/colapsed state of servers on Favorite Apps tab")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection FavoritesTab_ExpandedServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FavoritesTab_ExpandedServers"]));
            }
            set {
                this["FavoritesTab_ExpandedServers"] = value;
            }
        }
        
        /// <summary>
        /// Command line to launch the Control Panel RDS &quot;Access RemoteApp and desktops&quot; wizard
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Command line to launch the Control Panel RDS \"Access RemoteApp and desktops\" wiza" +
            "rd")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("RunWizard {7940ACF8-60BA-4213-A7C3-F3B400EE266D}")]
        public string ExternalTool_RdsAddConnectionCmd {
            get {
                return ((string)(this["ExternalTool_RdsAddConnectionCmd"]));
            }
        }
        
        /// <summary>
        /// Command line to launch the Control Panel &quot;RemoteApp and Desktop Connections&quot; applet
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Command line to launch the Control Panel \"RemoteApp and Desktop Connections\" appl" +
            "et")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("RunWizard {241D7C96-F8BF-4F85-B01F-E2B043341A4B}")]
        public string ExternalTool_RdsManageConnectionsCmd {
            get {
                return ((string)(this["ExternalTool_RdsManageConnectionsCmd"]));
            }
        }
        
        /// <summary>
        /// Remembered exanded/colapsed state of servers on the Sessions tab
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Remembered exanded/colapsed state of servers on the Sessions tab")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection SessionsTab_ExpandedServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["SessionsTab_ExpandedServers"]));
            }
            set {
                this["SessionsTab_ExpandedServers"] = value;
            }
        }
        
        /// <summary>
        /// List of &quot;Known&quot; RDS servers to query for user sessions and processes
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("List of \"Known\" RDS servers to query for user sessions and processes")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>accounting.jrfcorp.net</string>
  <string>jrv0-tfs.jrfcorp.net</string>
  <string>saunders.jrfcorp.net</string>
  <string>misrds.jrfcorp.net</string>
  <string>stamper.jrfcorp.net</string>
  <string>remote.jrfcorp.net</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection KnownRdsServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["KnownRdsServers"]));
            }
            set {
                this["KnownRdsServers"] = value;
            }
        }
        
        /// <summary>
        /// List of Active Directory Domains (&apos;;&apos; delimited)
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("List of Active Directory Domains (\';\' delimited)")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("jrcc.local")]
        public string ADDomains {
            get {
                return ((string)(this["ADDomains"]));
            }
        }
        
        /// <summary>
        /// List of Active Directory admin groups. Members of these groups will have access to RDS Client admin features (&apos;;&apos; delimited)
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("List of Active Directory admin groups. Members of these groups will have access t" +
            "o RDS Client admin features (\';\' delimited)")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Domain Admins")]
        public string ADAdminGroups {
            get {
                return ((string)(this["ADAdminGroups"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int JumpListItems_Maximum {
            get {
                return ((int)(this["JumpListItems_Maximum"]));
            }
            set {
                this["JumpListItems_Maximum"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int RdsExplorerTab_MaxNameLength {
            get {
                return ((int)(this["RdsExplorerTab_MaxNameLength"]));
            }
            set {
                this["RdsExplorerTab_MaxNameLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string RdsExplorerState {
            get {
                return ((string)(this["RdsExplorerState"]));
            }
            set {
                this["RdsExplorerState"] = value;
            }
        }
    }
}