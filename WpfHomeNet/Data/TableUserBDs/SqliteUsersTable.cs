using System.Data;

namespace WpfHomeNet.Data.TableUserBDs
{
    public class SqliteUsersTable : BaseUsersTable
    {
        private const string TextNotNull = "TEXT NOT NULL";
        private const string DateTimeDefault = "DATETIME DEFAULT (datetime('now', 'localtime'))";

        public SqliteUsersTable()
        {
            Id.SqlType = "INTEGER PRIMARY KEY AUTOINCREMENT";
            FirstName.SqlType = TextNotNull;
            LastName.SqlType = "TEXT";
            PhoneNumber.SqlType = "TEXT";
            Email.SqlType = "TEXT NOT NULL UNIQUE";
            Password.SqlType = TextNotNull;
            CreatedAt.SqlType = DateTimeDefault;
        }

        public override IReadOnlyDictionary<string, Column> Columns =>
            DefineColumns(Id, FirstName, LastName, PhoneNumber, Email, Password, CreatedAt);

        // Реализация интерфейса ITableSqlOperations
        public override string GenerateCreateTableSql()
        {
            var columnsSql = string.Join(", ", Columns.Values.Select(c => $"{c.Name} {c.SqlType}"));
            return $"CREATE TABLE {TableName} ({columnsSql})";
        }

        public override string GenerateInsertSql()
        {
            var fields = DataFields;
            var values = string.Join(", ", DataFields.Split(',').Select(f => $"@{f.Trim()}"));
            return $"INSERT INTO {TableName} ({fields}) VALUES ({values})";
        }

        public override string GenerateSelectByIdSql() =>
            $"SELECT {AllFields} FROM {TableName} WHERE Id = @Id";


        public override string GenerateSelectByEmailSql() =>
            $"SELECT {AllFields} FROM {TableName} WHERE Email = @Email";

        public override string GenerateUpdateSql()
        {
            var setClause = string.Join(", ", DataFields.Split(',')
                .Select(f => $"{f.Trim()} = @{f.Trim()}"));
            return $"UPDATE {TableName} SET {setClause} WHERE Id = @Id";
        }

        public override string GenerateDeleteSql() =>
            $"DELETE FROM {TableName} WHERE Id = @Id";


        public override string GenerateTableExistsSql() =>
            $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{TableName}'";


        public override string GenerateAddColumnSql(Column column) =>
            $"ALTER TABLE {TableName} ADD COLUMN {column.Name} {column.SqlType}";



        public override string GenerateSelectAllSql()
        {
            var columns = string.Join(", ", Columns.Keys);
            return $"SELECT {columns} FROM {TableName}";
        }


      


    }

}







