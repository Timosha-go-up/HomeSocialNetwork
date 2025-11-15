using WpfHomeNet.Data.TableUserBDs;



namespace WpfHomeNet.Data.GetTableStructure

{
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






}

