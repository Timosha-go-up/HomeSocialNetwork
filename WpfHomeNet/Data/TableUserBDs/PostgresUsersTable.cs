using WpfHomeNet.Data.Generators;

namespace WpfHomeNet.Data.TableUserBDs
{
    public class PostgresUsersTable 
    {
       

        public PostgresUsersTable()
        {
            // Задаём SqlType для каждой колонки
            Id.Type = "SERIAL PRIMARY KEY";
            FirstName.Type = "VARCHAR(100) NOT NULL";
            LastName.Type = "VARCHAR(100)";
            PhoneNumber.Type = "VARCHAR(100)";
            Email.Type = "VARCHAR(254) NOT NULL UNIQUE";
            Password.Type = "VARCHAR(255) NOT NULL";
            CreatedAt.Type = "TIMESTAMP DEFAULT NOW()";
        }

        #region методы запросы
        public override string GenerateCreateTableSql()
        {
            var columnsSql = string.Join(", ", Columns.Values.Select(c =>
                $"{c.Name} {c.Type}"));
            return $"CREATE TABLE {TableName} ({columnsSql})";
        }

        public override string GenerateInsertSql()
        {
            var fields = DataFields;
            var values = string.Join(", ", DataFields.Split(',').Select(f => $"@{f.Trim()}"));
            return $"INSERT INTO {TableName} ({fields}) VALUES ({values}) RETURNING Id";
        }

        public override string GenerateSelectByIdSql()
            => $"SELECT {AllFields} FROM {TableName} WHERE Id = @Id";

        public override string GenerateSelectByEmailSql()
            => $"SELECT {AllFields} FROM {TableName} WHERE Email = @Email";


        public override string GenerateUpdateSql()
        {
            var setClause = string.Join(", ", DataFields.Split(',')
                .Select(f => $"{f.Trim()} = @{f.Trim()}"));
            return $"UPDATE {TableName} SET {setClause} WHERE Id = @Id";
        }

        public override string GenerateDeleteSql()
            => $"DELETE FROM {TableName} WHERE Id = @Id";

        public override string GenerateTableExistsSql()
    => $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{TableName}')";

        public override string GenerateAddColumnSql(ColumnMetadata column)
        {
            return $"ALTER TABLE {TableName} ADD COLUMN IF NOT EXISTS {column.Name} {column.Type}";
        }


        public override string GenerateSelectAllSql()
        {
            var columns = string.Join(", ", Columns.Keys);
            return $"SELECT {columns} FROM {TableName}";
        }
        #endregion
    }
}
