using Dapper;
using HomeSocialNetwork.Helpers;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
public class DBInitializer
{
    private readonly string _connectionString;   
    private readonly ILogger _logger;
    /// <summary>
    /// Конструктор инициализатора БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения к SQLite</param>
    /// <param name="logAction">Делегат для записи логов (например, AppLogger.Log)</param>
    /// 

     public DBInitializer(ILogger logger,string connectionString)               
    {
            _logger = logger;
            _connectionString = connectionString ??
            throw new ArgumentNullException(nameof(connectionString));              
    }
    
    public async Task InitializeAsync()
    {
        LogInitializationStarted();

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        if (!await TableExistsAsync(connection, "users"))
        {
            await CreateUsersTableAsync(connection);
        }
        else
        {
            await CheckTableStructureAsync(connection);
        }
    }

    private void LogInitializationStarted()
    {
        _logger.LogDebug("Инициализация БД: проверка таблицы users...");
    }

    private async Task<bool> TableExistsAsync(SqliteConnection connection, string tableName)
    {
        var sql = @"SELECT COUNT(*) FROM sqlite_master 
                WHERE type = 'table' AND name = @tableName";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { tableName });
        return count > 0;
    }


    private async Task CreateUsersTableAsync(SqliteConnection connection)
    {
        _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        var sql = @"CREATE TABLE users (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        FirstName TEXT NOT NULL DEFAULT '',
        LastName TEXT DEFAULT '',
        PhoneNumber TEXT DEFAULT '',
        Email TEXT UNIQUE NOT NULL,
        Password TEXT NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT (DATETIME('now', 'localtime'))
    )";

        await connection.ExecuteAsync(sql);
        _logger.LogDebug("Таблица users успешно создана.");
    }

    /// <summary>
    /// Проверить структуру существующей таблицы users.
    /// </summary>
    private async Task CheckTableStructureAsync(SqliteConnection connection)
    {
        _logger.LogInformation("Таблица users уже существует. Проверяю структуру...");

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
                if (col.Type != "text") _logger.LogWarning("Ошибка: FirstName должен быть TEXT.");
                if (!col.NotNull)       _logger.LogError("Ошибка: FirstName должен быть NOT NULL.");
                if (col.DefaultValue != "''") _logger.LogError("Ошибка: FirstName должен иметь DEFAULT ''.");
            },
            ["lastname"] = col => {
                if (col.Type != "text")     _logger.LogError("Ошибка: LastName должен быть TEXT.");
                if (col.NotNull)    _logger.LogError("Ошибка: LastName не должен быть NOT NULL (может быть пустым).");
            },
            ["phonenumber"] = col => {
                if (col.Type != "text")     _logger.LogError("Ошибка: PhoneNumber должен быть TEXT.");
                if (col.NotNull)    _logger.LogError("Ошибка: PhoneNumber не должен быть NOT NULL (может быть пустым).");
            },
            ["email"] = col => {
                if (col.Type != "text")             _logger.LogWarning                ("Ошибка: Email должен быть TEXT.");
                if (!col.NotNull) _logger.LogWarning   ("Ошибка: Email должен быть NOT NULL.");
            },
            ["password"] = col => {
                if (col.Type != "text")     _logger.LogError("Ошибка: Password должен быть TEXT.");
                if (!col.NotNull) _logger.LogError("Ошибка: Password должен быть NOT NULL.");
            },
            ["createdat"] = col => {
                if (col.Type != "datetime") _logger.LogError("Ошибка: CreatedAt должен быть DATETIME.");
                if (!col.NotNull) _logger.LogError("Ошибка: CreatedAt должен быть NOT NULL.");
            }
        };

        bool isStructureValid = true;

        foreach (var (colName, checkAction) in requiredColumns)
        {
            if (!columns.ContainsKey(colName))
            {
                _logger.LogError($"Ошибка: отсутствует столбец '{colName}'.");
                isStructureValid = false;
            }
            else
            {
                checkAction(columns[colName]);
            }
        }

        if (isStructureValid)
            _logger.LogInformation("Структура таблицы users корректна.");
        else
            _logger.LogError("Предупреждение: структура таблицы users содержит ошибки.");
    }    
}

