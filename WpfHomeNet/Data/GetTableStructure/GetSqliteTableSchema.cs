using WpfHomeNet.Data.TableUserBDs;



namespace WpfHomeNet.Data.GetTableStructure

{
    public class GetSqliteTableSchema : GetBaseTableSchema
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
    }

}

