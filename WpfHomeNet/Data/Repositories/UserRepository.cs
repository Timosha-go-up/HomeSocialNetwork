using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.Sqlite;
using WpfHomeNet.Data.ConnectionFactories;
namespace WpfHomeNet.Data.Repositories
{
   public class UserRepository:IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger _logger;

        public UserRepository(IDbConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory ??
                throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserEntity> CreateAsync(UserEntity user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO users (FirstName, LastName, PhoneNumber, Email, Password)
                VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @Password)",
                user);

            _logger.LogInformation($"Пользователь {user.FirstName} добавлен.");
            return user;
        }



        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(
                @"DELETE FROM users WHERE Id = @Id",
                new { Id = id });

            if (affectedRows == 0)
            {
                throw new NotFoundException($"Пользователь с ID {id} не найден.");
            }

            _logger.LogInformation($"Пользователь с ID {id} удалён.");
        }



        public async Task<List<UserEntity>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var users = (await connection.QueryAsync<UserEntity>(
            @"SELECT Id, FirstName, LastName, PhoneNumber, Email,
            Password, CreatedAt FROM users ORDER BY Id"))         
            .ToList(); 

           _logger.LogInformation($"Получено {users.Count} пользователей.");
           return users;
        }


       
        public async Task<UserEntity?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserEntity>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
        FROM users WHERE Id = @Id",
                new { Id = id });
        }


        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserEntity>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
        FROM users WHERE Email = @Email",
                new { Email = email });
        }


        public async Task UpdateAsync(UserEntity user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                @"UPDATE users SET FirstName = @FirstName, LastName = @LastName,
           PhoneNumber = @PhoneNumber, Email = @Email, Password = @Password
           WHERE Id = @Id",
                user);

            _logger.LogInformation($"Пользователь {user.Id} обновлён.");
        }



    }

    [Serializable]
    internal class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
