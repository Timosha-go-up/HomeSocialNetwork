using System.Data;
using WpfHomeNet.Data.Generators;

namespace WpfHomeNet.Data.TableUserBDs
{
    public class SqliteUsersTable 
    {
        



        #region методы запросы 
        public  string GenerateCreateTableSql()
        {
            var columnsSql = string.Join(", ", Columns.Values.Select(c => $"{c.Name} {c.DataType}"));
            return $"CREATE TABLE {TableName} ({columnsSql})";
        }

        public  string GenerateInsertSql()
        {
            var fields = DataFields;
            var values = string.Join(", ", DataFields.Split(',').Select(f => $"@{f.Trim()}"));
            return $"INSERT INTO {TableName} ({fields}) VALUES ({values})";
        }

        public  string GenerateSelectByIdSql() =>
            $"SELECT {AllFields} FROM {TableName} WHERE Id = @Id";


        public  string GenerateSelectByEmailSql() =>
            $"SELECT {AllFields} FROM {TableName} WHERE Email = @Email";

        public  string GenerateUpdateSql()
        {
            var setClause = string.Join(", ", DataFields.Split(',')
                .Select(f => $"{f.Trim()} = @{f.Trim()}"));
            return $"UPDATE {TableName} SET {setClause} WHERE Id = @Id";
        }

        public  string GenerateDeleteSql() =>
            $"DELETE FROM {TableName} WHERE Id = @Id";


        public  string GenerateTableExistsSql() =>
            $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{TableName}'";


        public  string GenerateAddColumnSql(ColumnMetadata column) =>
            $"ALTER TABLE {TableName} ADD COLUMN {column.Name} {column.DataType}";



        public  string GenerateSelectAllSql()
        {
            var columns = string.Join(", ", Columns.Keys);
            return $"SELECT {columns} FROM {TableName}";
        }
        #endregion

    }

}

