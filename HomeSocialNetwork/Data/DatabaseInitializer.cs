using Dapper;
using HomeSocialNetwork.Helpers;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;

namespace HomeSocialNetwork.Data
{

    public static class DatabaseConfig
    {
        private static readonly string _databasePath;

        static DatabaseConfig()
        {
            _databasePath = PathBaseFiles.DatabasePath;
            PathBaseFiles.EnsureDatabaseDirectoryExists();
        }

        public static string ConnectionString => $"Data Source={_databasePath}";
    }



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
            Log("Инициализация БД: проверка таблицы Users...");

            using var connection = new SqliteConnection(DatabaseConfig.ConnectionString);

            try
            {
               connection.Open();
                var tableExists = connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*) FROM sqlite_master
                  WHERE type = 'table' AND name = 'Users'");

                if (tableExists == 0)
                {
                    Log("Таблица Users не найдена. Создаю новую...");
                    connection.Execute(
                    @"CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL DEFAULT '',
                    LastName TEXT DEFAULT '',
                    PhoneNumber TEXT DEFAULT '',
                    Email TEXT UNIQUE NOT NULL,
                    Password TEXT NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT (DATETIME('now', 'localtime'))
                    )");

                    Log("Таблица Users успешно создана.");
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
            Log("Таблица Users уже существует. Проверяю структуру таблицы Users...");

            var columns = connection.Query<dynamic>(@"PRAGMA table_info(Users)")
                .ToDictionary(c => c.name.ToLower(), c => new
                {
                    Type = c.type.ToLower(),
                    NotNull = c.notnull == 1,
                    DefaultValue = c.dflt_value?.ToString() ?? ""
                });

            // Список обязательных столбцов с ожидаемыми свойствами
            var requiredColumns = new Dictionary<string, Action<dynamic>>
            {
                ["id"] = col => {
                    if (col.Type != "integer") Log("Ошибка: Id должен быть INTEGER.");
                    if (!col.NotNull) Log("Ошибка: Id должен быть NOT NULL.");
                },
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
                Log("Структура таблицы Users корректна.");
            else
                Log("Предупреждение: структура таблицы Users содержит ошибки.");
        }


        // Вспомогательный метод для вывода лога
        private void Log(string message)
        {
            if (_logAction != null)
                _logAction(message);
        }
    }

}
