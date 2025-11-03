using HomeSocialNetwork.Data;
using HomeSocialNetwork.Models;
using HomeSocialNetwork.Services;
using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
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
        private DispatcherTimer _logHideTimer;
        private Grid _logPanel; // Ссылка на контейнер логов из XAML



        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                _logHideTimer = new DispatcherTimer();
                _logHideTimer.Interval = TimeSpan.FromSeconds(10);
                _logHideTimer.Tick += OnLogHideTimerTick;
                this.Closed += (s, e) => _logHideTimer.Stop();

                WriteLog("Приложение запущено. PID: " + Process.GetCurrentProcess().Id);
                WriteLog("Таймер скрытия панели логов запущен (интервал: 10 сек).");

                var userRepository = new UserRepository();
                _userService = new UserService(userRepository); LoadUsers();

                _logPanel = LogPanel;
                if (_logPanel == null)
                {
                    Debug.WriteLine("LogPanel не найден в XAML!");
                }

               

                StartLogHideTimer();


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
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
                          this.Close();            
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

                        await Task.Delay(30); 
                    }
                }
            }
            finally
            {
                _isProcessing = false; 
            }
        }

        private void UsersGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если нажата левая кнопка мыши — начинаем перетаскивание окна
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        // Загрузка списка пользователей
        private void LoadUsers()
        {

            



            try
            {



                var users =  _userService.GetAllUsers();

                DataContext = new { Users = new ObservableCollection<User>(users) };

               

                StatusText.Text = $"Загружено {users.Count} пользователей";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Ошибка загрузки: {ex.Message}";
            }
        }



        // Кнопка «Добавить»



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

                LoadUsers();

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


        // Кнопка «Найти»
        private void FindUser_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция поиска пока не реализована");
        }

        // Кнопка «Обновить»
        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            
            StatusText.Text = "Обновление...";

            await Task.Delay(2000);

            // 4. Выводим финальное сообщение
            StatusText.Text = "Список обновлён";

            LoadUsers();


        }
    }
    }
