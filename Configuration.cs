using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace GlobalBan
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int TableVersion;
        public string DatabaseAddress;
        public string DatabaseUserName;
        public string DatabasePassword;
        public string DatabaseName;
        public int DatabasePort;
        public string DatabaseTableName;
        public string DefaultBanMessage;
        public void LoadDefaults()
        {
            TableVersion = 2;
            DatabaseAddress = "127.0.0.1";
            DatabaseUserName = "root";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabasePort = 3306;
            DatabaseTableName = "GlobalBan";
            DefaultBanMessage = "You have been banned from this server";
        }
    }
}
