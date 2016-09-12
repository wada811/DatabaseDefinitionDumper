using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class ViewColumn
    {
        public Database Database { get; private set; }
        public View View { get; private set; }
        public int ColumnId { get; private set; }
        public string Name { get; private set; }
        public string TypeName { get; private set; }
        public int Length { get; private set; }
        public int Precision { get; private set; }
        public int Scale { get; private set; }
        public bool IsNullable { get; private set; }
        public string DefaultValue { get; private set; }
        public string PrimaryKeyName { get; private set; }
        public int? PrimaryKeyOrdinal { get; private set; }
        public string Comment { get; private set; }
        public ViewColumn(
            Database Database,
            View View,
            int ColumnId,
            string Name,
            string TypeName,
            int Length,
            int Precision,
            int Scale,
            bool IsNullable,
            string DefaultValue,
            string PrimaryKeyName,
            int? PrimaryKeyOrdinal,
            string Comment
        )
        {
            this.Database = Database;
            this.View = View;
            this.ColumnId = ColumnId;
            this.Name = Name;
            this.TypeName = TypeName;
            this.Length = Length;
            this.Precision = Precision;
            this.Scale = Scale;
            this.IsNullable = IsNullable;
            this.DefaultValue = DefaultValue;
            this.PrimaryKeyName = PrimaryKeyName;
            this.PrimaryKeyOrdinal = PrimaryKeyOrdinal;
            this.Comment = Comment;
        }
    }
}
