using Dapper;
using Microsoft.Data.Sqlite;

namespace HomeSocialNetwork.Data
{
    public class DatabaseInitializer
    {

        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            Console.WriteLine("Инициализация БД: проверка таблицы Users...");

            using var connection = new SqliteConnection(_connectionString);

            try
            {
                // Открываем соединение для выполнения запросов
                connection.Open();

                // Проверяем, существует ли таблица Users
                var tableExists = connection.ExecuteScalar<int>
                (
                    @"SELECT COUNT(*) FROM sqlite_master 
                    WHERE type = 'table' AND name = 'Users'"
                );

                if (tableExists == 0)
                {
                    // Таблицы нет — создаём
                    Console.WriteLine("Таблица Users не найдена. Создаю новую...");

                    connection.Execute
                    (                       
                       @"CREATE TABLE Users 
                       (
                         Id INTEGER PRIMARY KEY AUTOINCREMENT,
                         Email TEXT UNIQUE NOT NULL,
                         Password TEXT NOT NULL,
                         CreatedAt DATETIME NOT NULL DEFAULT (DATETIME('now'))
                       )"                                                                
                    );


                    Console.WriteLine("Таблица Users успешно создана.");
                }
                else
                {
                    // Таблица уже есть — проверяем её структуру (опционально)
                    Console.WriteLine("Таблица Users уже существует. Проверяю структуру...");

                    var columnCheck = connection.Query<dynamic>(@"PRAGMA table_info(Users)").ToList();

                    bool hasRequiredColumns =
                    columnCheck.Any(c => c.name == "Id") &&
                    columnCheck.Any(c => c.name == "Email") &&
                    columnCheck.Any(c => c.name == "Password");

                    if (hasRequiredColumns)
                    {
                        Console.WriteLine("Структура таблицы Users корректна.");
                    }
                    else
                    {
                        Console.WriteLine("Предупреждение: структура таблицы Users отличается от ожидаемой.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации БД: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

    }
}
