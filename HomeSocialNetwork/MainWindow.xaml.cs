using HomeSocialNetwork.Data;
using HomeSocialNetwork.Models;
using HomeSocialNetwork.Services;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace HomeSocialNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private UserService _userService;

        private ConcurrentQueue<string> _logQueue = new();
        private bool _isProcessing = false;

        // Добавляем сюда новые поля
        private DispatcherTimer _logHideTimer;
        private Grid _logPanel; // Ссылка на контейнер логов из XAML
       
        
        
        public MainWindow()
        {

            InitializeComponent();

            var userRepository = new UserRepository(WriteLog);
            _userService = new UserService(userRepository);

            System.Diagnostics.Debug.WriteLine($"MainWindow запущен. PID: {Process.GetCurrentProcess().Id}");
            WriteLog("Приложение запущено. PID: " + Process.GetCurrentProcess().Id);
            WriteLog("Таймер скрытия панели логов запущен (интервал: 5 сек).");
            _logPanel = LogPanel;

            if (_logPanel == null)
            {
                throw new Exception("LogPanel не найден! Проверьте x:Name в XAML.");
            }

            _logHideTimer = new DispatcherTimer();
            _logHideTimer.Interval = TimeSpan.FromSeconds(20);
            _logHideTimer.Tick += OnLogHideTimerTick;
            
            StartLogHideTimer();  // Запускаем таймер ПОСЛЕ записи в лог

            LoadUsers();
        }


        private async void OnLogHideTimerTick(object sender, EventArgs e)
        {
            try
            {
                // Ждём, пока завершится обработка очереди логов
                await WaitForLogProcessingCompletion();

                _logHideTimer.Stop();
                _logPanel.Visibility = Visibility.Collapsed;
                WriteLog("Таймер сработал: панель логов скрыта.");
            }
            catch (Exception ex)
            {
                WriteLog($"Ошибка при скрытии панели логов: {ex.Message}");
            }
        }

        // Ждём завершения обработки очереди логов
        private async Task WaitForLogProcessingCompletion()
        {
            while (_isProcessing)
            {
                await Task.Delay(1000); 
            }
        }

        // Запускаем/перезапускаем таймер
        private void StartLogHideTimer()
        {
            _logHideTimer.Stop();
            _logHideTimer.Start();
        }


        private void WriteLog(string message)
        {
            _logQueue.Enqueue(message);
            ProcessLogQueue(); 
        }

        private async void ProcessLogQueue()
        {
           
            if (_isProcessing)
                return;

            _isProcessing = true;

            try
            {
                while (_logQueue.TryDequeue(out string message))
                {
                    string timestamp = $"{DateTime.Now:HH:mm:ss} › ";
                    string fullText = timestamp + message + "\n";

                    // Посимвольно добавляем текст
                    foreach (char c in fullText)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LogDisplay.Text += c;
                            if (LogDisplay.Parent is ScrollViewer sv)
                                sv.ScrollToEnd();
                        });

                        await Task.Delay(40); 
                    }
                }
            }
            finally
            {
                _isProcessing = false; 
            }
        }






        // Загрузка списка пользователей
        private void LoadUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                UsersGrid.ItemsSource = users; // Привязка к DataGrid
                StatusText.Text = $"Загружено {users.Count} пользователей";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                StatusText.Text = "Ошибка";
            }
        }

        // Кнопка «Добавить»
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddUserDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                var newUser = new User
                {
                    FirstName = dialog.FirstName,  // добавьте это в диалоге!
                    LastName = dialog.LastName,    // добавьте это в диалоге!
                    PhoneNumber = dialog.PhoneNumber, // добавьте это в диалоге!
                    Email = dialog.Email,
                    Password = dialog.Password
                    // CreatedAt не задаём — БД заполнит сама
                };

                _userService.AddUser(newUser);  // передаём объект целиком
                LoadUsers();
                StatusText.Text = "Пользователь добавлен";
            }
        }



        // Кнопка «Найти»
        private void FindUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция поиска пока не реализована");
        }

        // Кнопка «Обновить»
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            StatusText.Text = "Список обновлён";
        }
    }
}