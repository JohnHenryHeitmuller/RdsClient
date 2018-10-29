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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jrfc
{
    /// <summary>
    /// Interaction logic for RdsExplorerPanel.xaml
    /// </summary>
    [Serializable()]
    public partial class RdsExplorerPanel : UserControl
    {
        private Jrfc.Rds.RdsServerList m_RdsServerList; // { get; set; }

        public Jrfc.Rds.RdsServerList ServerList
        {
            get { return this.m_RdsServerList; }
            set { this.m_RdsServerList = value; }
        }

        private Jrfc.Rds.RdsAppList m_RdsAppList; // { get; set; }
        public Jrfc.Rds.RdsAppList AppList
        {
            get { return m_RdsAppList; }
            //set { this.m_RdsAppList = value; }
        }

        private ExpandedServerList m_ExandedServerList;
        public string ToXmlDictionaryString()
        {
            string ServerList_Xml = this.m_RdsServerList.ToXmlString();
            string AppList_Xml = this.m_RdsAppList.ToXmlString();
            string ExpandedServerList_Xml = this.m_ExandedServerList.ToXmlString();

            XmlDictionary XmlList = new XmlDictionary();
            XmlList.Add("ServerList", ServerList_Xml);
            XmlList.Add("AppList", AppList_Xml);
            XmlList.Add("ExpandedServerList", ExpandedServerList_Xml);
            return XmlList.ToXmlString();
        }

        public static RdsExplorerPanel CreatFromXmlDictionaryString(string _xml)
        {
            XmlDictionary XmlList = Jrfc.Utility.ObjectSerializer<XmlDictionary>.FromXml(_xml);
            Jrfc.RdsExplorerPanel rep = new RdsExplorerPanel();
            string ServerList_Xml = XmlList["ServerList"];
            if (ServerList_Xml != null)
            {
                RdsServerList rds_servers = (RdsServerList)Jrfc.Utility.ObjectSerializer<RdsServerList>.FromXml(ServerList_Xml);
                rep.ServerList.Copy(rds_servers);
            }
            string AppList_Xml = XmlList["AppList"];
            if (AppList_Xml != null)
            {
                RdsAppList apps = (RdsAppList)Jrfc.Utility.ObjectSerializer<RdsAppList>.FromXml(AppList_Xml);
                rep.m_RdsAppList.Copy(apps);
            }
            string ExpandedServerList_Xml = XmlList["ExpandedServerList"];
            if(ExpandedServerList_Xml != null)
            {
                ExpandedServerList exp_servers = (ExpandedServerList)Jrfc.Utility.ObjectSerializer<ExpandedServerList>.FromXml(ExpandedServerList_Xml);
                rep.m_ExandedServerList.Copy(exp_servers);
            }
            return rep;
        }

        public RdsExplorerPanel()
        {
            InitializeComponent();
            this.DataContext = this;
            this.m_RdsServerList = new RdsServerList(false);
            this.m_RdsAppList = new RdsAppList();
            this.m_ExandedServerList = new ExpandedServerList();
            this.uc_RdsExplorerPanel_ListView.ItemsSource = this.m_RdsAppList;

            CollectionView view_ex = (CollectionView)CollectionViewSource.GetDefaultView(this.uc_RdsExplorerPanel_ListView.ItemsSource);
            PropertyGroupDescription groupDescription_ex = new PropertyGroupDescription("Hostname");
            view_ex.GroupDescriptions.Add(groupDescription_ex);
            view_ex.SortDescriptions.Add(new SortDescription("Hostname", ListSortDirection.Ascending));
            view_ex.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
        }

        private void uc_RdsExplorerPanel_Loaded(object sender, RoutedEventArgs e)
        {
        }

        //public RdsExplorerPanel(bool _LoadOnCreate)
        //{
        //    InitializeComponent();
        //    this.m_RdsServerList = new RdsServerList();
        //    if (!_LoadOnCreate)
        //    {
        //        this.m_RdsAppList = new RdsAppList();
        //    }
        //    else
        //    {
        //        this.m_RdsAppList = new RdsAppList(this.m_RdsServerList);
        //    }
        //    this.uc_ExplorerListView.ItemsSource = this.m_RdsAppList;
        //    CollectionView view_ex = (CollectionView)CollectionViewSource.GetDefaultView(this.uc_ExplorerListView.ItemsSource);
        //    PropertyGroupDescription groupDescription_ex = new PropertyGroupDescription("Hostname");
        //    view_ex.GroupDescriptions.Add(groupDescription_ex);
        //}

        //public RdsExplorerPanel(RdsServerList _RdsServerList)
        //{
        //    InitializeComponent();
        //    this.m_RdsServerList = _RdsServerList;
        //    this.m_RdsAppList = new RdsAppList(this.m_RdsServerList);
        //}
        //public RdsExplorerPanel(RdsAppList _RdsAppsList)
        //{
        //    InitializeComponent();
        //    this.m_RdsServerList = new RdsServerList(_RdsAppsList);
        //    this.m_RdsAppList = _RdsAppsList;
        //}

        private bool m_IsAnAllAppsPanel;
        public bool IsAnAllAppsPanel
        {
            get { return this.m_IsAnAllAppsPanel; }
            set
            {
                this.m_IsAnAllAppsPanel = value;
                if (value == true)
                {
                    this.m_RdsServerList = new RdsServerList(true);     
                    this.m_RdsAppList.Add(this.m_RdsServerList);
                }
            }
        }

        public void RefreshPanel()
        {
            this.m_RdsServerList.RefreshList();
            this.m_RdsAppList.RefreshList();
        }
        private void uc_RdsExplorerPanel_ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            /*
            < MenuItem Header = "Pin to Taskbar"
                      x: Name = "uc_RdsExplorerPanel_ContextMenu_PinToTaskbar"
                      Click = "uc_RdsExplorerPanel_ContextMenu_PinToTaskbar_Click" />
            < MenuItem Header = "Pin to Start"
                      x: Name = "uc_RdsExplorerPanel_ContextMenu_PinToStart"
                      Click = "uc_RdsExplorerPanel_ContextMenu_PinToStart_Click" />
            < MenuItem Header = "Remove from list"
                      x: Name = "uc_RdsExplorerPanel_ContextMenu_RemoveFromList"
                      Click = "uc_RdsExplorerPanel_ContextMenu_RemoveFromList_Click" />
            */
            this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Clear();
            this.ApplyContextMenuItems_RemoteMruList(sender);
            this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Add(new Separator());

            MenuItem pin_to_taskbar = new MenuItem();
            pin_to_taskbar.Header = "Pin To Taskbar";
            pin_to_taskbar.Click += uc_RdsExplorerPanel_ContextMenu_PinToTaskbar_Click;
            this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Add(pin_to_taskbar);

            MenuItem pin_to_start = new MenuItem();
            pin_to_start.Header = "Pin To start";
            pin_to_start.Click += uc_RdsExplorerPanel_ContextMenu_PinToStart_Click;
            this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Add(pin_to_start);

                           
            //if (!IsAnAllAppsPanel)
            //{
                MenuItem remove_app = new MenuItem();
                remove_app.Header = "Remove";
                remove_app.Click += uc_RdsExplorerPanel_ContextMenu_RemoveFromList_Click;
                this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Add(remove_app);
            //}

            this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Add(new Separator());
            this.ApplyContextMenuItems_CopyAppToTab();
        }

        private void ApplyContextMenuItems_RemoteMruList(Object sender)
        {
            RdsApp app = (RdsApp)this.uc_RdsExplorerPanel_ListView.SelectedItem;
            if (app == null)
                return;
            MruList mru_list = app.GetRemoteMruList();
            if (mru_list == null)
                return;

            //if(mru_list.Count() > 0)
            //{
            //    this.uc_RdsExplorerPanel_ListView.ContextMenu.Items.Insert(0, new Separator());
            //}

            foreach (Mru mru in mru_list)
            {
                MenuItem itm = new MenuItem();
                
                itm.Header = mru.DisplayName;
                //itm.Icon = new Image { Source = new BitmapImage(new Uri("Resources/pin.ico")) };
                //itm.Header = CreateMruMenuItemHeader(mru);

                itm.Click += this.uc_RdsExplorerPanel_ContextMenu_LaunchMruItem;
                itm.DataContext = mru;
                itm.ToolTip = mru.FlybyHintText;
                
                Jrfc.Utility.AddItemToContextMenu( this.uc_RdsExplorerPanel_ListView.ContextMenu, itm, 
                                                   Jrfc.Utility.TYPE_OF_ADD.SkipIfExists, 
                                                   Utility.ADD_LOCATION.Top);
            }
        }

        //private object CreateMruMenuItemHeader(Mru _mru)
        //{
        //    TextBlock tb_header = new TextBlock();
        //    tb_header.Text = _mru.DisplayName;
        //    tb_header.VerticalAlignment = VerticalAlignment.Center;
        //    tb_header.HorizontalAlignment = HorizontalAlignment.Left;

        //    TextBlock tb_pinsymbol = new TextBlock();
        //    tb_pinsymbol.FontFamily = new FontFamily("Segoe UI Symbol");
        //    tb_pinsymbol.Text = "\uE141";
        //    tb_pinsymbol.Margin = new Thickness(10,0,5,0);
        //    tb_pinsymbol.VerticalAlignment = VerticalAlignment.Center;
        //    tb_pinsymbol.HorizontalAlignment = HorizontalAlignment.Right;

        //    Grid grid = new Grid();
        //    grid.Background = this.uc_RdsExplorerPanel_ListView.ContextMenu.Background;
        //    grid.Width = this.uc_RdsExplorerPanel_ListView.ContextMenu.Width;
        //    grid.RowDefinitions.Add(new RowDefinition());
        //    grid.ColumnDefinitions.Add(new ColumnDefinition());
        //    grid.ColumnDefinitions.Add(new ColumnDefinition());
        //    grid.Children.Add(tb_header);
        //    grid.Children.Add(tb_pinsymbol);
        //    Grid.SetColumn(tb_header, 0);
        //    Grid.SetColumn(tb_pinsymbol, 1);
        //    return grid;
        //}
      
        private void uc_RdsExplorerPanel_ContextMenu_LaunchMruItem(object sender, RoutedEventArgs e)
        {
            DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            if (originalSource != null)
            {
                RdsApp app = (RdsApp)this.uc_RdsExplorerPanel_ListView.SelectedItem;
                app.Launch(((Mru)((MenuItem)originalSource).DataContext).Path);
            }
        }
        private void ApplyContextMenuItems_CopyAppToTab()
        {
            RdsExplorerContainer rec = Jrfc.Utility.FindParent<RdsExplorerContainer>(this);
            if (rec != null)
            {
                if (rec.uc_MainTabControl.SelectedIndex == 0)
                {
                    foreach (TabItem tab in rec.uc_MainTabControl.Items)
                    {
                        if (tab.Header.ToString() != "All Apps" && tab.Header.ToString() != "+")
                        {
                            MenuItem itm = new MenuItem();
                            itm.Header = "Add selected App(s) to " + tab.Header.ToString() + " tab";
                            itm.Click += this.uc_RdsExplorerPanel_ContextMenu_AddAppToTab_Click;
                            Jrfc.Utility.AddItemToContextMenu(this.uc_RdsExplorerPanel_ListView.ContextMenu, itm, Jrfc.Utility.TYPE_OF_ADD.SkipIfExists);
                        }
                    }
                }
            }
        }

        private void uc_RdsExplorerPanel_ContextMenu_AddAppToTab_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (MenuItem)sender;

            RdsExplorerContainer rec = Jrfc.Utility.FindParent<RdsExplorerContainer>(this);
            string target_tab_header = mnu.Header.ToString().Replace("Add selected App(s) to ", "").Replace(" tab", "");
            TabItem t = Jrfc.Utility.FindTabByHeader(rec.uc_MainTabControl, target_tab_header);
            RdsExplorerPanel exp = (RdsExplorerPanel)t.Content;

            foreach (RdsApp app in this.uc_RdsExplorerPanel_ListView.SelectedItems)
            {
                exp.AppList.Add(app);
            }
        }

        private void uc_RdsExplorerPanel_ContextMenu_PinToTaskbar_Click(object sender, RoutedEventArgs e)
        {
            foreach(RdsApp app in this.uc_RdsExplorerPanel_ListView.SelectedItems)
            {
                app.PinToTaskbar(RdsApp.PinAction.Pin);
            }
        }

        private void uc_RdsExplorerPanel_ContextMenu_PinToStart_Click(object sender, RoutedEventArgs e)
        {
            foreach (RdsApp app in this.uc_RdsExplorerPanel_ListView.SelectedItems)
            {
                app.PinToStart(RdsApp.PinAction.Pin);
            }
        }

        private void uc_RdsExplorerPanel_ContextMenu_RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            while(this.uc_RdsExplorerPanel_ListView.SelectedIndex > -1)
            {
                this.m_RdsAppList.RemoveAt(this.uc_RdsExplorerPanel_ListView.SelectedIndex);
            }
        }

        private void uc_RdsExplorerPanel_ServerGroupExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;
            if (exp.DataContext != BindingOperations.DisconnectedSource)
            {
                CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
                string Hostname = (string)dc.Name.ToString();

                //Loaded event is fired earlier than the Click event, so I'm sure that the dictionary contains the key
                if (this.m_ExandedServerList.IndexOf(Hostname) < 0)
                {
                    this.m_ExandedServerList.Add(Hostname);
                }
            }
        }

        private void uc_RdsExplorerPanel_ServerGroupExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;
            if (exp.DataContext != BindingOperations.DisconnectedSource)
            {
                CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
                string Hostname = (string)dc.Name.ToString();
                this.m_ExandedServerList.Remove(Hostname);
            }
        }

        private void uc_RdsExplorerPanel_ServerGroupExpander_Loaded(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;
            if (exp.DataContext == BindingOperations.DisconnectedSource)
            {
                exp.IsExpanded = false;
            }
            else
            {
                CollectionViewGroup dc = (CollectionViewGroup)exp.DataContext;
                string Hostname = (string)dc.Name.ToString();
                if (m_ExandedServerList.IndexOf(Hostname) >= 0)
                {
                    exp.IsExpanded = true;
                }
                else
                {
                    exp.IsExpanded = false;
                }
            }
        }

        private void uc_RdsExplorerPanel_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.EventHandler_AppLaunch((DependencyObject)e.OriginalSource);
        }

        private void EventHandler_AppLaunch(DependencyObject originalSource)
        {
            //DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is ListViewItem))
            {
                originalSource = VisualTreeHelper.GetParent(originalSource);
            }
            //if it didn’t find a ListViewItem anywhere in the hierarch, it’s because the user
            //didn’t click on one. Therefore, if the variable isn’t null, run the code
            if (originalSource != null)
            {
                RdsApp app = (RdsApp)((System.Windows.FrameworkElement)originalSource).DataContext;
                app.Launch();
            }
        }

        private void uc_RdsExplorerPanel_ListView_Item_MouseEnter(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("MouseEnter");
        }
    }

    [Serializable()]
    public class ExpandedServerList: List<string>
    {
        public ExpandedServerList() : base()
        {
        }

        public string ToXmlString()
        {
            return Jrfc.Utility.ObjectSerializer<ExpandedServerList>.ToXml(this);
        }

        public ExpandedServerList Copy(ExpandedServerList _NewList)
        {
            this.Clear();
            foreach (string s in _NewList)
            {
                this.Add(s);
            }
            return this;
        }
    }
}
