using HomeSocialNetwork.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace HomeSocialNetwork.Controls
{
    /// <summary>
    /// Interaction logic for UsersTableView.xaml
    /// </summary>
    public partial class UsersTableView : UserControl
    {
        public UsersTableView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }


        private void TextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ScrollViewerVisibility = Visibility.Collapsed; // Скрываем ScrollViewer
            }
        }












    }
}
