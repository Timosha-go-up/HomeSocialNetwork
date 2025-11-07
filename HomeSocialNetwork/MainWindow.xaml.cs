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
using System.Windows.Media.Animation;
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
        private MainViewModel _mainVm; // Сохраняем ссылку на VM
        private IStatusUpdater _status; // Только этот интерфейс!
        private DBInitializer _databaseInitializer;
        private string _connectionDB;
        public MainWindow()
        {
            InitializeComponent();
            this.Left = 100;

            // Запуск асинхронной инициализации с обработкой ошибок
            InitializeAsync().ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    MessageBox.Show(
                        $"Критическая ошибка при запуске: {task.Exception.InnerException?.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Close();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task InitializeAsync()
        {
            try
            {
                _logWindow = new LogWindow();                             
                _connectionDB = $"Data Source={PathBaseFiles.DatabasePath}";
                _logManager = new LogManager(_logWindow);
                _logger = new GenericLogger(_logManager.WriteLog);

                _logger.LogInformation("Application started. PID: " + Process.GetCurrentProcess().Id);

                _databaseInitializer = new DBInitializer(_logger, _connectionDB);

                // Асинхронное ожидание инициализации БД
                await _databaseInitializer.InitializeAsync();

                var repo = new UserRepository(_databaseInitializer, _connectionDB, _logger);
                _userService = new UserService(repo, _logger);

                // Создание ViewModel
                _mainVm = new MainViewModel(_userService, _logger);
                _status = (IStatusUpdater)_mainVm;
                DataContext = _status;

                // Асинхронная загрузка данных
                await LoadUsersOnStartupAsync();


                _logWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                _logWindow.Left = Application.Current.MainWindow.Left + 1000;
                _logWindow.Top = Application.Current.MainWindow.Top + 10;
                

                _logWindow.Show();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Инициализация завершилась с ошибкой: {ex.Message}");
                throw; // Передаём исключение в ContinueWith
            }
        }

        // Асинхронный метод загрузки данных
        private async Task LoadUsersOnStartupAsync()
        {
            try
            {
                await _mainVm.LoadUsersAsync();
                _logger.LogInformation("Пользователи загружены при старте.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка загрузки пользователей при старте: " + ex.Message);
                MessageBox.Show(
                    $"Не удалось загрузить пользователей: {ex.Message}",
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ShowScrollViewerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.ShowScrollViewer();
            else
                Debug.WriteLine("DataContext не является MainViewModel!");
        }

       


        private async void ShowUser_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {                 
                await Task.Delay(2000);
            
                vm.ScrollViewerVisibility = Visibility.Visible;                                   
            }
            else
            {
                Debug.WriteLine("DataContext не является MainViewModel. Проверьте привязку в XAML.");
            }
        }

        public static class ButtonHelpers  // ← public static!
        {
            // Присоединённое свойство — public
            public static HorizontalAlignment GetContentAlignment(DependencyObject obj)
                => (HorizontalAlignment)obj.GetValue(ContentAlignmentProperty);

            public static void SetContentAlignment(DependencyObject obj, HorizontalAlignment value)
                => obj.SetValue(ContentAlignmentProperty, value);

            public static readonly DependencyProperty ContentAlignmentProperty =
                DependencyProperty.RegisterAttached(
                    "ContentAlignment",
                    typeof(HorizontalAlignment),
                    typeof(ButtonHelpers),
                    new PropertyMetadata(HorizontalAlignment.Center)
                );
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void UsersGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
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
                await _userService.AddUserAsync(newUser); // Делаем асинхронным!

                _status.SetStatus("Пользователь добавлен");

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

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // 1. Показываем статус "Обновление..."
            _status.SetStatus("Обновление...");

            // 2. Ждём 2 секунды
            await Task.Delay(2000);

            try
            {
                // 3. Обновляем статус на "Список обновлён"
                _status.SetStatus("Список обновлён");

                // 4. Ждём ещё 2 секунды
                await Task.Delay(2000);

                // 5. Загружаем пользователей
                await _mainVm.LoadUsersAsync();
            }
            catch (Exception ex)
            {
                // В случае ошибки показываем сообщение
                _status.SetStatus($"Ошибка обновления: {ex.Message}");
            }
        }


    }
}
