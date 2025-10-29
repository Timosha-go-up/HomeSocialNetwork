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



        public void AddUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email и пароль обязательны");

            _repo.Create(new User { Email = email, Password = password });
        }

        public User FindUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email обязателен");

            return _repo.GetByEmail(email);
        }

        
    }
}
