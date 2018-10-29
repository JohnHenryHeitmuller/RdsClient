using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Jrfc;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using Jrfc.Wts;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Input;

namespace RdsClient
{
    public partial class MainWindow : Window
    {
        //private void uc_RdsFav_ServerGroupExpander_Expanded(object sender, RoutedEventArgs e)
        //{
        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();

        //    //Loaded event is fired earlier than the Click event, so I'm sure that the dictionary contains the key
        //    if (Properties.Settings.Default.FavoritesTab_ExpandedServers == null)
        //        Properties.Settings.Default.FavoritesTab_ExpandedServers = new System.Collections.Specialized.StringCollection();
        //    if (Properties.Settings.Default.FavoritesTab_ExpandedServers.IndexOf(Hostname) < 0)
        //    {
        //        Properties.Settings.Default.FavoritesTab_ExpandedServers.Add(Hostname);
        //    }
        //}

        //private void uc_RdsFav_ServerGroupExpander_Collapsed(object sender, RoutedEventArgs e)
        //{
        //    if (Properties.Settings.Default.FavoritesTab_ExpandedServers == null)
        //        return;

        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();
        //    Properties.Settings.Default.FavoritesTab_ExpandedServers.Remove(Hostname);
        //}

        //private void uc_RdsFav_ServerGroupExpander_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (Properties.Settings.Default.FavoritesTab_ExpandedServers == null)
        //        return;

        //    Expander exp = (Expander)sender;
        //    CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
        //    string Hostname = (string)dc.Name.ToString();
        //    if (Properties.Settings.Default.FavoritesTab_ExpandedServers.IndexOf(Hostname) >= 0)
        //    {
        //        exp.IsExpanded = true;
        //    }
        //    else
        //    {
        //        exp.IsExpanded = false;
        //    }
        //}
        //private void uc_FavoriteApps_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    this.EventHandler_AppLaunch((DependencyObject)e.OriginalSource);
        //}

        //private void uc_FavoriteApps_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Return)
        //    {
        //        this.EventHandler_AppLaunch((DependencyObject)e.OriginalSource);
        //    }
        //}
        //private void M_FavoriteRdsAppList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    this.RefreshJumpList();
        //    this.SaveUserSettings();
        //}

    }
}
