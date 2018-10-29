using System;
using System.Collections.Generic;
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
using Jrfc.Rds;

namespace Jrfc
{
    /// <summary>
    /// Interaction logic for RdsExplorerContainer.xaml
    /// </summary>
    [Serializable()]
    public partial class RdsExplorerContainer : UserControl
    {
        //public Jrfc.Rds.RdsServerList m_RdsServerList { get; set; }
        //public Jrfc.Rds.RdsAppList m_CompositeRdsAppList { get; set; }
        public int MaxTabNameLength { get; set; }
        public RdsExplorerContainer()
        {
            InitializeComponent();
            this.MaxTabNameLength = 30; // default to 30, caller can override by setting this value

        }

        public RdsServerList AllAppsRdsServerList
        {
            get
            {
                RdsExplorerPanel p = this.GetRdsExplorerPanel("All Tabs");
                return p.ServerList;
            }
            set
            {
                RdsExplorerPanel p = this.GetRdsExplorerPanel("All Tabs");
                if (p != null)
                {
                    p.ServerList.Copy(value);
                    p.RefreshPanel();
                }
            }
        }

        public TabItem GetTabItem(int _index)
        {
            TabItem rtn = null;

            try
            {
                rtn = (TabItem)this.uc_MainTabControl.Items[_index];
            }
            catch(System.Exception x)
            {
                Jrfc.Exception.HandleException(x);
            }
            return rtn;
        }
        public TabItem GetTabItem(string _TabHeaderText)
        {
            string TabHeaderText_ToLower = _TabHeaderText.ToLower(); // case insensitive compare

            foreach(TabItem ti in this.uc_MainTabControl.Items)
            {
                if (ti.Header.ToString().ToLower() == TabHeaderText_ToLower)
                    return ti;
            }

            return null;
        }

        public RdsExplorerPanel GetRdsExplorerPanel(int _index)
        {
            TabItem ti = this.GetTabItem(_index);
            RdsExplorerPanel rep = this.GetRdsExplorerPanel(ti);
            return rep;
        }
        public RdsExplorerPanel GetRdsExplorerPanel(string _TabHeaderText)
        {
            TabItem ti = this.GetTabItem(_TabHeaderText);
            RdsExplorerPanel rep = this.GetRdsExplorerPanel(ti);
            return rep;
        }

        public RdsExplorerPanel GetRdsExplorerPanel(TabItem _TabItem)
        {
            if (_TabItem == null)
                return null;
            if (_TabItem.Content == null)
                return null;

            RdsExplorerPanel rtn = null;
            try
            {
                rtn = (RdsExplorerPanel)_TabItem.FindChildren<RdsExplorerPanel>().First<RdsExplorerPanel>();
            }
            catch(System.Exception x)
            {
#if DEBUG
                Jrfc.Exception.HandleException(x, System.Diagnostics.EventLogEntryType.Warning, Exception.DisplayMessage.Yes);
#else
                Jrfc.Exception.HandleException(x, System.Diagnostics.EventLogEntryType.Warning, Exception.DisplayMessage.No);
#endif
            }
            
            return rtn;
        }



        public string ToXmlDictionaryString()
        {
            XmlDictionary XmlList = new XmlDictionary();
            foreach (TabItem ti in this.uc_MainTabControl.Items)
            {
                if (ti.Header.ToString() != "+")
                {
                    Jrfc.RdsExplorerPanel p = this.GetRdsExplorerPanel(ti);
                    if (p != null)
                    {
                        XmlList.Add(ti.Header.ToString(), p.ToXmlDictionaryString());
                    }
                }
            }
            return XmlList.ToXmlString();
        }

        public static RdsExplorerContainer CreatFromXmlDictionaryString(string _xml)
        {
            XmlDictionary XmlList = Jrfc.Utility.ObjectSerializer<XmlDictionary>.FromXml(_xml);
            Jrfc.RdsExplorerContainer rec = new RdsExplorerContainer();
            foreach (XmlKeyValuePair kvp in XmlList)
            {
                Jrfc.RdsExplorerPanel rep = Jrfc.RdsExplorerPanel.CreatFromXmlDictionaryString(kvp.XmlString);
                TabItem plus_tab = Jrfc.Utility.FindTabByHeader(rec.uc_MainTabControl, "+");
                TabItem ti = null;
                bool new_tab = false;
                if (kvp.Key == "All Apps")
                {
                    ti = Jrfc.Utility.FindTabByHeader(rec.uc_MainTabControl, "All Apps");
                }
                if (ti == null)
                {   // if the "All Apps" TabItem was not found
                    ti = new TabItem();
                    new_tab = true;
                    ti.Header = kvp.Key;
                }
                ti.Content = rep;
                if (new_tab)
                {
                    if (plus_tab != null)
                    {
                        rec.uc_MainTabControl.Items.Insert(rec.uc_MainTabControl.Items.Count, ti);
                    }
                    else
                    {
                        rec.uc_MainTabControl.Items.Add(ti);
                    }
                }
            }
            return rec;
        }


