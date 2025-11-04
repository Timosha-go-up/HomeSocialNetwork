using Dapper;
using HomeSocialNetwork.Helpers;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly Action<string> _logAction;
    private readonly ILogger _logger;
    /// <summary>
    /// Конструктор инициализатора БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения к SQLite</param>
    /// <param name="logAction">Делегат для записи логов (например, AppLogger.Log)</param>
    /// 

    public DatabaseInitializer(ILogger logger,
        string connectionString,
        Action<string>? logAction = null)
    {
        _logger = logger;
        _connectionString = connectionString ??
            throw new ArgumentNullException(nameof(connectionString));

        // Если логгер не передан — используем пустой делегат (без вывода)
        _logAction = logAction ?? (msg => { });
    }

    /// <summary>
    /// Инициализировать БД: проверить/создать таблицу users.
    /// </summary>
    public void Initialize()
    {
        _logger.Log("Инициализация БД: проверка таблицы users...");


        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();

            // Проверяем, существует ли таблица users
            var tableExists = connection.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM sqlite_master
                  WHERE type = 'table' AND name = 'users'");

            if (tableExists == 0)
            {
              _logger.Log("Таблица users не найдена. Создаю новую...");
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
               _logger.Log("Таблица users успешно создана.");
            }
            else
            {
                CheckTableStructure(connection);
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Ошибка при инициализации БД: {ex.Message}");
            _logger.Log($"StackTrace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Проверить структуру существующей таблицы users.
    /// </summary>
    private void CheckTableStructure(SqliteConnection connection)
    {
        _logger.Log("Таблица users уже существует. Проверяю структуру...");

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
                if (col.Type != "text") _logger.Log("Ошибка: FirstName должен быть TEXT.");
                if (!col.NotNull)       _logger.Log("Ошибка: FirstName должен быть NOT NULL.");
                if (col.DefaultValue != "''") _logger.Log("Ошибка: FirstName должен иметь DEFAULT ''.");
            },
            ["lastname"] = col => {
                if (col.Type != "text")     _logger.Log("Ошибка: LastName должен быть TEXT.");
                if (col.NotNull)    _logger.Log("Ошибка: LastName не должен быть NOT NULL (может быть пустым).");
            },
            ["phonenumber"] = col => {
                if (col.Type != "text")     _logger.Log("Ошибка: PhoneNumber должен быть TEXT.");
                if (col.NotNull)    _logger.Log("Ошибка: PhoneNumber не должен быть NOT NULL (может быть пустым).");
            },
            ["email"] = col => {
                if (col.Type != "text")             _logger.Log("Ошибка: Email должен быть TEXT.");
                if (!col.NotNull) _logger.Log    ("Ошибка: Email должен быть NOT NULL.");
            },
            ["password"] = col => {
                if (col.Type != "text")     _logger.Log("Ошибка: Password должен быть TEXT.");
                if (!col.NotNull) _logger.Log("Ошибка: Password должен быть NOT NULL.");
            },
            ["createdat"] = col => {
                if (col.Type != "datetime") _logger.Log("Ошибка: CreatedAt должен быть DATETIME.");
                if (!col.NotNull) _logger.Log("Ошибка: CreatedAt должен быть NOT NULL.");
            }
        };

        bool isStructureValid = true;

        foreach (var (colName, checkAction) in requiredColumns)
        {
            if (!columns.ContainsKey(colName))
            {
                _logger.Log($"Ошибка: отсутствует столбец '{colName}'.");
                isStructureValid = false;
            }
            else
            {
                checkAction(columns[colName]);
            }
        }

        if (isStructureValid)
            _logger.Log("Структура таблицы users корректна.");
        else
            _logger.    Log("Предупреждение: структура таблицы users содержит ошибки.");
    }

    


}

