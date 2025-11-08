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
using WpfHomeNet.UiHelpers;


namespace WpfHomeNet
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
        private DBInitializerSql _databaseInitializer;
        private string _connectionDB;
        public MainWindow()
        {
            InitializeComponent();
            this.Left = 20;

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
                _logManager = new LogManager(_logWindow);
                _logger = new GenericLogger(_logManager.WriteLog);
                // Получаем валидный путь к БД (основной или резервный)
                var dbPath = PathBaseFiles.GetValidDatabasePath();
                _logger.LogInformation($"Путь бд {dbPath}");
                _connectionDB = $"Data Source={dbPath}";

                _logger.LogInformation("Application started. PID: " + Process.GetCurrentProcess().Id);

                _databaseInitializer = new DBInitializerSql(_logger, _connectionDB);

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

    }
}