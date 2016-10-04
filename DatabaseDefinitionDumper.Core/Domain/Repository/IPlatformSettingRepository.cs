using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Repository
{
    public interface IPlatformSettingRepository
    {
        string this[string key] { get; set; }
        IObservable<Unit> Save();
        IObservable<Unit> Load();
    }
}
