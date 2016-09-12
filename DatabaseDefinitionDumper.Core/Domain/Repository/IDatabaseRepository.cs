using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public interface IDatabaseRepository
    {
        List<Database> LoadDatabases();
        List<Table> LoadTables(Database Database);
        List<TableTrigger> LoadTriggers(Database Database, Table Table);
        List<TableColumn> LoadColumns(Database Database, Table Table);
        List<TableIndex> LoadIndexes(Database Database, Table Table);
        List<View> LoadViews(Database Database);
        List<ViewTrigger> LoadTriggers(Database Database, View View);
        List<ViewColumn> LoadColumns(Database Database, View View);
        List<ViewIndex> LoadIndexes(Database Database, View View);
    }
}
