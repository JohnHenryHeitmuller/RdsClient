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

namespace Jrfc
{
    /// <summary>
    /// Interaction logic for UserInputWindow.xaml
    /// </summary>
    public partial class UserInputWindow : Window
    {
        public string UserPrompt
        {
            get { return this.uc_UserPrompt.Text; }
            set { this.uc_UserPrompt.Text = value; }
        }
        public string UserInput
        {
            get { return this.uc_UserInput.Text; }
            set { this.uc_UserInput.Text = value; }
        }

        public int UserInputMaxLength
        {
            get { return this.uc_UserInput.MaxLength; }
            set { this.uc_UserInput.MaxLength = value; }
        }
        public UserInputWindow()
        {
            InitializeComponent();
        }

        public UserInputWindow(String _Title, string _UserPrompt, int _UserInputMaxLength, Window _Owner = null)
        {
            InitializeComponent();
            this.Title = _Title;
            this.UserPrompt = _UserPrompt;
            this.UserInputMaxLength = _UserInputMaxLength;
            if (_Owner != null)
                this.Owner = _Owner;
        }

        private void uc_OkayButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void uc_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.uc_UserInput.Focus();
        }
    }
}
