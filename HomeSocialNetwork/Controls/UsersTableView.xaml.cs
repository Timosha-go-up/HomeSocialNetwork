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
using HomeSocialNetwork;
namespace HomeSocialNetwork.Controls
{
    /// <summary>
    /// Interaction logic for UsersTableView.xaml
    /// </summary>
    public partial class UsersTableView : UserControl
    {
        private MainViewModel _viewModel;

        public UsersTableView()
        {
            InitializeComponent();

            // 1. Проверяем DataContext сразу
            if (DataContext is MainViewModel vm)
            {
                _viewModel = vm;
            }
            else
            {
                // 2. Если не получилось — ждём события Loaded
                Loaded += UsersTableView_Loaded;
            }
        }

        private void UsersTableView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is MainViewModel vm)
                {
                    _viewModel = vm;
                    // Отписываемся только если успешно обработали
                    Loaded -= UsersTableView_Loaded;
                }
                else
                {
                    Debug.WriteLine($"DataContext имеет тип: {DataContext?.GetType().Name}, ожидается MainViewModel");
                    throw new InvalidOperationException("DataContext не установлен или имеет неверный тип!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка в UsersTableView_Loaded: {ex.Message}");
                // Не отписываемся, чтобы попытаться снова при следующем Loaded
            }
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
