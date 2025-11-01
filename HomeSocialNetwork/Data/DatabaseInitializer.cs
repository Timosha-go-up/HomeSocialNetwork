using Dapper;
using HomeSocialNetwork.Helpers;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace HomeSocialNetwork.Data
{ 
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly Action<string>? _logAction;
        public DatabaseInitializer(string connectionString, Action<string>? logAction = null)
        {
            _connectionString = connectionString;
            _logAction = logAction ?? (msg => { });
        }
        public void Initialize()
        {
            Log("Инициализация БД: проверка таблицы users...");

            using var connection = new SqliteConnection(_connectionString);

            try
            {
               connection.Open();
                var tableExists = connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*) FROM sqlite_master
                  WHERE type = 'table' AND name = 'users'");

                if (tableExists == 0)
                {
                    Log("Таблица users не найдена. Создаю новую...");
                    connection.Execute(
                    @"CREATE TABLE users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL DEFAULT '',
                    LastName TEXT DEFAULT '',
                    PhoneNumber TEXT DEFAULT '',
                    Email TEXT UNIQUE NOT NULL,
                    Password TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT (DATETIME('now', 'localtime'))
                    )");

                    Log("Таблица users успешно создана.");
                }
                else
                {
                    CheckTableStructure(connection);                    
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при инициализации БД: {ex.Message}");
                Log($"StackTrace: {ex.StackTrace}");
            }
        }       
        private void CheckTableStructure(SqliteConnection connection)
        {
            Log("Таблица users уже существует. Проверяю структуру таблицы Users...");

            var columns = connection.Query<dynamic>(@"PRAGMA table_info(users)")
                .ToDictionary(c => c.name.ToLower(), c => new
                {
                    Type = c.type.ToLower(),
                    NotNull = c.notnull == 1,
                    DefaultValue = c.dflt_value?.ToString() ?? ""
                });
           
            var requiredColumns = new Dictionary<string, Action<dynamic>>
            {               
                ["firstname"] = col => {
                    if (col.Type != "text") Log("Ошибка: FirstName должен быть TEXT.");
                    if (!col.NotNull) Log("Ошибка: FirstName должен быть NOT NULL.");
                    if (col.DefaultValue != "''") Log("Ошибка: FirstName должен иметь DEFAULT ''.");
                },
                ["lastname"] = col => {
                    if (col.Type != "text") Log("Ошибка: LastName должен быть TEXT.");
                    if (col.NotNull) Log("Ошибка: LastName не должен быть NOT NULL (может быть пустым).");
                },
                ["phonenumber"] = col => {
                    if (col.Type != "text") Log("Ошибка: PhoneNumber должен быть TEXT.");
                    if (col.NotNull) Log("Ошибка: PhoneNumber не должен быть NOT NULL (может быть пустым).");
                },
                ["email"] = col => {
                    if (col.Type != "text") Log("Ошибка: Email должен быть TEXT.");
                    if (!col.NotNull) Log("Ошибка: Email должен быть NOT NULL.");
                },
                ["password"] = col => {
                    if (col.Type != "text") Log("Ошибка: Password должен быть TEXT.");
                    if (!col.NotNull) Log("Ошибка: Password должен быть NOT NULL.");
                },
                ["createdat"] = col => {
                    if (col.Type != "datetime") Log("Ошибка: CreatedAt должен быть DATETIME.");
                    if (!col.NotNull) Log("Ошибка: CreatedAt должен быть NOT NULL.");
                }
            };

            bool isStructureValid = true;

            foreach (var (colName, checkAction) in requiredColumns)
            {
                if (!columns.ContainsKey(colName))
                {
                    Log($"Ошибка: отсутствует столбец '{colName}'.");
                    isStructureValid = false;
                }
                else
                {
                    checkAction(columns[colName]);
                }
            }
            if (isStructureValid)
                Log("Структура таблицы users корректна.");
            else
                Log("Предупреждение: структура таблицы users содержит ошибки.");
        }
        // Вспомогательный метод для вывода лога
        private void Log(string message)
        {
            if (_logAction != null)
                _logAction(message);
        }             
    }

}
