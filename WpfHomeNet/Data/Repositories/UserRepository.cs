using Dapper;
using HomeSocialNetwork.Helpers;
using HomeSocialNetwork.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using WpfHomeNet.Data.TableUserBDs;

namespace WpfHomeNet.Data.Repositories
{
   public class UserRepository:IUserRepository
    {
        private readonly IDbConnection _connection;
        BaseUsersTable _usersTable;
        private readonly ILogger _logger;

        public UserRepository(IDbConnection connection, ILogger logger,BaseUsersTable usersTable)
        {
            _connection = connection ??
                throw new ArgumentNullException(nameof(connection));
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _usersTable = usersTable;
        }

        public async Task<UserEntity> InsertUserAsync(UserEntity user)
        {
            try
            {
                var sql = _usersTable.GenerateInsertSql();
                var newId = await _connection.QuerySingleAsync<int>(sql, user);
                user.Id = newId;
                return user;
            }
            catch (SqlException ex)
            {
                    _logger.LogError(
                    $"SQL Error ErrorCode:{ex.Number}" +
                    $" ErrorMessage {ex.Message} " +
                    $" UserEmail: {user.Email}");
                                                                                          
                throw; // просто перебрасываем дальше — лог уже есть
            }
        }



        public async Task DeleteByIdAsync(int id)
        {
            
            var affectedRows = 
            await _connection.ExecuteAsync(_usersTable.GenerateDeleteSql(),new { Id = id });
                               
            if (affectedRows == 0)
            {
                throw new NotFoundException($"Пользователь с ID {id} не найден.");
            }

            _logger.LogInformation($"Пользователь с ID {id} удалён.");
        }



        public async Task<List<UserEntity>> GetAllAsync()
        {
           
            var users = (await _connection.QueryAsync<UserEntity>
            (_usersTable.GenerateSelectAllSql())).ToList(); 

           _logger.LogInformation($"Получено {users.Count} пользователей.");
           return users;
        }


       
        public async Task<UserEntity?> GetByIdAsync(int id)
        {            
            return await _connection.QueryFirstOrDefaultAsync<UserEntity>
            (_usersTable.GenerateSelectByIdSql(), new { Id = id });               
        }


        public async Task<UserEntity?> GetByEmailAsync(string email)
        {           
            return await _connection.QueryFirstOrDefaultAsync<UserEntity>
           (_usersTable.GenerateSelectByEmailSql(),new { Email = email });
        }


        public async Task UpdateAsync(UserEntity user)
        {           
            await _connection.ExecuteAsync
           (_usersTable.GenerateUpdateSql(),user);

            _logger.LogInformation($"Пользователь {user.Id} обновлён.");
        }

    }
}
