using DatabaseDefinitionDumper.Core.Domain.Model;
using DatabaseDefinitionDumper.Core.Repository;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core
{
    public class DatabaseDefinitionDumperContext : BindableModel
    {
        public static DatabaseDefinitionDumperContext Current = new DatabaseDefinitionDumperContext();
        private DatabaseDefinitionDumperContext() { }

        public IPlatformSettingRepository SettingRepository;
        public IDatabaseRepository DatabaseRepository;

        private const string ConnectionSettingsListKey = "ConnectionSettingsList";
        public ObservableCollection<ConnectionSettings> ConnectionSettingsList { get; private set; } = new ObservableCollection<ConnectionSettings>();
        private ConnectionSettings _CurrentConnectionSettings = new ConnectionSettings();
        public ConnectionSettings CurrentConnectionSettings
        {
            get { return this._CurrentConnectionSettings; }
            private set { this.SetProperty(ref this._CurrentConnectionSettings, value); }
        }

        public void Load()
        {
            Console.WriteLine("DatabaseDefinitionDumperContext.Load()");
            SettingRepository.Load()
                .Subscribe(_ => {
                    Console.WriteLine("SettingRepository[ConnectionSettingsListKey]: " + SettingRepository[ConnectionSettingsListKey]);
                    var connectionSettingsList = JsonConvert.DeserializeObject<IList<ConnectionSettings>>(SettingRepository[ConnectionSettingsListKey]);
                    Console.WriteLine("ConnectionSettings.Count: " + connectionSettingsList.Count);
                    foreach (var connectionSetttings in connectionSettingsList)
                    {
                        Console.WriteLine("ConnectionSettingsList: " + connectionSetttings.ToConnectionString());
                        ConnectionSettingsList.Add(connectionSetttings);
                    }
                    if (ConnectionSettingsList.Any())
                    {
                        CurrentConnectionSettings = ConnectionSettingsList[0];
                    }
                });
        }

        public void Save(ConnectionSettings ConnectionSettings)
        {
            if (CurrentConnectionSettings.ServerName == "")
            {
                ConnectionSettingsList.Add(ConnectionSettings);
            }
            CurrentConnectionSettings = ConnectionSettings;
            SettingRepository[ConnectionSettingsListKey] = JsonConvert.SerializeObject(ConnectionSettingsList);
            SettingRepository.Save().Subscribe(
                _ => Console.WriteLine("SettingRepository.Save().onNext()"),
                e => Console.WriteLine("SettingRepository.Save().onError(): " + e.ToString()),
                () => Console.WriteLine("SettingRepository.Save().onCompleted()")
            );
        }
    }
}
