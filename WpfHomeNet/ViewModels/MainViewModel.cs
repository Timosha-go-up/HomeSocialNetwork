using HomeSocialNetwork.Helpers;
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

    
        public interface IStatusUpdater
        {
            void SetStatus(string message);
        }

    public class MainViewModel : INotifyPropertyChanged, IStatusUpdater
    {
        private readonly UserService _userService;
        private readonly ILogger _logger;


        public MainViewModel(UserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private ObservableCollection<UserEntity> _users = new ObservableCollection<UserEntity>();
        public ObservableCollection<UserEntity> Users
        {
            get => _users;
            private set
            {
                if (_users == value) return;
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private Visibility _scrollViewerVisibility = Visibility.Collapsed;
        public Visibility ScrollViewerVisibility
        {
            get => _scrollViewerVisibility;
            set
            {
                _scrollViewerVisibility = value;
                OnPropertyChanged(nameof(ScrollViewerVisibility));
            }
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            private set
            {
                if (_statusText == value) return;
                _statusText = value;
                Debug.WriteLine($"SETTER: {_statusText} → {value}");
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public void SetStatus(string message) => StatusText = message;

        public void ShowScrollViewer()
        {
            ScrollViewerVisibility = Visibility.Visible;
        }

        public async Task LoadUsersAsync()
        {
            StatusText = "Загрузка...";

            try
            {
                var users = await _userService.GetAllUsersAsync();

                if (users == null)
                {
                    StatusText = "Ошибка: получены пустые данные.";
                    return;
                }

                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                _logger?.LogInformation($"Загружено {users.Count} пользователей");

                StatusText = $"Загружено {users.Count} пользователей";
            }
            catch (Exception ex)
            {
                _logger?.LogError("Ошибка загрузки пользователей");
                StatusText = $"Ошибка загрузки: {ex.Message}";
            }
        }





        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }





}
