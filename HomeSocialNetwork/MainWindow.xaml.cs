using HomeSocialNetwork.Data;
using HomeSocialNetwork.Models;
using HomeSocialNetwork.Services;
using System.Diagnostics;
using System.Windows;
namespace HomeSocialNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private UserService _userService;

        public MainWindow()
        {
            InitializeComponent();
          
            var userRepository = new UserRepository(); // Создаём репозиторий
            _userService = new UserService(userRepository); // Создаём сервис
            System.Diagnostics.Debug.WriteLine($"MainWindow запущен. PID: {Process.GetCurrentProcess().Id}");
            LoadUsers();
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
                    Email = dialog.Email,
                    Password = dialog.Password
                    // CreatedAt НЕ задаём — БД заполнит сама!
                };
               
                _userService.AddUser(newUser.Email,newUser.Password);
                LoadUsers();
                StatusText.Text = "Пользователь добавлен!";
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