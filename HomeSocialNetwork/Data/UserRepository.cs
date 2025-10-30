using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using HomeSocialNetwork.Helpers;
using System.IO;
using HomeSocialNetwork.Models;
namespace HomeSocialNetwork.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(Action<string>? logAction = null)
        {
            _connectionString = DatabaseConfig.ConnectionString;

            var initializer = new DatabaseInitializer(_connectionString, logAction);
            initializer.Initialize();
        }

        public void Create(User user)
        {
            using var connection = new SqliteConnection(_connectionString);

            try
            {
                connection.Execute(
                    @"INSERT INTO Users (FirstName, LastName, PhoneNumber, Email, Password) 
                  VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @Password)",
                    user);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // UNIQUE constraint
            {
                throw new InvalidOperationException(
                    $"Email '{user.Email}' уже зарегистрирован.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка базы данных: {ex.Message}", ex);
            }
        }

        public List<User> GetAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<User>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt 
              FROM Users ORDER BY Id").ToList();
        }

        public User? GetByEmail(string email)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<User>(
                @"SELECT Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt
          FROM Users WHERE Email = @Email",
                new { Email = email });
        }
    }

}
