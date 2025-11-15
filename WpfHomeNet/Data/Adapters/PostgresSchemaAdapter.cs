using System.Text;
using WpfHomeNet.Data.Bilders;
using WpfHomeNet.Data.Schemes;

namespace WpfHomeNet.Data.Adapters
{
    public class PostgresSchemaAdapter : ISchemaAdapter
    {
        public string GetTableName(string rawName) =>
            ToSnakeCase(rawName);

        public List<string> GetColumnDefinitions(TableSchema schema)
        {
            return schema.Columns.Select(col =>
            {
                // 1. Имя колонки в snake_case
                var name = $"\"{ToSnakeCase(col.Name)}\"";

                // 2. Определяем SQL‑тип
                string sqlType;
                if (col.IsAutoIncrement && col.Type == ColumnType.Int)
                    sqlType = "SERIAL";
                else
                    sqlType = col.Type switch
                    {
                        ColumnType.Varchar => $"VARCHAR({col.Length})",
                        ColumnType.Int => "INTEGER",
                        ColumnType.DateTime => "TIMESTAMP",
                        ColumnType.Boolean => "BOOLEAN",
                        _ => col.Type.ToString()
                    };

                // 3. Собираем ограничения
                var constraints = new List<string>();
                if (col.IsNotNull) constraints.Add("NOT NULL");
                if (col.IsPrimaryKey) constraints.Add("PRIMARY KEY");
                if (col.IsUnique) constraints.Add("UNIQUE");

                // 4. Формируем итоговую строку
                return $"{name} {sqlType} {string.Join(" ", constraints)}";
            })
            .ToList();
        }

        private string ToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var builder = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        builder.Append('_');
                    builder.Append(char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

    }



}
