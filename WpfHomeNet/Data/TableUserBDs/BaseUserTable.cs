using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHomeNet.Data.GetTableStructure.WpfHomeNet.Data.GetTableStructure;

namespace WpfHomeNet.Data.TableUserBDs
{


    public abstract class BaseUsersTable
    {
        public string TableName => "users";


        // Колонки — как свойства
        public Column Id => new Column("Id");
        public Column FirstName => new Column("FirstName");
        public Column LastName => new Column("LastName");
        public Column PhoneNumber => new Column("PhoneNumber");
        public Column Email => new Column("Email");
        public Column Password => new Column("Password");
        public Column CreatedAt => new Column("CreatedAt");


        // Обязательный для наследника словарь колонок
        public abstract IReadOnlyDictionary<string, Column> Columns { get; }


        // Утилита для сборки словаря колонок
        protected IReadOnlyDictionary<string, Column> DefineColumns(params Column[] columns)
            => columns.ToDictionary(col => col.Name);


        // Готовые строки для SQL
        public string AllFields => string.Join(", ", Columns.Keys);
        public string DataFields => string.Join(", ", Columns.Keys.Where(k => k != "Id"));
       

        public abstract   string GenerateCreateTableSql();
       public abstract string GenerateInsertSql();
       public abstract string  GenerateSelectByIdSql();         
       public abstract string  GenerateSelectByEmailSql();
       public abstract string  GenerateUpdateSql();
       public abstract string  GenerateDeleteSql();
       public abstract string  GenerateTableExistsSql();
       public abstract string  GenerateAddColumnSql(Column column);
        public abstract string GenerateSelectAllSql();

        // Вложенный класс Column
        public class Column
        {
            public string Name { get; }
            public string? SqlType { get;  set; }
            public string? Parameter { get; set; }
            public Column(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Имя колонки не может быть пустым");
                Name = name;
            }
        }
    }

}







