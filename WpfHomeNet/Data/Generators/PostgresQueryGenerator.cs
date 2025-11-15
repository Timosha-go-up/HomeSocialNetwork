using WpfHomeNet.Data.Adapters;
using WpfHomeNet.Data.Bilders;
using WpfHomeNet.Data.Schemes;

namespace WpfHomeNet.Data.Generators
{
    public class PostgresSqlGenerator  
    {
        public string GenerateCreateTableSql(TableSchema schema, ISchemaAdapter adapter)
        {
            var tableName = adapter.GetTableName(schema.TableName);
            var columns = string.Join(",\n  ", adapter.GetColumnDefinitions(schema));

            return $"CREATE TABLE \"{tableName}\" (\n  {columns}\n);";
        }
    }

}
