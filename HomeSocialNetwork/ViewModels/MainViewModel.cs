using HomeSocialNetwork.Models;
using HomeSocialNetwork.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeSocialNetwork.ViewModels
{


    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> _users;


        

        public ObservableCollection<User> Users                 
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users)); // Указываем имя свойства
            }

        }

        private Visibility _scrollViewerVisibility = Visibility.Visible;

        public Visibility ScrollViewerVisibility
        {
            get => _scrollViewerVisibility;
            set
            {
                _scrollViewerVisibility = value;
                OnPropertyChanged(nameof(ScrollViewerVisibility));
            }
        }





        // 2. Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        // Свойство для статуса (можно привязать к TextBlock)
        private string _statusText;
        public string StatusText
        {
            get => _statusText;

            set
            {
                string current = _hideText.ToString();
                string newValue = value?.ToString() ?? "null";
                Debug.WriteLine($"SETTER: {current} → {newValue}");
                
                OnPropertyChanged(nameof(HideText));
            }
        }


      



        public void LoadUsers(UserService userService)
        {
            try
            {
                var users = userService.GetAllUsers();
                Users = new ObservableCollection<User>(users);
                StatusText = $"Загружено {users.Count} пользователей";
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка загрузки: {ex.Message}";
            }
        }


        private bool _hideText = false; // Начальное значение
        public bool HideText
        {
            get => _hideText;

            set
            {
                Debug.WriteLine($"SETTER: {_hideText} → {value}");
                _hideText = value;
                OnPropertyChanged(nameof(HideText));
            }

        }
        
    }




}
