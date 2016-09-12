using DatabaseDefinitionDumper.Core;
using DatabaseDefinitionDumper.Core.Data;
using DatabaseDefinitionDumper.Core.Domain.Model;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.WPF.ViewModel
{
    public class MainViewModel
    {
        public static ConnectionSettings ConnectionSettings = new ConnectionSettings();
        public ReactiveProperty<string> ServerName { get; private set; } = ConnectionSettings.ObserveProperty(x => x.ServerName).ToReactiveProperty(ConnectionSettings.ServerName);
        public ReactiveProperty<string> UserName { get; private set; } = ConnectionSettings.ObserveProperty(x => x.UserName).ToReactiveProperty(ConnectionSettings.UserName);
        public ReactiveProperty<string> Password { get; private set; } = ConnectionSettings.ObserveProperty(x => x.Password).ToReactiveProperty(ConnectionSettings.Password);
        public ReactiveProperty<string> OutputFilePath { get; private set; } = new ReactiveProperty<string>(@"C:\Users\rs1333\Downloads\dump.sql");
        public ReactiveCommand DumpCommand { get; private set; } = new ReactiveCommand();

        public MainViewModel()
        {
            DumpCommand.Subscribe(_ => {
                ConnectionSettings.Save(ServerName.Value, UserName.Value, Password.Value);
                DatabaseDefinitionDumperContext.Current.DatabaseRepository = new DatabaseRepository(new SQLServerDatabaseDataSource(ConnectionSettings));
                using (var writer = new StreamWriter(OutputFilePath.Value))
                {
                    var databases = DatabaseDefinitionDumperContext.Current.DatabaseRepository.LoadDatabases();
                    databases.ForEach(database =>
                    {
                        writer.WriteLine($"database: {database.Name}");
                    });
                }
            });
        }
    }
}
