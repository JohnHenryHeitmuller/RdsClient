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

namespace RdsClient
{
    public partial class MainWindow : Window
    {
        private void uc_Users_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_WtsUserList.Add(this.m_WtsSessionList.SessionsSortedByUser.ToArray(), true);
        }

        private void uc_Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.uc_Users.SelectedItem == null)
                return;
            WTSSession session = (WTSSession)this.uc_Users.SelectedItem;
            this.m_WtsUserProcessList.RefreshList(session.Hostname, session.SessionId);

        }


    }
}
