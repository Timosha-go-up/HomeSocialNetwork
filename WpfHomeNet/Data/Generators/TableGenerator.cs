using WpfHomeNet.Data.Bilders;
using WpfHomeNet.Data.Schemes;

namespace WpfHomeNet.Data.Generators
{
    public class TableGenerator
    {
        private readonly string _tableName;
        private readonly List<ColumnSchema> _columns = new();

        public TableGenerator(string tableName) => _tableName = tableName;

        public ColumnBuilder AddColumn(string name)
        {
            var builder = new ColumnBuilder(name);
            _columns.Add(builder.Build());
            return builder;
        }

        public TableSchema Generate() => new TableSchema
        {
            TableName = _tableName,
            Columns = _columns
        };
    }

}
