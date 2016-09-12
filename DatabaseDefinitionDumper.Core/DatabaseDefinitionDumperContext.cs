using DatabaseDefinitionDumper.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core
{
    public class DatabaseDefinitionDumperContext
    {
        public static DatabaseDefinitionDumperContext Current = new DatabaseDefinitionDumperContext();
        private DatabaseDefinitionDumperContext() { }

        public IDatabaseRepository DatabaseRepository;
    }
}
