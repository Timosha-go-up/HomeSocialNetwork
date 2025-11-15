using WpfHomeNet.Data.Generators;

namespace WpfHomeNet.Data.Bilders
{
    using System;
    using WpfHomeNet.Data.Schemes;

    public enum ColumnType { Int, Varchar, DateTime, Boolean }

    public class ColumnBuilder
    {
        private string? _name;
        private ColumnType _type;
        private int? _length;
        private bool _isNotNull;
        private bool _isPrimaryKey;
        private bool _isUnique;
        private bool _isAutoIncrement;
        private DateTime? _createdAt;
        private string? _defaultString;
        private string? _comment;
        private int? _defaultInt;
        private bool? _defaultBool;
        private DateTime? _defaultDateTime;
        public ColumnBuilder(string name) => _name = name;

        public ColumnBuilder WithType(ColumnType type)
        {
            _type = type;
            return this;
        }

        public ColumnBuilder Varchar(int length)
        {
            if (length < 1 || length > 65535)
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Length must be between 1 and 65535");

            _type = ColumnType.Varchar;
            _length = length;
            return this;
        }

        public ColumnBuilder Integer()
        {
            _type = ColumnType.Int;
            return this;
        }

        public ColumnBuilder NotNull()
        {
            _isNotNull = true;
            return this;
        }

        public ColumnBuilder PrimaryKey()
        {
            _isPrimaryKey = true;
            return this;
        }

        public ColumnBuilder Unique()
        {
            _isUnique = true;
            return this;
        }

        public ColumnBuilder AutoIncrement()
        {
            _isAutoIncrement = true;
            return this;
        }

        public ColumnBuilder CreatedAt(DateTime? timestamp = null)
        {
            _createdAt = timestamp ?? DateTime.UtcNow;
            return this;
        }

        public ColumnBuilder Default(string value)
        {
            _defaultString = value;
            return this;
        }


        public ColumnBuilder Default(int value)
        {
            _defaultInt = value;
            return this;
        }

        public ColumnBuilder Default(DateTime value)
        {
            _defaultDateTime = value;
            return this;
        }

        public ColumnBuilder Default(bool value)
        {
            _defaultBool = value;
            return this;
        }

        public ColumnBuilder Comment(string text)
        {
            _comment = text;
            return this;
        }

        public ColumnSchema Build()
        {
            if (_isAutoIncrement && (_type != ColumnType.Int || !_isPrimaryKey))
                throw new InvalidOperationException(
                    "AutoIncrement can only be applied to Int primary keys");

            if (_type == ColumnType.Varchar && !_length.HasValue)
                throw new InvalidOperationException("Length must be specified for Varchar columns");

            return new ColumnSchema
            {
                Name = _name,
                Type = _type,
                Length = _length,
                IsNotNull = _isNotNull,
                IsPrimaryKey = _isPrimaryKey,
                IsUnique = _isUnique,
                IsAutoIncrement = _isAutoIncrement,
                CreatedAt = _createdAt,
                DefaultString = _defaultString,
                DefaultInt = _defaultInt,
                DefaultDateTime = _defaultDateTime,
                DefaultBool = _defaultBool,
                Comment = _comment
            };
        }
    }

}





