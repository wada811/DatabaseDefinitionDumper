using DatabaseDefinitionDumper.Core;
using DatabaseDefinitionDumper.Core.Data;
using DatabaseDefinitionDumper.Core.Domain.Model;
using DatabaseDefinitionDumper.WPF.Properties;
using Livet.Messaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseDefinitionDumper.WPF.ViewModel
{
    public class MainViewModel : Livet.ViewModel
    {
        public ReadOnlyReactiveCollection<ConnectionSettingsViewModel> ConnectionSettingsViewModels { get; private set; }
        public ReactiveProperty<int> SelectedConnectionSettingsIndex { get; private set; }
        public ReactiveCommand ShowConenctionSettingsCommand { get; private set; }
        public ReactiveProperty<string> OutputFilePath { get; private set; }
        public ReactiveCommand DumpCommand { get; private set; }
        public ReactiveCommand DebugCommand { get; private set; }

        public MainViewModel()
        {
            ConnectionSettingsViewModels = DatabaseDefinitionDumperContext.Current
                .ConnectionSettingsList
                .ToReadOnlyReactiveCollection(x => new ConnectionSettingsViewModel(x))
                .AddTo(CompositeDisposable);

            SelectedConnectionSettingsIndex = DatabaseDefinitionDumperContext.Current
                .ObserveProperty(x => x.CurrentConnectionSettings)
                .Select(x => DatabaseDefinitionDumperContext.Current.ConnectionSettingsList.IndexOf(x))
                .ToReactiveProperty();

            ShowConenctionSettingsCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            ShowConenctionSettingsCommand.Subscribe(_ =>
            {
                Console.WriteLine("ShowConenctionSettingsCommand");
                Console.WriteLine($"SelectedConnectionSettingsIndex.Value: {SelectedConnectionSettingsIndex.Value}");
                var selectedConnectionSettingsViewModel = ConnectionSettingsViewModels[SelectedConnectionSettingsIndex.Value];
                if (SelectedConnectionSettingsIndex.Value == 0)
                {
                    selectedConnectionSettingsViewModel = new ConnectionSettingsViewModel(new ConnectionSettings());
                }
                Messenger.Raise(new TransitionMessage(selectedConnectionSettingsViewModel, "ShowConenctionSettings"));
            }).AddTo(CompositeDisposable);

            OutputFilePath = new ReactiveProperty<string>(@"C:\Users\rs1333\Downloads\dump.sql").AddTo(CompositeDisposable);

            DumpCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            DumpCommand.Subscribe(_ => {
                DatabaseDefinitionDumperContext.Current.DatabaseRepository = new DatabaseRepository(new SQLServerDatabaseDataSource(DatabaseDefinitionDumperContext.Current.CurrentConnectionSettings));
                DatabaseDefinitionDumperContext.Current.LoadDatabases();
            }).AddTo(CompositeDisposable);

            DebugCommand = new ReactiveCommand().AddTo(CompositeDisposable);
            DebugCommand.Subscribe(_ =>
            {
                Settings.Default.Reset();
                foreach (var connectionSettingsViewModel in ConnectionSettingsViewModels)
                {
                    Console.WriteLine("ConnectionSettingsList: " + connectionSettingsViewModel.ConnectionSettings.ToConnectionString());
                }
                MessageBox.Show(DatabaseDefinitionDumperContext.Current.CurrentConnectionSettings?.ToConnectionString());
            }).AddTo(CompositeDisposable);
        }
    }
}
