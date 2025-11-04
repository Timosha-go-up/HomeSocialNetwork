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
using System.Windows.Input;

namespace HomeSocialNetwork.ViewModels
{


    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                if (_users == value) return; // Оптимизация: избегаем лишних уведомлений

                _users = value ?? new ObservableCollection<User>(); // Защита от null
                OnPropertyChanged(nameof(Users));
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


        public void ShowScrollViewer()
        {
            ScrollViewerVisibility = Visibility.Visible;
        }


        // 2. Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private bool _hideText = false;
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

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText == value) return;
                Debug.WriteLine($"SETTER: {_statusText} → {value}");
                _statusText = value ?? string.Empty;
                OnPropertyChanged(nameof(StatusText));
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


        
        
    }




}
