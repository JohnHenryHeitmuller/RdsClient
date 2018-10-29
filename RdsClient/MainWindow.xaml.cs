using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Jrfc.Rds;
using Microsoft.WindowsAPICodePack.Taskbar;
using Jrfc.Wts;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Markup;

namespace RdsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Data Members
        private enum MainTabs
        {
            RdsExplorer = 0,
            Sessions = 1,
            Users = 2
        }
        public Jrfc.RdsExplorerContainer uc_RdsExplorerContainer { get; set; }
        public Jrfc.Rds.RdsServerList m_RdsServerList {get; set;}
        public Jrfc.Rds.RdsAppList m_CompositeRdsAppList { get; set; }
        public Jrfc.Rds.RdsAppList m_FavoriteRdsAppList { get; set; }
        public Jrfc.Wts.WTSSessionList m_WtsSessionList { get; set; }
        public Jrfc.Wts.WTSSessionList m_WtsUserList { get; set; }
        public Jrfc.Wts.WTSProcessList m_WtsSessionProcessList { get; set; }
        public Jrfc.Wts.WTSProcessList m_WtsUserProcessList { get; set; }
        public JumpList m_JumpList = null;
        
        #endregion

        #region MainWindow Methods and Event Handlers
        public MainWindow()
        {
            InitializeComponent();
            InitializeAppSettingMetadataDictionary();

        }
        private void uc_RdsClientMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_RdsServerList = new RdsServerList(true);
            this.m_CompositeRdsAppList = new RdsAppList(this.m_RdsServerList);
            this.m_FavoriteRdsAppList = new RdsAppList();

            this.DataContext = this;

            ///* Setup uc_RdsExplorer
            // */
            //this.uc_RemoteApps.ItemsSource = this.m_CompositeRdsAppList;
            //CollectionView view_ex = (CollectionView)CollectionViewSource.GetDefaultView(this.uc_RemoteApps.ItemsSource);
            //PropertyGroupDescription groupDescription_ex = new PropertyGroupDescription("Hostname");
            //view_ex.GroupDescriptions.Add(groupDescription_ex);

            ///* Setup uc_FavoriteApps
            // */
            //this.uc_FavoriteApps.ItemsSource = this.m_FavoriteRdsAppList;
            //CollectionView view_fav = (CollectionView)CollectionViewSource.GetDefaultView(this.uc_FavoriteApps.ItemsSource);
            //PropertyGroupDescription groupDescription_fav = new PropertyGroupDescription("Hostname");
            //view_fav.GroupDescriptions.Add(groupDescription_fav);

            this.LoadUserSettings();

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.RdsExplorerState))
            {
                this.uc_RdsExplorerContainer = new Jrfc.RdsExplorerContainer();
            }
            else
            {
                try
                {
                    this.uc_RdsExplorerContainer = Jrfc.RdsExplorerContainer.CreatFromXmlDictionaryString(Properties.Settings.Default.RdsExplorerState);
                }
                catch (System.Exception x)
                {
                    Jrfc.Exception.HandleException(x);
                }
            }
            TabItem RdsExplorerTab = Jrfc.Utility.FindTabByHeader(this.uc_MainTabControl, "RDS Explorer");
            RdsExplorerTab.Content = this.uc_RdsExplorerContainer;
            this.uc_RdsExplorerContainer.AllAppsRdsServerList = this.m_RdsServerList;


            //this.m_FavoriteRdsAppList.CollectionChanged += M_FavoriteRdsAppList_CollectionChanged;

            /* Setup uc_WtsSessionList
             */
            this.m_WtsSessionList = new Jrfc.Wts.WTSSessionList(Properties.Settings.Default.KnownRdsServers);
            this.m_WtsSessionList.CollectionChanged += new NotifyCollectionChangedEventHandler(uc_Sessions_CollectionChanged);
            this.uc_Sessions.ItemsSource = this.m_WtsSessionList;

            CollectionView view_sessions = (CollectionView)CollectionViewSource.GetDefaultView(this.uc_Sessions.ItemsSource);
            PropertyGroupDescription groupDescription_sessions = new PropertyGroupDescription("Hostname");
            view_sessions.GroupDescriptions.Add(groupDescription_sessions);

            /* Setup uc_WtsProcessList
             */
            this.m_WtsSessionProcessList = new Jrfc.Wts.WTSProcessList();
            this.uc_SessionProcesses.ItemsSource = this.m_WtsSessionProcessList;

            this.m_WtsUserList = new Jrfc.Wts.WTSSessionList();
            this.uc_Users.ItemsSource = this.m_WtsUserList;
            this.m_WtsUserProcessList = new Jrfc.Wts.WTSProcessList();
            this.uc_UserProcesses.ItemsSource = this.m_WtsUserProcessList;

            Jrfc.MruList m_MruList = Jrfc.MruList.CreateRemoteHostMruListByExt("misrds.jrfcorp.net", "exe");
            // m_MruList.LoadRemoteAppMruList("misrds.jrfcorp.net", "");

            this.SetJumpListItemsMaximumIfNeeded();
        }

        private void InitializeAppSettingMetadataDictionary()
        {
            Jrfc.Settings.AppSettingMetadataDictionary asmd = new Jrfc.Settings.AppSettingMetadataDictionary();
            
            asmd.Add("MainWindow_Size", "Remembered Settings", "Remembered size of RDS Client Main Window");
            asmd.Add("MainWindow_SelectedTabIndex", "Remembered Settings", "Remembered Active Tab in the RDS Client Main Window");
            asmd.Add("RDSExplorerTab_ExpandedServers", "Remembered Settings", "Remembered exanded/colapsed state of servers on the RDS Explorer tab");
            asmd.Add("FavoritesTab_AppList", "Remembered Settings", "Remembered Registry Keys of Apps on the user's favorites list");
            asmd.Add("FavoritesTab_ExpandedServers", "Remembered Settings", "Remembered exanded/colapsed state of servers on Favorite Apps tab");
            asmd.Add("SessionsTab_ExpandedServers", "Remembered Settings", "Remembered exanded/colapsed state of servers on the Sessions tab");

            asmd.Add("ExternalTool_RdsAddConnectionCmd", "Application Metadata", "Command line to launch the Control Panel RDS 'Access RemoteApp and desktops' wizard");
            asmd.Add("ExternalTool_RdsManageConnectionsCmd", "Application Metadata", "Command line to launch the Control Panel 'RemoteApp and Desktop Connections' applet");
            asmd.Add("RdsExplorerState", "Application Metadata", "*** DO NOT EDIT THIS *** State of the RDS Explorer Tab saved in XML");

            asmd.Add("KnownRdsServers", "User Modifiable", "List of 'Known' RDS servers to query for user sessions and processes");
            asmd.Add("ADDomains", "User Modifiable", "List of Active Directory Domains (';' delimited)");
            asmd.Add("ADAdminGroups", "User Modifiable", "List of Active Directory admin groups. Members of these groups will have access to RDS Client admin features (';' delimited)");
            asmd.Add("JumpListItems_Maximum", "User Modifiable", "Maximum number of items 'Favorite' app that can be displayed in Windows Taskbar icon for RdsClient");
            asmd.Add("RdsExplorerTab_MaxNameLength", "User Modifiable", "The maximum length of the name that a user can create for a new RDS Explorer tab");
            asmd.ApplyAppSettingMetadataDictionary(Properties.Settings.Default);
        }

        private void SetJumpListItemsMaximumIfNeeded()
        {
            string regkeyname = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
            string regvalname = "JumpListItems_Maximum";
            object obj = Registry.GetValue(regkeyname, regvalname, 0);
            int reg_value;
            bool reg_value_okay = int.TryParse(obj.ToString(), out reg_value);
            if(reg_value_okay)
            {
                if (reg_value > Properties.Settings.Default.JumpListItems_Maximum)
                {
                    // if current registry value for JumpListItems_Maximum is greater than
                    //  Properties.Settings.Default.JumpListItems_Maximum then do not update
                    return;
                }
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JumpListItems_Maximum", Properties.Settings.Default.JumpListItems_Maximum);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.RefreshJumpList();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveUserSettings();
        }

        private void RefreshServerAndAppList()
        {
            this.m_RdsServerList.Clear();
            this.m_CompositeRdsAppList.Clear();

            this.m_RdsServerList.RefreshList();
            this.m_CompositeRdsAppList.RefreshList();
        }
        #endregion

        #region Load/Save User Setting
        private void LoadUserSettings()
        {
            RdsClient.Properties.Settings d = Properties.Settings.Default;

            this.Width = (d.MainWindow_Size.Width >= this.MinWidth ? d.MainWindow_Size.Width : this.MinWidth);
            this.Height = (d.MainWindow_Size.Height >= this.MinHeight ? d.MainWindow_Size.Height : this.MinHeight);
            this.uc_MainTabControl.SelectedIndex = d.MainWindow_SelectedTabIndex;
            //this.uc_RdsExplorerContainer.MaxTabNameLength = Properties.Settings.Default.RdsExplorerTab_MaxNameLength;

            //if(d.FavoritesTab_AppList != null && this.m_CompositeRdsAppList != null)
            //{
            //    RdsAppList fav_apps = new RdsAppList(d.FavoritesTab_AppList, this.m_CompositeRdsAppList);
            //    if (this.m_FavoriteRdsAppList == null)
            //    {
            //        this.m_FavoriteRdsAppList = fav_apps;
            //    }
            //    else
            //    {
            //        this.m_FavoriteRdsAppList.Clear();
            //        foreach (RdsApp app in fav_apps)
            //        {
            //            this.m_FavoriteRdsAppList.Add(app);
            //        }
            //    }
            //}
        }
        private void SaveUserSettings()
        {
            RdsClient.Properties.Settings d = Properties.Settings.Default;

            d.MainWindow_Size = new System.Drawing.Size((int)this.Width, (int)this.Height);
            d.MainWindow_SelectedTabIndex = this.uc_MainTabControl.SelectedIndex;
            d.FavoritesTab_AppList = this.m_FavoriteRdsAppList.ToStringCollection();

            try
            {
                d.RdsExplorerState = this.uc_RdsExplorerContainer.ToXmlDictionaryString();
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            Properties.Settings.Default.Save();
        }

        private void ResetUserSettings()
        {
            RdsClient.Properties.Settings d = Properties.Settings.Default;

            d.MainWindow_Size = new System.Drawing.Size((int)this.MinWidth, (int)this.MinHeight);
            d.MainWindow_SelectedTabIndex = 0;
            d.RDSExplorerTab_ExpandedServers.Clear();
            Properties.Settings.Default.Save();
            this.LoadUserSettings();
        }

        private void RefreshJumpList()
        {
            if (m_FavoriteRdsAppList != null)
            {
                this.m_JumpList = JumpList.CreateJumpList();

                RdsServerIdList hosts = this.m_FavoriteRdsAppList.RdsServerIds;
                foreach (RdsServerId id in hosts)
                {
                    JumpListCustomCategory category = new JumpListCustomCategory(id.Hostname);
                    Dictionary<string, string> apps = this.m_FavoriteRdsAppList.GetAppLinksAndDisplayNamesForHost(id.Hostname);
                    foreach (KeyValuePair<string, string> app in apps)
                    {
                        JumpListLink link = new JumpListLink(app.Key, app.Value);
                        category.AddJumpListItems(link);
                    }
                    this.m_JumpList.AddCustomCategories(category);
                }
                this.m_JumpList.Refresh();
            }
        }
        #endregion

        //private void EventHandler_AppLaunch(DependencyObject originalSource)
        //{
        //    //DependencyObject originalSource = (DependencyObject)e.OriginalSource;
        //    while ((originalSource != null) && !(originalSource is ListViewItem))
        //    {
        //        originalSource = VisualTreeHelper.GetParent(originalSource);
        //    }
        //    //if it didn’t find a ListViewItem anywhere in the hierarch, it’s because the user
        //    //didn’t click on one. Therefore, if the variable isn’t null, run the code
        //    if (originalSource != null)
        //    {
        //        RdsApp app = (RdsApp)((System.Windows.FrameworkElement)originalSource).DataContext;
        //        app.Launch();
        //    }
        //}

        private void uc_MenuItem_ManageRemoteApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Process.Start("xwizard.exe", Properties.Settings.Default.ExternalTool_RdsManageConnectionsCmd);
                Process.Start("control.exe", @"/name Microsoft.RemoteAppAndDesktopConnections");
            }
            catch (System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }

        private void uc_MenuItem_AddRemoteAppConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("xwizard.exe", Properties.Settings.Default.ExternalTool_RdsAddConnectionCmd);
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
        }

        private void uc_MenuItem_Preferences_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow pref_win = new PreferencesWindow();
            
            pref_win.ShowDialog();
        }
        private void uc_MenuItem_ClearUserRdsConnections_Click(object sender, RoutedEventArgs e)
        {
            if(this.m_WtsSessionList != null)
            {
                this.m_WtsSessionList.LogoffAllSessionsForUser(Environment.UserName);
            }
        }

        private void uc_MenuItem_SaveAppState_Click(object sender, RoutedEventArgs e)
        {
            this.SaveUserSettings();
        }

        private void uc_MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow(this);
            win.ShowDialog();
        }

        private void uc_MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow(this);
            win.ShowDialog();
        }

        private void uc_MenuItem_DiagnoticAndSystemInfo_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow(this);
            win.ShowDialog();
        }

        private void uc_MenuItem_ExitWithoutSaving_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow(this);
            win.ShowDialog();
        }

        private void uc_MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
