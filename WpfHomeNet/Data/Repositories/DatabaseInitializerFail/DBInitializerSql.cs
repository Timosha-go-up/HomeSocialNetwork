using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WpfHomeNet.Helpers;
using System.Collections.Generic;
public class DBInitializerSql
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public DBInitializerSql(ILogger logger, string connectionString)
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
        else
            await CheckTableStructureAsync(connection);
    }

    private void LogInitializationStarted() =>
        _logger.LogDebug("Инициализация БД: проверка таблицы users...");

    private async Task<bool> TableExistsAsync(SqliteConnection connection, string tableName)
    {
        var sql = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @tableName";
        return await connection.ExecuteScalarAsync<int>(sql, new { tableName = UsersTable.TableName }) > 0;
    }


    private async Task CreateUsersTableAsync(SqliteConnection connection)
    {
        _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        try
        {
            // Формируем SQL для создания таблицы
            var createSql = $@"
            CREATE TABLE {UsersTable.TableName} (
                {string.Join(", ", UsersTable.Columns.Values)}
            )";

            await connection.ExecuteAsync(createSql);
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
        var actualColumns = await GetActualColumnsAsync(connection);
        var expectedColumns = UsersTable.Columns; // Берём готовую схему из UsersTable

        foreach (var expected in expectedColumns)
        {
            var colName = expected.Key.ToLower();
            if (!actualColumns.ContainsKey(colName))
            {
                issues.Add($"Отсутствует столбец: {expected.Key}");
                continue;
            }

            var actual = actualColumns[colName];
            var expectedDef = expected.Value.ToString().ToLower();

            if (actual != expectedDef)
                issues.Add($"{expected.Key}: ожидается '{expectedDef}', найдено '{actual}'");
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


    private async Task<Dictionary<string, string>> GetActualColumnsAsync(SqliteConnection connection)
    {
        var rows = await connection.QueryAsync<dynamic>("PRAGMA table_info(users)");

        var dictionary = new Dictionary<string, string>();

        foreach (var row in rows)
        {
            // Безопасное приведение к строке с обработкой null
            var name = (row.name?.ToString() ?? "").ToLower();
            var type = (row.type?.ToString() ?? "").ToLower();
            var notNull = row.notnull?.Equals(1) ?? false;
            var dfltValue = row.dflt_value?.ToString() ?? "";

            // Формируем определение колонки
            var definitionParts = new List<string> { type };

            if (notNull)
                definitionParts.Add("NOT NULL");

            if (!string.IsNullOrEmpty(dfltValue))
                definitionParts.Add($"DEFAULT {dfltValue}");

            var definition = string.Join(" ", definitionParts).Trim().ToLower();

            // Добавляем в словарь (избегаем дубликатов)
            if (!dictionary.ContainsKey(name))
                dictionary[name] = definition;
        }

        return dictionary;
    }






}

