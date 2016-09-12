using System;
using System.Collections.Generic;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class Database
    {
        public int DatabaseId { get; private set; }
        public string Name { get; private set; }
        public Lazy<List<Table>> Tables { get; private set; }
        public Lazy<List<View>> Views { get; private set; }
        private Database()
        {
            Tables = new Lazy<List<Table>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadTables(this));
            Views = new Lazy<List<View>>(() => DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadViews(this));
        }
    }
}
