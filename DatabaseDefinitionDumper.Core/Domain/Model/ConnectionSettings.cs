using Newtonsoft.Json;
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
        [JsonProperty]
        public string ServerName
        {
            get { return this._ServerName; }
            private set { this.SetProperty(ref this._ServerName, value); }
        }
        private string _UserName;
        [JsonProperty]
        public string UserName
        {
            get { return this._UserName; }
            private set { this.SetProperty(ref this._UserName, value); }
        }
        private string _Password;
        [JsonProperty]
        public string Password
        {
            get { return this._Password; }
            private set { this.SetProperty(ref this._Password, value); }
        }
        public ConnectionSettings()
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
            DatabaseDefinitionDumperContext.Current.Save(this);
        }
        public string ToConnectionString()
        {
            return $"Data Source={ServerName};User ID={UserName};Password={Password}";
        }
    }
}
