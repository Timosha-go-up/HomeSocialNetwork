using HomeSocialNetwork.Data;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using HomeSocialNetwork.Services;
using HomeSocialNetwork.UiHelpers;
using HomeSocialNetwork.ViewModels;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace HomeSocialNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ILogger _logger;
        private UserService _userService;
      
        private LogManager _logManager;
      public LogWindow _logWindow;
            
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                  // 1. Создаём ViewModel
                var mainVm = new MainViewModel();
                
               
                // 2. Устанавливаем DataContext окна
                DataContext = mainVm;

               

                _logWindow = new LogWindow();
                


                _logManager = new LogManager(_logWindow);

              _logger = new GenericLogger(_logManager.WriteLog);


                _logger.LogInformation("Application started. PID: " + Process.GetCurrentProcess().Id);

                var repo = new UserRepository(_logger);                  

                  _userService = new UserService(repo);



               


                mainVm.LoadUsers(_userService);
             _logWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Критическая ошибка при запуске: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        

        

       

      


        private void ShowScrollViewerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ShowScrollViewer(); 
            }
            else
            {
               Debug.WriteLine("DataContext не является MainViewModel!");
            }

        }
        private void ShowUser_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ScrollViewerVisibility = Visibility.Visible;
            }
            else
            {
               Debug.WriteLine
               ("DataContext не является MainViewModel. Проверьте привязку в XAML.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UsersGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если нажата левая кнопка мыши — начинаем перетаскивание окна
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        private  void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddUserDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() != true) return;

            var newUser = new User
            {
                FirstName = dialog.FirstName,
                LastName = dialog.LastName,
                PhoneNumber = dialog.PhoneNumber,
                Email = dialog.Email,
                Password = dialog.Password
            };

            var button = (Button)sender;
            button.IsEnabled = false;

            try
            {
                 _userService.AddUser(newUser);
                              
                StatusText.Text = "Пользователь добавлен";                
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить пользователя: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }


        // Кнопка «Обновить»
        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            
            StatusText.Text = "Обновление...";

            await Task.Delay(2000);

            // 4. Выводим финальное сообщение
            StatusText.Text = "Список обновлён";
           
        }
    }
    }
