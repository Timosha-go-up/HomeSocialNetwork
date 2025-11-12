using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
namespace HomeSocialNetwork.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;
        private readonly DBInitializer _databaseInitializer;
        private readonly ILogger _logger;
        IDbConnection connection;
        public UserRepository(DBInitializer databaseInitializer,string connectionString, ILogger logger)
          
        {
            _databaseInitializer = databaseInitializer;

            _connectionString = connectionString;

           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task CreateAsync(UserEntity user)
        {
            

            try
            {
                await connection.ExecuteAsync(
                    @"INSERT INTO users (FirstName, LastName, PhoneNumber, Email, Password)
              VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @Password)",
                    user);

                _logger.LogInformation($"Вставка пользователя {user.FirstName} прошла успешно");
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                _logger.LogError($"Попытка вставки существующего email: {user.Email}");

                if (ex.Message.Contains("Email", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("users.Email", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Email '{user.Email}' уже зарегистрирован.");
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при добавлении пользователя: {ex.Message}");
                throw; 
            }
        }

        public List<UserEntity> GetAll()
        {
           
            _logger.LogDebug("UserRepository.GetAll: начало запроса");
            var users = connection.Query<UserEntity>(
            @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
            FROM users ORDER BY Id").ToList();
           
            _logger.LogInformation($"Получено пользователей из БД: [{users.Count}]");
            _logger.LogDebug("UserRepository.GetAll:  конец запроса");
            return users;
        }
        public UserEntity? GetByEmail(string email)
        {

            
                
                return connection.QueryFirstOrDefault<UserEntity>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
                FROM users WHERE Email = @Email",
                new { Email = email });
        }
    }







}
