using Dapper;
using System.Data;
using System.Data.Common;
using WpfHomeNet.Data.GetTableStructure;
using WpfHomeNet.Data.TableUserBDs;
using WpfHomeNet.Helpers;
public class DBInitializer
{
    IDbConnection _dbConnection;
    private readonly IBaseUsersTable _usersTable;
    private readonly ILogger _logger;
    private readonly GetBaseTableSchema _schemaProvider;
   
    public DBInitializer(
        IDbConnection connection,
        IBaseUsersTable usersTable,
        GetBaseTableSchema schemaProvider,
        ILogger logger)
    {
        _dbConnection = connection;
        _usersTable = usersTable;
        _schemaProvider = schemaProvider;
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
        if (_dbConnection.State != ConnectionState.Open)
        {
            await _dbConnection.OpenAsync();
        }
        return await _dbConnection.ExecuteScalarAsync<int>
        (_usersTable.GenerateTableExistsSql(), new { tableName = _usersTable.TableName }) > 0;
    }


    private async Task CreateUsersTableAsync()
    {
        _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                await _dbConnection.OpenAsync();
            }
            await _dbConnection.ExecuteAsync(_usersTable.GenerateCreateTableSql());

            if (await TableExistsAsync()) _logger.LogDebug("Таблица users успешно создана.");

            else
            {
                _logger.LogError(
        "Таблица users не создана. " +
        $"SQL: {_usersTable.GenerateCreateTableSql()}, " +
        $"Соединение: connectionState{_dbConnection.State}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при создании таблицы users: {Error}", ex.Message);
            throw;
        }
    }


    private async Task CheckTableStructureAsync()
    {
        _logger.LogInformation("Проверяю структуру таблицы {Table}...", _usersTable.TableName);

        try
        {
            await _dbConnection.OpenAsync();

            // Создаём actualColumns прямо здесь — только для этой проверки
            var actualColumns = await _schemaProvider.GetActualColumnsAsync(
                (DbConnection)_dbConnection,
                _usersTable.TableName
            );

            // Создаём expectedColumns здесь — только для сравнения
            var expectedColumns = _usersTable.Columns.ToDictionary(c => c.Key.ToLower(), c => c.Value);
               

            _logger.LogDebug($"Ожидается колонок: ExpectedCount{expectedColumns.Count}, найдено: ActualCount {actualColumns.Count}");

            var issues = _schemaProvider.CompareWithExpected(actualColumns, expectedColumns);


            if (!issues.Any())
            {
                _logger.LogInformation("Структура таблицы {Table} корректна.", _usersTable.TableName);
            }
            else
            {
                foreach (var issue in issues)
                _logger.LogError(issue);
                _logger.LogWarning($"Найдено IssueCount{issues.Count} проблем в структуре таблицы Table{_usersTable.TableName}.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при проверке структуры таблицы: {Error}", ex.Message);
        }
    }




}