        private void uc_MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private string PromptUserForNewTabName()
        {
            Window mywin = Window.GetWindow(this);
            UserInputWindow prompt_win = new UserInputWindow("Create new app group tab...", 
                                                             "Enter name of new app group tab", 
                                                             MaxTabNameLength, mywin);

            TabItem ti = Jrfc.Utility.FindTabByHeader(this.uc_MainTabControl, "All Apps");
            RdsExplorerPanel rep = (RdsExplorerPanel)Jrfc.Utility.FindChildren<RdsExplorerPanel>(ti).First<RdsExplorerPanel>();
            if (rep != null)
            {
                prompt_win.Left = (mywin.Left + mywin.Width) - (prompt_win.Width + 5);
                Point relativeLocation = rep.TranslatePoint(new Point(0, 0), mywin);
                prompt_win.Top = mywin.Top + relativeLocation.Y + uc_RdsExplorer_ButtonGrid.Height +5;
            }
            else
            {
                prompt_win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            if (prompt_win.ShowDialog() == true)
            {
                if(!string.IsNullOrWhiteSpace(prompt_win.UserInput))
                {
                    return (prompt_win.UserInput);
                }
            }
            return null;
        }

        private TabItem m_RightClickedTabItem = null;
        private void uc_MainTabControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(e.Source is TabItem)
            {
                this.m_RightClickedTabItem = (TabItem)e.Source;
            }

        }
        private void uc_MainTabControl_ContextMenu_DeleteTab_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_RightClickedTabItem == null)
                return;

            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete the (" + this.m_RightClickedTabItem.Header + ") tab?", 
                "Delete tab?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                this.uc_MainTabControl.Items.Remove(this.m_RightClickedTabItem);
                this.m_RightClickedTabItem = null;
            }
        }

        private void uc_MainTabControl_ContextMenu_Closing(object sender, ContextMenuEventArgs e)
        {
            //this.m_RightClickedTabItem = null;
        }

        private void uc_MainTabControl_ContextMenu_RefreshTab_Click(object sender, RoutedEventArgs e)
        {

        }

        private void uc_MainTabControl_ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (this.m_RightClickedTabItem == null)
                return;

            if (this.m_RightClickedTabItem.Header.ToString() == "All Apps")
            {
                this.uc_MainTabControl_ContextMenu_DeleteTab.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.uc_MainTabControl_ContextMenu_DeleteTab.Visibility = Visibility.Visible;
            }
        
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TabItem ti = Jrfc.Utility.FindTabByHeader(this.uc_MainTabControl, "All Apps");
            RdsExplorerPanel rep = (RdsExplorerPanel)Jrfc.Utility.FindChildren<RdsExplorerPanel>(ti).First<RdsExplorerPanel>();
            if (rep == null)
                return;

            rep.IsAnAllAppsPanel = true;
        }

        private void uc_RdsExplorer_AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            string header = PromptUserForNewTabName();
            if (header != null)
            {
                RdsExplorerTab new_tabitem = new RdsExplorerTab();
                new_tabitem.Header = header;
                this.uc_MainTabControl.Items.Insert(this.uc_MainTabControl.Items.Count, new_tabitem);
                this.uc_MainTabControl.SelectedIndex = this.uc_MainTabControl.Items.Count;
                new_tabitem.Content = new RdsExplorerPanel(); // false == create empty apps list
            }
        }

        private void uc_RdsExplorer_RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Known bug: Fix code to proper refresh ExplorerPanels other than Main
            //foreach(TabItem ti in this.uc_MainTabControl.Items)
            //{
            //    RdsExplorerPanel rep = (RdsExplorerPanel)Jrfc.Utility.FindChildren<RdsExplorerPanel>(ti).First<RdsExplorerPanel>();
            //    if (rep != null)
            //    {
            //        rep.RefreshPanel();
            //    }
            //}
            TabItem ti = (TabItem)this.uc_MainTabControl.Items.GetItemAt(0);
            RdsExplorerPanel rep = (RdsExplorerPanel)Jrfc.Utility.FindChildren<RdsExplorerPanel>(ti).First<RdsExplorerPanel>();
            if (rep != null)
            {
                rep.RefreshPanel();
            }
        }
    }

}
