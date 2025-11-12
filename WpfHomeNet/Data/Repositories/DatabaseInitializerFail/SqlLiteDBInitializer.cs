using HomeSocialNetwork.Helpers;
using WpfHomeNet.Data.TableUserBDs;
public class SqlLiteDBInitializer
{  
    IDbconn
    private readonly BaseUsersTable _usersTable; 
    private readonly string _connectionString;
    private readonly ILogger _logger;
   
   

    public SqlLiteDBInitializer(BaseUsersTable usersTable, string connectionString, ILogger logger)
    {
        _usersTable = usersTable;
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogDebug("Инициализация БД: проверка таблицы users...");

        

        if (!await TableExistsAsync())
        {
            await CreateUsersTableAsync(); 
        }

        else
        {
            await CheckTableStructureAsync();
        }            
    }
        
    private async Task<bool> TableExistsAsync()
    {
       
        return await _connection.ExecuteScalarAsync<int>(_usersTable.GenerateTableExistsSql(), new { tableName = _usersTable.TableName }) > 0;
    }


    private async Task CreateUsersTableAsync()
    {
        _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        try
        {                        
            await connection.ExecuteAsync(_usersTable.GenerateCreateTableSql());

            _logger.LogDebug("Таблица users успешно создана.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при создании таблицы users: {Error}", ex.Message);
            throw;
        }
    }


    private async Task CheckTableStructureAsync()
    {
        _logger.LogInformation("Проверяю структуру таблицы users...");

        var issues = new List<string>();
        var actualColumns = await GetActualColumnsAsync(connection);
        var expectedColumns = _usersTable.Columns; // Берём готовую схему из UsersTable

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


    private async Task<Dictionary<string, string>> GetActualColumnsAsync(IDbConnection connection)
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

