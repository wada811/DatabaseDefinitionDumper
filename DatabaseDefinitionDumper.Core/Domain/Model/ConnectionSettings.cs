using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDefinitionDumper.Core.Domain.Model
{
    public class ConnectionSettings : BindableModel
    {
        private string _ServerName;
        public string ServerName
        {
            get { return this._ServerName; }
            private set { this.SetProperty(ref this._ServerName, value); }
        }
        private string _UserName;
        public string UserName
        {
            get { return this._UserName; }
            private set { this.SetProperty(ref this._UserName, value); }
        }
        private string _Password;
        public string Password
        {
            get { return this._Password; }
            private set { this.SetProperty(ref this._Password, value); }
        }
        public ConnectionSettings() : this("", "", "")
        {
        }
        public ConnectionSettings(string ServerName, string UserName, string Password)
        {
            this.ServerName = ServerName;
            this.UserName = UserName;
            this.Password = Password;
        }
        public void Save(string ServerName, string UserName, string Password)
        {
            this.ServerName = ServerName;
            this.UserName = UserName;
            this.Password = Password;
        }
        public string ToConnectionString()
        {
            return $"Data Source={ServerName};User ID={UserName};Password={Password}";
        }

    }
}
