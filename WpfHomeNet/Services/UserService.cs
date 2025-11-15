using WpfHomeNet.Data;
using WpfHomeNet.Helpers;
using WpfHomeNet.Models;
using WpfHomeNet.Data.Repositories;
namespace WpfHomeNet.Services
{
    public class UserService
    {
        private readonly ILogger _logger;

        private readonly UserRepository _repo;

        public UserService(UserRepository repo,ILogger logger) 
        {
            _logger = logger;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo), "Repository не может быть null");
        }


        public Task<List<UserEntity>> GetAllUsersAsync()
        {
            return Task.Run(() =>
            {
                var users = _repo.GetAllAsync(); // Синхронный вызов
                if (users == null)
                    throw new InvalidOperationException("Репозиторий вернул null");
               // _logger.LogInformation($" GetAllUsersAsync вернул {users.Count} пользователей");
                return users;
            });
        }


        public async Task AddUserAsync(UserEntity user)
        {
            // 1. Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Имя (FirstName) обязательно");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email обязателен");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Пароль (Password) обязателен");

            // 2. Заполняем дефолтные значения
            user.LastName ??= string.Empty;
            user.PhoneNumber ??= string.Empty;

            // 3. Асинхронный вызов репозитория
            await _repo.InsertUserAsync(user);
        }


        public async Task<UserEntity?> FindUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email обязателен");

            return await _repo.GetByEmailAsync(email);
        }

    }

}
