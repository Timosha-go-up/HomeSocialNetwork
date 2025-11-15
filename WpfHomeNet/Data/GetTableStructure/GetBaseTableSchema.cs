using Dapper;
using WpfHomeNet.Data.TableUserBDs;
using System.Data;
using System.Data.Common;



namespace WpfHomeNet.Data.GetTableStructure

{
    public abstract class GetBaseTableSchema
    {
        /// <summary>
        /// получить метаданные запроса
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected abstract string GetMetadataQuery(string tableName);


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
        public List<string> CompareWithExpected( Dictionary<string, ColumnMetadata> actual, Dictionary<string, ColumnMetadata> expected )
           
          
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

            return issues;
        }

        private bool AreColumnsEqual(ColumnMetadata expected, ColumnMetadata actual)
        {
            return string.Equals(expected.Type, actual.Type, StringComparison.OrdinalIgnoreCase) &&
                   expected.IsNotNull == actual.IsNotNull &&
                   string.Equals(expected.DefaultValue, actual.DefaultValue, StringComparison.OrdinalIgnoreCase);
        }


        
}
    }






}

