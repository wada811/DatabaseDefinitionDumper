using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class View
    {
        public Database Database { get; private set; }
        public int ViewId { get; private set; }
        public string Name { get; private set; }
        public string Definition { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public Lazy<List<ViewTrigger>> Triggers { get; private set; }
        public Lazy<List<ViewColumn>> Columns { get; private set; }
        public Lazy<List<ViewIndex>> Indexes { get; private set; }

        public View(Database Database, int ViewId, string Name, string Definition, string Comment, DateTime CreatedAt, DateTime UpdatedAt)
        {
            this.Database = Database;
            this.ViewId = ViewId;
            this.Name = Name;
            this.Definition = Definition;
            this.Comment = Comment;
            this.CreatedAt = CreatedAt;
            this.UpdatedAt = UpdatedAt;
            this.Triggers = new Lazy<List<ViewTrigger>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadTriggers(Database, this));
            this.Columns = new Lazy<List<ViewColumn>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadColumns(Database, this));
            this.Indexes = new Lazy<List<ViewIndex>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadIndexes(Database, this));
        }
    }
}
