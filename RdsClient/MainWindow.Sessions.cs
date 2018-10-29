using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Jrfc;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using Jrfc.Wts;

using System.Windows.Media;

namespace RdsClient
{
    public partial class MainWindow : Window
    {
        private void uc_Sessions_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_WtsSessionList.RefreshList();
            WTSSession[] proc_array = this.m_WtsSessionList.SessionsSortedByUser.ToArray();
            this.m_WtsUserList.Add(proc_array, true);
        }

        private void uc_Sessions_ServerGroupExpander_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SessionsTab_ExpandedServers == null)
                return;

            Expander exp = (Expander)sender;
            CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
            string Hostname = (string)dc.Name.ToString();
            if (Properties.Settings.Default.SessionsTab_ExpandedServers.IndexOf(Hostname) >= 0)
            {
                exp.IsExpanded = true;
            }
            else
            {
                exp.IsExpanded = false;
            }
        }

        private void uc_Sessions_ServerGroupExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SessionsTab_ExpandedServers == null)
                return;

            Expander exp = (Expander)sender;
            CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
            string Hostname = (string)dc.Name.ToString();
            Properties.Settings.Default.SessionsTab_ExpandedServers.Remove(Hostname);
        }

        private void uc_Sessions_ServerGroupExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;
            CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
            string Hostname = (string)dc.Name.ToString();

            //Loaded event is fired earlier than the Click event, so I'm sure that the dictionary contains the key
            if (Properties.Settings.Default.SessionsTab_ExpandedServers == null)
                Properties.Settings.Default.SessionsTab_ExpandedServers = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.SessionsTab_ExpandedServers.IndexOf(Hostname) < 0)
            {
                Properties.Settings.Default.SessionsTab_ExpandedServers.Add(Hostname);
            }

        }

        private void uc_UserSessionsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_WtsSessionList.RefreshList();
            WTSSession[] proc_array = this.m_WtsSessionList.SessionsSortedByUser.ToArray();
            this.m_WtsUserList.Add(proc_array, true);
        }

        private void uc_SessionsShowProtocolChanged(object sender, RoutedEventArgs e)
        {
            //string text = "";

            //if (this.uc_SessionsShowRdp.IsChecked == true && this.uc_SessionsShowConsole.IsChecked == true &&
            //    this.uc_SessionsShowService.IsChecked == true && this.uc_SessionsShowOtherProtocol.IsChecked == true)
            //{
            //    text = "Show All";
            //}
            //else
            //{
            //    if (this.uc_SessionsShowRdp.IsChecked == true)
            //    {
            //        text = "RDP";
            //    }
            //    if (this.uc_SessionsShowConsole.IsChecked == true)
            //    {
            //        if (text.Length > 0)
            //            text += ", ";
            //        text += "Console";
            //    }
            //    if (this.uc_SessionsShowService.IsChecked == true)
            //    {
            //        if (text.Length > 0)
            //            text += ", ";
            //        text += "Service";
            //    }
            //    if (this.uc_SessionsShowOtherProtocol.IsChecked == true)
            //    {
            //        if (text.Length > 0)
            //            text += ", ";
            //        text += "Other";
            //    }
            //}
            //this.uc_SessionsFilterProtocol.Text = text;
        }

        private void uc_SessionsShowStateChanged(object sender, RoutedEventArgs e)
        {
            //string text = "";

            //if (this.uc_SessionsShowActive.IsChecked == true && this.uc_SessionsShowDiconnected.IsChecked == true &&
            //    this.uc_SessionsShowOtherStates.IsChecked == true)
            //{
            //    text = "Show All";
            //}
            //else
            //{
            //    if (this.uc_SessionsShowActive.IsChecked == true)
            //    {
            //        text = "Active";
            //    }
            //    if (this.uc_SessionsShowDiconnected.IsChecked == true)
            //    {
            //        if (text.Length > 0)
            //            text += ", ";
            //        text += "Diconnected";
            //    }
            //    if (this.uc_SessionsShowOtherStates.IsChecked == true)
            //    {
            //        if (text.Length > 0)
            //            text += ", ";
            //        text += "Other";
            //    }
            //}
            //this.uc_SessionsFilterState.Text = text;
        }

        private void uc_Sessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.uc_Sessions.SelectedItem == null)
                return;
            WTSSession session = (WTSSession)this.uc_Sessions.SelectedItem;
            this.m_WtsSessionProcessList.RefreshList(session.Hostname, session.SessionId);
        }
        private void uc_SessionsContextMenu_Logoff_Click(object sender, RoutedEventArgs e)
        {
            WTSSessionList session_list = new WTSSessionList();
            if(this.uc_MainTabControl.SelectedIndex == (int)MainTabs.Sessions)
            {
                if (this.uc_Sessions.SelectedItems != null)
                {
                    foreach(object session in this.uc_Sessions.SelectedItems)
                    {
                        session_list.Add((WTSSession)session);
                    }
                }
            }
            else if(this.uc_MainTabControl.SelectedIndex == (int)MainTabs.Users)
            {
                if (this.uc_Users.SelectedItems != null)
                {
                    foreach (object session in this.uc_Users.SelectedItems)
                    {
                        session_list.Add((WTSSession)session);
                    }
                }
            }
            if(session_list != null)
            {            
                this.m_WtsSessionList.LogoffSessions(session_list);
            }
        }

        private void uc_SessionsContextMenu_OpenDesktopSession_Click(object sender, RoutedEventArgs e)
        {

        }

        private void uc_Sessions_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu mnu = (ContextMenu)FindResource("uc_SessionsContextMenu");
            if (mnu == null)
                return;

            string[] ItemHeadersServer = new string[1] { "Open Desktop session to server" };
            string[] ItemHeadersSession = new string[1] { "Logoff Session" };

            if (e.OriginalSource is TextBlock)
            {
                TextBlock tb = (TextBlock)e.OriginalSource;
                if (this.m_RdsServerList[tb.Text] != null) // Opening ContextMenu on a Server
                {
                    Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeadersServer, Visibility.Visible);
                    Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeadersSession, Visibility.Collapsed);
                }
                else
                {
                    Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeadersServer, Visibility.Collapsed);
                    Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeadersSession, Visibility.Visible);
                }
            }
        }
        private void uc_Sessions_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            //different kind of changes that may have occurred in collection
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    //your code
            //}
            //if (e.Action == NotifyCollectionChangedAction.Replace)
            //{
            //    //your code
            //}
            //if (e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    //your code
            //}
            //if (e.Action == NotifyCollectionChangedAction.Move)
            //{
            //    //your code
            //}
            WTSSession[] proc_array = this.m_WtsSessionList.SessionsSortedByUser.ToArray();
            this.m_WtsUserList.Add(proc_array, true);
        }

    }
}
