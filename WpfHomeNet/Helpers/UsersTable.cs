using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace WpfHomeNet.Helpers
 {
    public static class UsersTable
    {
        public static string TableName => "users";


        public static readonly SqliteColumn Id = new SqliteColumn("Id", "INTEGER PRIMARY KEY")
            .WithNotNull();
          

        public static readonly SqliteColumn FirstName = new SqliteColumn("FirstName", "TEXT")
            .WithNotNull();

        public static readonly SqliteColumn LastName = new SqliteColumn("LastName", "TEXT");

        public static readonly SqliteColumn PhoneNumber = new SqliteColumn("PhoneNumber", "TEXT");

        public static readonly SqliteColumn Email = new SqliteColumn("Email", "TEXT");
        

        public static readonly SqliteColumn Password = new SqliteColumn("Password", "TEXT")
            .WithNotNull();

        public static readonly SqliteColumn CreatedAt = new SqliteColumn("CreatedAt", "DATETIME")
            .WithNotNull()
            .WithDefault("(DATETIME('now'))");

        // Собираем все колонки в словарь (удобно для итерации)
        public static readonly Dictionary<string, SqliteColumn> Columns = new()
        {
            [Id.Name] = Id,
            [FirstName.Name] = FirstName,
            [LastName.Name] = LastName,
            [PhoneNumber.Name] = PhoneNumber,
            [Email.Name] = Email,
            [Password.Name] = Password,
            [CreatedAt.Name] = CreatedAt
        };

        // Генерируем SQL для CREATE TABLE
        public static string AllFields => string.Join(", ", Columns.Keys);
        public static string DataFields => string.Join(", ",
            Columns.Keys.Where(k => k != "Id"));  // без Id
    }


    public class SqliteColumn
    {
        public string Name { get; }
        public string SqlType { get; }  // TEXT, INTEGER, DATETIME и т. п.
        public bool NotNull { get; set; } = false;
        public string? DefaultValue { get; set; } = null;  // например, 'now' для DATETIME

        public SqliteColumn(string name, string sqlType)
        {
            Name = name;
            SqlType = sqlType;
        }

        public SqliteColumn WithNotNull()
        {
            NotNull = true;
            return this;
        }

        public SqliteColumn WithDefault(string value)
        {
            DefaultValue = value;
            return this;
        }

        // Для генерации части SQL: "Name TEXT NOT NULL DEFAULT 'now'"
        public override string ToString()
        {
            var parts = new List<string> { Name, SqlType };
            if (NotNull) parts.Add("NOT NULL");
            if (DefaultValue != null) parts.Add($"DEFAULT {DefaultValue}");
            return string.Join(" ", parts);
        }
    }





}
