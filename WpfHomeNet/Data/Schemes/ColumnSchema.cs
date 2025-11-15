using WpfHomeNet.Data.Bilders;

namespace WpfHomeNet.Data.Schemes
{
    using System;

    public class ColumnSchema
    {
        public string? Name { get; set; }
        public ColumnType Type { get; set; }
        public int? Length { get; set; }
        public bool IsNotNull { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }  // Новое свойство
        public bool IsAutoIncrement { get; set; }
        public DateTime? CreatedAt { get; set; }  // Новое свойство
        public string? DefaultString { get; set; }
        public string? Comment { get; internal set; }
        public int? DefaultInt { get; internal set; }
        public DateTime? DefaultDateTime { get; internal set; }
        public bool? DefaultBool { get; internal set; }
    }
}





