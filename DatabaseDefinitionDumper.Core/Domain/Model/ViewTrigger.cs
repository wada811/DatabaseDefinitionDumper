using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain
{
    public class ViewTrigger
    {
        public Database Database { get; private set; }
        public View View { get; private set; }
        public int TriggerId { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public string TypeName { get; private set; }
        public ViewTrigger(Database Database, View View, int TriggerId, string Name, int Type, string TypeName)
        {
            this.Database = Database;
            this.View = View;
            this.TriggerId = TriggerId;
            this.Name = Name;
            this.Type = Type;
            this.TypeName = TypeName;
        }
    }
}
