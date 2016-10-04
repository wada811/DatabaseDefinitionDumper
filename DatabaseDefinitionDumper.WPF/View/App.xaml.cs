using DatabaseDefinitionDumper.Core;
using DatabaseDefinitionDumper.WPF.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseDefinitionDumper.WPF
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DatabaseDefinitionDumperContext.Current.SettingRepository = new WpfSettingRepository();
            DatabaseDefinitionDumperContext.Current.Load();
        }
    }
}
