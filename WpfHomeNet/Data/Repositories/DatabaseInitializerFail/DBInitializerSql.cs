using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
public class DBInitializerSql
{
    private readonly string _connectionString;   
    private readonly ILogger _logger;
   

     public DBInitializerSql(ILogger logger,string connectionString)               
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
                  await CreateUsersTableAsync(connection);
        
        else await CheckTableStructureAsync(connection);
     
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

    {   _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        try
        {                       
            var sql = GenerateCreateTableSql();
            await connection.ExecuteAsync(sql);

            _logger.LogDebug("Таблица users успешно создана.");
        }

        catch (Exception ex)
        {
            _logger.LogError("Ошибка при создании таблицы users: {Error}", ex.Message);
            throw;
        }
    }


    private async Task CheckTableStructureAsync(SqliteConnection connection)
    {
        _logger.LogInformation("Проверяю структуру таблицы users...");

        var issues = new List<string>();

        try
        {
            // Получаем текущую схему из БД

            var actualColumns = connection.Query<dynamic>(@"PRAGMA table_info(users)")
           .ToDictionary(
           c => c.name.ToLower(),  // ключ словаря
           c => new                 // значение словаря
           {
               Type = c.type.ToLower(),
               NotNull = c.notnull == 1,
               DefaultValue = (c.dflt_value?.ToString() ?? "").ToLower()
           }
       );
            // Получаем ожидаемую схему из модели
            var expectedColumns = GetColumnMetadata();

            foreach (var expected in expectedColumns)
            {
                var colName = expected.Name.ToLower();

                if (!actualColumns.ContainsKey(colName))
                {
                    issues.Add($"Отсутствует столбец: {expected.Name}");
                    continue;
                }

                var actual = actualColumns[colName];

                // Проверяем тип
                if (actual.Type != expected.SqlType.ToLower())
                    issues.Add($"{expected.Name}: тип {actual.Type}, ожидается {expected.SqlType}");

                // Проверяем NOT NULL
                if (actual.NotNull != expected.IsRequired)
                    issues.Add($"{expected.Name}: NOT NULL={actual.NotNull}, ожидается {expected.IsRequired}");

                // Проверяем DEFAULT (если задан)
                if (!string.IsNullOrEmpty(expected.DefaultSql))
                {
                    var expectedDefault = expected.DefaultSql
                        .Replace("DEFAULT (", "")
                        .Replace(")", "")
                        .ToLower();

                    if (actual.DefaultValue != expectedDefault)
                        issues.Add($"{expected.Name}: DEFAULT '{actual.DefaultValue}', ожидается '{expectedDefault}'");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при проверке структуры таблицы users: {Message}", ex.Message);
            throw;
        }

        if (!issues.Any())
            _logger.LogInformation("Структура таблицы users корректна.");
        else
        {
            foreach (var issue in issues)
                _logger.LogError(issue);
            _logger.LogWarning($"Найдено {issues.Count} проблем в структуре таблицы users.");
        }
    }




    #region получение строки создания таблицы 

    private string GenerateCreateTableSql()
    {
        var columns = GetColumnMetadata()
            .Select(col => $"{col.Name} {col.SqlType} {(col.IsRequired ? "NOT NULL" : "")} {col.DefaultSql}".Trim())
            .ToList();

        return $"CREATE TABLE users ({string.Join(", ", columns)})";
    }


   

    private string GetSqlType(Type clrType)
    {
        clrType = Nullable.GetUnderlyingType(clrType) ?? clrType;
        return _typeRules.TryGetValue(clrType, out var rule) ? rule.SqlType : "TEXT";
    }

    private string GetDefaultSql(Type clrType)
    {
        clrType = Nullable.GetUnderlyingType(clrType) ?? clrType;
        return _typeRules.TryGetValue(clrType, out var rule) && rule.DefaultSql != null
            ? $"DEFAULT ({rule.DefaultSql})"
            : "";
    }

    private bool IsPropertyRequired(PropertyInfo prop)
    {
        var clrType = prop.PropertyType;
        var underlying = Nullable.GetUnderlyingType(clrType);
        var isNullable = underlying != null;
        var baseType = underlying ?? clrType;

        // Для value-типов: если не nullable → обязательно (NOT NULL)
        // Для reference-типов: смотрим на правило IsRequired
        return clrType.IsValueType
            ? !isNullable
            : (_typeRules.TryGetValue(baseType, out var rule) ? rule.IsRequired : false); 
    }

    #endregion




    private class DbColumnMetadata
    {
        public string Name { get; set; }
        public string SqlType { get; set; }
        public bool IsRequired { get; set; }
        public string DefaultSql { get; set; } // уже с "DEFAULT (...)"
    }


    private IEnumerable<DbColumnMetadata> GetColumnMetadata()
    {
        var properties = typeof(UserEntity).GetProperties();

        foreach (var prop in properties)
        {
            yield return new DbColumnMetadata
            {
                Name = prop.Name,
                SqlType = GetSqlType(prop.PropertyType),
                IsRequired = IsPropertyRequired(prop),
                DefaultSql = GetDefaultSql(prop.PropertyType)
            };
        }
    }


    private static readonly Dictionary<Type, (string SqlType, string DefaultSql, bool IsRequired)> _typeRules =
    new()
    {
        [typeof(int)] = ("INTEGER", null, true),
        [typeof(string)] = ("TEXT", "''", false), // DEFAULT '', но может быть NULL
        [typeof(DateTime)] = ("DATETIME", "DATETIME('now', 'localtime')", true)
    };




}

