using Microsoft.Data.Sqlite;
using System.Data;
using Npgsql;
using Microsoft.Data.SqlClient;

namespace WpfHomeNet.Data.ConnectionFactories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
   
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }


   


    public class PostgreSQLConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public PostgreSQLConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }


    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }




}
