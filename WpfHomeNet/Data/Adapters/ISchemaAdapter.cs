using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHomeNet.Data.Schemes;

namespace WpfHomeNet.Data.Adapters
{
    public interface ISchemaAdapter
    {
        string GetTableName(string rawName);
        List<string> GetColumnDefinitions(TableSchema schema);
    }



}
