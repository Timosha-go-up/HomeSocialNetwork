using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
namespace HomeSocialNetwork.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        public UserRepository(ILogger logger,Action<string>? logAction = null)
          
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));_logger = logger;
            _connectionString = $"Data Source={PathBaseFiles.DatabasePath}";

            var initializer = new DatabaseInitializer(_logger,_connectionString, logAction);
            initializer.Initialize();
        }
        public void Create(User user)
        {
            using var connection = new SqliteConnection(_connectionString);

            try
            {
                connection.Execute(
                    @"INSERT INTO users (FirstName, LastName, PhoneNumber, Email, Password) 
                  VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @Password)",
                    user);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                // Проверяем, что в тексте ошибки упоминается Email
                if (ex.Message.Contains("Email", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("users.Email", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Email '{user.Email}' уже зарегистрирован.");
                }
                // Если ошибка по другому полю — перебрасываем исходное исключение
                throw;
            }
        }
        public List<User> GetAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<User>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt 
              FROM users ORDER BY Id").ToList();

           
        }
        public User? GetByEmail(string email)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<User>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
          FROM users WHERE Email = @Email",
                new { Email = email });
        }
    }

}
