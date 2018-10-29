using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Jrfc;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using Jrfc.Wts;
using Jrfc.Rds;
using System.Collections.Specialized;
using System.Windows.Media;

namespace RdsClient
{
    public partial class MainWindow : Window
    {
        //private void uc_RdsEx_AppMenu_AddToFavorites_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem mnu = (MenuItem)sender;

        //    foreach (RdsApp app in this.uc_RemoteApps.SelectedItems)
        //    {
        //        if (this.m_FavoriteRdsAppList[app.RegistryFullKeyString] == null)
        //        {
        //            this.m_FavoriteRdsAppList.Add(app);
        //        }
        //    }
        //}

        //private void uc_RemoteApp_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    ContextMenu mnu = (ContextMenu)FindResource("uc_RemoteAppContextMenu");
        //    Visibility visibility_fav = Visibility.Visible;
        //    Visibility visibility = Visibility.Visible;
        //    if (mnu == null)
        //        return;

        //    ListView lv = (ListView)sender;
        //    string[] ItemHeaders_fav = new string[1] { "Add to Favorites" };
        //    if (lv.Name == "uc_FavoriteApps")
        //    {
        //        visibility_fav = Visibility.Collapsed;
        //    }
        //    Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeaders_fav, visibility_fav);

        //    string[] ItemHeaders = new string[2] { "Pin to Taskbar", "Pin to Start" };
        //    if (e.OriginalSource is TextBlock)
        //    {
        //        TextBlock tb = (TextBlock)e.OriginalSource;
        //        if (this.m_RdsServerList[tb.Text] != null) // Opening ContextMenu on a Server
        //        {
        //            visibility = Visibility.Collapsed;
        //        }
        //        Jrfc.Utility.SetMenuItemsVisibility(mnu, ItemHeaders, visibility);
        //    }
        //}
        //private void uc_RdsEx_ServerGroupExpander_Expanded(object sender, RoutedEventArgs e)
        //{
        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();

        //    //Loaded event is fired earlier than the Click event, so I'm sure that the dictionary contains the key
        //    if (Properties.Settings.Default.RDSExplorerTab_ExpandedServers == null)
        //        Properties.Settings.Default.RDSExplorerTab_ExpandedServers = new System.Collections.Specialized.StringCollection();
        //    if (Properties.Settings.Default.RDSExplorerTab_ExpandedServers.IndexOf(Hostname) < 0)
        //    {
        //        Properties.Settings.Default.RDSExplorerTab_ExpandedServers.Add(Hostname);
        //    }
        //}

        //private void uc_RdsEx_ServerGroupExpander_Collapsed(object sender, RoutedEventArgs e)
        //{
        //    if (Properties.Settings.Default.RDSExplorerTab_ExpandedServers == null)
        //        return;

        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();
        //    Properties.Settings.Default.RDSExplorerTab_ExpandedServers.Remove(Hostname);
        //}

        //private void uc_RdsEx_ServerGroupExpander_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (Properties.Settings.Default.RDSExplorerTab_ExpandedServers == null)
        //        return;

        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();
        //    if (Properties.Settings.Default.RDSExplorerTab_ExpandedServers.IndexOf(Hostname) >= 0)
        //    {
        //        exp.IsExpanded = true;
        //    }
        //    else
        //    {
        //        exp.IsExpanded = false;
        //    }
        //}
        //private void uc_RdsExplorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    this.EventHandler_AppLaunch((DependencyObject)e.OriginalSource);
        //}

        //private void uc_RdsExplorer_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return)
        //    {
        //        this.EventHandler_AppLaunch((DependencyObject)e.OriginalSource);
        //    }
        //}

    }
}
