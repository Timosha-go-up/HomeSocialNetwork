using HomeSocialNetwork.Data;
using HomeSocialNetwork.Models;
namespace HomeSocialNetwork.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;

        public UserService(UserRepository repo) => _repo = repo;

        public List<User> GetAllUsers()
        {
            return _repo.GetAll();
        }

        public void AddUser(User user)
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Имя (FirstName) обязательно");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email обязателен");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Пароль (Password) обязателен");

            // Заполняем дефолтные значения для nullable-строк
            user.LastName ??= string.Empty;
            user.PhoneNumber ??= string.Empty;

            _repo.Create(user);
        }


        public User? FindUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email обязателен");

            return _repo.GetByEmail(email);
        }
    }

}
