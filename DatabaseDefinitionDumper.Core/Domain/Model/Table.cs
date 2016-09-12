using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class Table
    {
        public Database Database { get; private set; }
        public int TableId { get; private set; }
        public string Name { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Lazy<List<TableTrigger>> Triggers { get; private set; }
        public Lazy<List<TableColumn>> Columns { get; private set; }
        public Lazy<List<TableIndex>> Indexes { get; private set; }
        public Table(Database Database, int TableId, string Name, string Comment, DateTime CreatedAt, DateTime UpdatedAt)
        {
            this.Database = Database;
            this.TableId = TableId;
            this.Name = Name;
            this.Comment = Comment;
            this.CreatedAt = CreatedAt;
            this.UpdatedAt = UpdatedAt;
            this.Triggers = new Lazy<List<TableTrigger>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadTriggers(Database, this));
            this.Columns = new Lazy<List<TableColumn>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadColumns(Database, this));
            this.Indexes = new Lazy<List<TableIndex>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadIndexes(Database, this));
        }
    }
}
