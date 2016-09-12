using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseDefinitionDumper.Core.Data;
using DatabaseDefinitionDumper.Core.Domain;
using DatabaseDefinitionDumper.Core;
using DatabaseDefinitionDumper.Core.Domain.Model;
using SQLServerDump.Extensions;

namespace DatabaseDefinitionDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("ServerName: ");
            var ServerName = Console.ReadLine();
            Console.Write("UserName: ");
            var UserName = Console.ReadLine();
            Console.Write("Password: ");
            var Password = ConsoleExtensions.ReadPassword();
            var ConnectionSettings = new ConnectionSettings(ServerName, UserName, Password);
            DatabaseDefinitionDumperContext.Current.DatabaseRepository = new DatabaseRepository(new SQLServerDatabaseDataSource(ConnectionSettings));
            var databases = DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadDatabases();
            databases.ForEach(database =>
            {
                Console.WriteLine($"database: {database.Name}");
                var tables = database.Tables.Value;
                tables.ForEach(table =>
                {
                    Console.WriteLine($"    table: {table.Name}");
                    var triggers = table.Triggers.Value;
                    triggers.ForEach(trigger =>
                    {
                        Console.WriteLine($"        tigger: {trigger.Name}, {trigger.TypeName}");
                    });
                    var columns = table.Columns.Value;
                    columns.ForEach(column =>
                    {
                        Console.WriteLine($"        column: {column.Name}");
                    });
                    var indexes = table.Indexes.Value;
                    indexes.ForEach(index =>
                    {
                        Console.WriteLine($"        index: {index.Name}");
                    });
                });
                var views = database.Views.Value;
                views.ForEach(view =>
                {
                    Console.WriteLine($"    view: {view.Name}");
                    var triggers = view.Triggers.Value;
                    triggers.ForEach(trigger =>
                    {
                        Console.WriteLine($"    view: {view.Name}");
                        Console.WriteLine($"        tigger: {trigger.Name}, {trigger.TypeName}");
                    });
                    var columns = view.Columns.Value;
                    columns.ForEach(column =>
                    {
                        Console.WriteLine($"        column: {column.Name}");
                    });
                    var indexes = view.Indexes.Value;
                    indexes.ForEach(index =>
                    {
                        Console.WriteLine($"    view: {view.Name}");
                        Console.WriteLine($"        index: {index.Name}");
                    });
                });
            });
        }
    }
}
