using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeNet.Data.GetTableStructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dapper;
    namespace WpfHomeNet.Data.GetTableStructure
    {

        public abstract class GetBaseTableSchema
        {
            protected abstract string GetMetadataQuery(string tableName);
            protected abstract ColumnMetadata MapRowToColumn(dynamic row);

            public async Task<Dictionary<string, ColumnMetadata>>
            GetActualColumnsAsync(DbConnection connection, string tableName)

            {
                var sql = GetMetadataQuery(tableName);

                // Открываем соединение, если закрыто
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                var rows = await connection.QueryAsync<dynamic>(sql);
                var result = new Dictionary<string, ColumnMetadata>();

                foreach (var row in rows)
                {
                    var column = MapRowToColumn(row);
                    result[column.Name.ToLower()] = column;
                }

                return result;
            }
        }


        public class SqliteTableSchema : GetBaseTableSchema
        {
            protected override string GetMetadataQuery(string tableName)
                => $"PRAGMA table_info({tableName})";

            protected override ColumnMetadata MapRowToColumn(dynamic row)
            {
                return new ColumnMetadata
                {
                    Name = row.name?.ToString() ?? "",
                    Type = (row.type?.ToString() ?? "").ToLower(),
                    IsNotNull = row.notnull?.Equals(1) ?? false,
                    DefaultValue = row.dflt_value?.ToString()
                };
            }
        }


        public class PostgresTableSchema : GetBaseTableSchema
        {
            protected override string GetMetadataQuery(string tableName) => @"
        SELECT column_name, data_type, is_nullable, column_default
        FROM information_schema.columns
        WHERE table_name = @tableName";

            protected override ColumnMetadata MapRowToColumn(dynamic row)
            {
                return new ColumnMetadata
                {
                    Name = row.column_name?.ToString() ?? "",
                    Type = (row.data_type?.ToString() ?? "").ToLower(),
                    IsNotNull = row.is_nullable?.ToString() == "NO",
                    DefaultValue = row.column_default?.ToString()
                };
            }
        }

        public class ColumnMetadata
        {
            public string? Name { get; set; }
            public string? Type { get; set; }
            public bool IsNotNull { get; set; }
            public string? DefaultValue { get; set; }

        }
    }
}
