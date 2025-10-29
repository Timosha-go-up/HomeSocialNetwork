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

        public UserRepository()
        {
            // Создаём папку Data/DB/, если её нет
            PathBaseFiles.EnsureDatabaseDirectoryExists();

            // Получаем полный путь к файлу БД (уже включает "users.db")
            var dbPath = PathBaseFiles.DatabasePath;

            // Формируем строку подключения
            _connectionString = $"Data Source={dbPath}";

            // Инициализируем БД (если нужно)
            var initializer = new DatabaseInitializer(_connectionString);
            initializer.Initialize();
        }

        public void Create(User user)
        {
            using var connection = new SqliteConnection(_connectionString);

            try
            {
                connection.Execute(
                "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)",
                 user);
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                throw new InvalidOperationException(
                    $"Email '{user.Email}' уже зарегистрирован.");
            }
            catch (SqliteException ex)
            {
                // Другие ошибки SQLite (на всякий случай)
                throw new Exception($"Ошибка базы данных: {ex.Message}", ex);
            }
        }




        public List<User> GetAll()
        {
            var users = new List<User>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Email, Password ,CreatedAt FROM Users ORDER BY Id";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Password = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)  // добавляем чтение даты
                });
            }


            return users;
        }

        public User GetByEmail(string email)
        {
          using var connection = new SqliteConnection(_connectionString);
          return connection.QueryFirstOrDefault<User>( "SELECT * FROM Users WHERE Email = @Email", new { Email = email });                                         
        }

        
    }
}
