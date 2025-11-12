using Dapper;
using HomeSocialNetwork.Helpers;
using System.Data;
using System.Data.Common;
using WpfHomeNet.Data.GetTableStructure.WpfHomeNet.Data.GetTableStructure;
using WpfHomeNet.Data.TableUserBDs;
public class DBInitializer
{
    IDbConnection _dbConnection;
    private readonly BaseUsersTable _usersTable;     
    private readonly ILogger _logger;
    private readonly GetBaseTableSchema _schemaProvider;
   

    public DBInitializer(
        IDbConnection connection ,
        BaseUsersTable usersTable,        
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
       
        return await _dbConnection.ExecuteScalarAsync<int>
        (_usersTable.GenerateTableExistsSql(), new { tableName = _usersTable.TableName }) > 0;
    }


    private async Task CreateUsersTableAsync()
    {
        _logger.LogWarning("Таблица users не найдена. Создаю новую...");
        try
        {                        
            await _dbConnection.ExecuteAsync(_usersTable.GenerateCreateTableSql());

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
        _logger.LogInformation("Проверяю структуру таблицы {Table}...", _usersTable.TableName);

        var actualColumns = await _schemaProvider.GetActualColumnsAsync(_connection, _usersTable.TableName);
        var expectedColumns = _usersTable.ExpectedColumns; // Dictionary<string, ColumnMetadata>

        var issues = _schemaProvider.CompareWithExpected(actualColumns, expectedColumns);

        if (!issues.Any())
        {
            _logger.LogInformation("Структура таблицы {Table} корректна.", _usersTable.TableName);
        }
        else
        {
            foreach (var issue in issues)
                _logger.LogError(issue);
            _logger.LogWarning("Найдено {Count} проблем в структуре таблицы {Table}.", issues.Count, _usersTable.TableName);
        }
    }










}

