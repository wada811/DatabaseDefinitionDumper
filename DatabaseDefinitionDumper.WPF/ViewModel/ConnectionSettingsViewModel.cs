using DatabaseDefinitionDumper.Core;
using DatabaseDefinitionDumper.Core.Data;
using DatabaseDefinitionDumper.Core.Domain.Model;
using Livet.Messaging;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseDefinitionDumper.WPF.ViewModel
{
    public class ConnectionSettingsViewModel : Livet.ViewModel
    {
        public ConnectionSettings ConnectionSettings { get; private set; }
        public ReactiveProperty<string> ServerName { get; private set; }
        public ReactiveProperty<string> UserName { get; private set; }
        public ReactiveProperty<string> Password { get; private set; }
        public ReactiveCommand ConnectionTestCommand { get; private set; }
        public ReactiveCommand SettingCommand { get; private set; }

        public ConnectionSettingsViewModel(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;

            ServerName = ConnectionSettings.ObserveProperty(x => x.ServerName).ToReactiveProperty(ConnectionSettings.ServerName).AddTo(CompositeDisposable);
            UserName = ConnectionSettings.ObserveProperty(x => x.UserName).ToReactiveProperty(ConnectionSettings.UserName).AddTo(CompositeDisposable);
            Password = ConnectionSettings.ObserveProperty(x => x.Password).ToReactiveProperty(ConnectionSettings.Password).AddTo(CompositeDisposable);

            ConnectionTestCommand = new ReactiveCommand();
            ConnectionTestCommand.Subscribe(_ =>
            {
                var ConnectionSettings = new ConnectionSettings(ServerName.Value, UserName.Value, Password.Value);
                DatabaseDefinitionDumperContext.Current.DatabaseRepository = new DatabaseRepository(new SQLServerDatabaseDataSource(ConnectionSettings));
                DatabaseDefinitionDumperContext.Current.DatabaseRepository.TestConnection()
                .Subscribe(
                    success =>
                    {
                        Messenger.Raise(new InformationMessage(success ? "接続に成功しました。" : "接続に失敗しました。", "接続テスト", MessageBoxImage.Information, "ConnectionTest"));
                    },
                    e =>
                    {
                        Messenger.Raise(new InformationMessage("接続に失敗しました。" + Environment.NewLine + Environment.NewLine + e.Message, "接続テスト", MessageBoxImage.Error, "ConnectionTest"));
                    }).AddTo(CompositeDisposable);
            });

            SettingCommand = new ReactiveCommand();
            SettingCommand.Subscribe(_ =>
            {
                ConnectionSettings.Save(ServerName.Value, UserName.Value, Password.Value);
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
            }).AddTo(CompositeDisposable);
        }
    }
}
