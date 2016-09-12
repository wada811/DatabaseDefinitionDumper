using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class ViewIndex
    {
        public Database Database { get; private set; }
        public View View { get; private set; }
        public int IndexId { get; private set; }
        public string Name { get; private set; }
        public int IndexType { get; private set; }
        public bool IsClustered { get; private set; }
        public int ColumnOrdinal { get; private set; }
        public int ColumnId { get; private set; }
        public string ColumnName { get; private set; }
        public string Comment { get; private set; }

        public ViewIndex(
            Database database,
            View View,
            int IndexId,
            string Name,
            int IndexType,
            bool IsClustered,
            int ColumnOrdinal,
            int ColumnId,
            string ColumnName,
            string Comment
        )
        {
            this.Database = Database;
            this.View = View;
            this.IndexId = IndexId;
            this.Name = Name;
            this.IndexType = IndexType;
            this.IsClustered = IsClustered;
            this.ColumnOrdinal = ColumnOrdinal;
            this.ColumnId = ColumnId;
            this.ColumnName = ColumnName;
            this.Comment = Comment;
        }
    }
}
