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
using System.Windows.Shapes;

namespace HomeSocialNetwork
{
    public partial class AddUserDialog : Window
    {
        public AddUserDialog()
        {
            InitializeComponent();
        }

        // Свойства для получения данных из диалогового окна
        public string Email => EmailTextBox.Text;
        public string Password => PasswordBox.Password;


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что поля заполнены
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true; // Подтверждаем выбор
            Close(); // Закрываем окно
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Отменяем выбор
            Close();
        }
    }
}
