using DatabaseDefinitionDumper.Core.Repository;
using DatabaseDefinitionDumper.WPF.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.WPF.Model
{
    class WpfSettingRepository : IPlatformSettingRepository
    {
        public string this[string key]
        {
            get
            {
                return Settings.Default[key] as string;
            }
            set
            {
                Settings.Default[key] = value as string;
            }
        }

        public IObservable<Unit> Save()
        {
            return Observable.Defer(() => Observable.Start(() => Settings.Default.Save()));
        }

        public IObservable<Unit> Load()
        {
            return Observable.Defer(() => Observable.Start(() => Settings.Default.Reload()));
        }
    }
}
