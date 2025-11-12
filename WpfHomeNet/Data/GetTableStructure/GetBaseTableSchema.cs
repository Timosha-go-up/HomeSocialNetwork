using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeNet.Data.GetTableStructure
{
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    namespace WpfHomeNet.Data.GetTableStructure
    {

        public abstract class GetBaseTableSchema
        {
          public abstract Dictionary<string, ColumnMetadata> ExpectedColumns { get; }



            /// <summary>
            /// получаем метаданные таблицы от эскуэль
            /// </summary>
            /// <param name="tableName"></param>
            /// <returns></returns>
            protected abstract string GetMetadataQuery(string tableName);


            /// <summary>
            /// преабразуем данные запроса в понятную структуру
            /// </summary>
            /// <param name="row"></param>
            /// <returns></returns>
            protected abstract ColumnMetadata MapRowToColumn(dynamic row);





            public async Task<Dictionary<string, ColumnMetadata>>



            GetActualColumnsAsync(DbConnection connection, string tableName)
            {
                var sql = GetMetadataQuery(tableName);
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var rows = await connection.QueryAsync<dynamic>(sql);
                var result = new Dictionary<string, ColumnMetadata>();
                foreach (var row in rows)
                    result[row.name.ToString().ToLower()] = MapRowToColumn(row);

                return result;
            }




            // Добавляем: универсальный метод сравнения!
            public List<string> CompareWithExpected(
                Dictionary<string, ColumnMetadata> actual,
                Dictionary<string, ColumnMetadata> expected)
            {
                var issues = new List<string>();

                // Проверяем, все ли ожидаемые колонки есть
                foreach (var (name, expectedCol) in expected)
                {
                    if (!actual.ContainsKey(name.ToLower()))
                    {
                        issues.Add($"Отсутствует колонка: {name}");
                        continue;
                    }

                    var actualCol = actual[name.ToLower()];
                    if (!AreColumnsEqual(expectedCol, actualCol))
                        issues.Add($"{name}: расхождение — ожидалось {expectedCol}, найдено {actualCol}");
                }

                // Можно добавить проверку «лишних» колонок в actual (по желанию)
                return issues;
            }

            private bool AreColumnsEqual(ColumnMetadata expected, ColumnMetadata actual)
            {
                return string.Equals(expected.Type, actual.Type, StringComparison.OrdinalIgnoreCase) &&
                       expected.IsNotNull == actual.IsNotNull &&
                       string.Equals(expected.DefaultValue, actual.DefaultValue, StringComparison.OrdinalIgnoreCase);
            }
        }



        public class SqliteTableSchema : GetBaseTableSchema
        {
            protected override string GetMetadataQuery(string tableName)
            {
                return $"PRAGMA table_info({tableName})";
            }


            protected override ColumnMetadata MapRowToColumn(dynamic row)
            {
                return new ColumnMetadata
                {
                    Name = row.name,
                    Type = row.type,
                    IsNotNull = row.notnull == 1,  // 1 → true, 0 → false
                    DefaultValue = row.dflt_value
                };
            }


            public override Dictionary<string, ColumnMetadata> ExpectedColumns => new()
            {
                ["id"] = new ColumnMetadata { Name = "id", Type = "integer", IsNotNull = true, DefaultValue = null },
                ["email"] = new ColumnMetadata { Name = "email", Type = "text", IsNotNull = true, DefaultValue = null },
               
            };
        }






        public class PostgreSqlTableSchema : GetBaseTableSchema
        {
            protected override string GetMetadataQuery(string tableName)
            {
                return @"SELECT column_name, data_type, is_nullable, column_default
                 FROM information_schema.columns
                 WHERE table_name = @tableName";
            }



            protected override ColumnMetadata MapRowToColumn(dynamic row)
            {
                return new ColumnMetadata
                {
                    Name = row.column_name,
                    Type = row.data_type,
                    IsNotNull = row.is_nullable == "NO",  // "NO" → true, "YES" → false
                    DefaultValue = row.column_default
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
