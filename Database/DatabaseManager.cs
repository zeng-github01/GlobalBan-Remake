using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GlobalBan.API;
using GlobalBan.API.Enum;
using Rocket.Unturned.Player;
using Steamworks;
using SDG.Unturned;
using Rocket.Core.Logging;
using PlayerInfoLibrary;

namespace GlobalBan.Database
{
    public class DatabaseManager 
    {
        private DatabaseConnection databaseConnection = new DatabaseConnection();
        public DatabaseManager()
        {
            CheckSchema();
        }

        internal void CheckSchema()
        {
            databaseConnection.ExecuteQuery(true, $"CREATE TABLE IF NOT EXISTS `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` (`SteamID` BIGINT NOT NULL, `HWID` varchar(128) NOT NULL,`IP` INT UNSIGNED NOT NULL, `BanOfTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP, `Reason` varchar(512), `Duration` INT UNSIGNED NOT NULL,`IsUnbanned` BOOLEAN NOT NULL, `Admin` VARCHAR(32) NOT NULL, `ServerID` smallint(5) NOT NULL,primary key (`SteamID`))");
            
            if(GlobalBan.Instance.Configuration.Instance.TableVersion == 1)
            {
                databaseConnection.ExecuteQuery(true,
                $"ALTER TABLE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` ADD `Admin` VARCHAR(32) NOT NULL;");
                GlobalBan.Instance.Configuration.Instance.TableVersion = 2;
                GlobalBan.Instance.Configuration.Save();
            }
        }
        internal EExecuteQuery BanPlayer(string admin,CSteamID cSteamID,uint duration = 0,string reason = null)
        {
            try
            {
                var playerdata = PlayerInfoLib.Database.QueryById(cSteamID, false);
                var banPlayerData = new BanPlayerData(cSteamID, admin, playerdata.HWID, playerdata.IP,duration, playerdata.LastServerID, DateTime.Now, false, reason);
                SaveToDB(banPlayerData);
                if (reason == null)
                {
                    Provider.kick(cSteamID,GlobalBan.Instance.Configuration.Instance.DefaultBanMessage);
                    return EExecuteQuery.Sucessed;
                }
                else
                {
                    Provider.kick(cSteamID,reason);
                }
                return EExecuteQuery.Sucessed;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex,"在存储数据时发生错误");
            }
            
            return EExecuteQuery.Failure;

        }

        internal EExecuteQuery UnBanPlayer(CSteamID cSteamID)
        {
            var banplayerdata = GetBanPlayerData(cSteamID, EQueryType.SearchByID);
            var unbanplayerdata = new BanPlayerData(cSteamID, banplayerdata.AdminID, banplayerdata.HWID, banplayerdata.IP, banplayerdata.Duration, banplayerdata.ServerID, banplayerdata.BanOfTime,true, banplayerdata.Reason);
            try
            {
                SaveToDB(unbanplayerdata);
               // databaseConnection.ExecuteQuery(true, $"UPDATE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}`SET `IsUnbanned` = '1' WHERE `SteamID`= '{cSteamID}';");
                return EExecuteQuery.Sucessed;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return EExecuteQuery.Failure;
        }
        private BanPlayerData BuildBanPlayerData(MySqlDataReader reader)
        {
            return new BanPlayerData((CSteamID)reader.GetUInt64("SteamID"),reader.GetString("Admin"), reader.GetString("HWID"),Parser.getIPFromUInt32(reader.GetUInt32("IP")), reader.GetUInt32("Duration"), reader.GetUInt16("ServerID"), reader.GetDateTime("BanOfTime"), reader.GetBoolean("IsUnbanned"),reader.GetString("Reason"));
        }

        public BanPlayerData GetBanPlayerData(CSteamID cSteamID,EQueryType searchMode)
        {
            BanPlayerData playerData = null;
            try
            {
                MySqlConnection connection =databaseConnection.CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                switch(searchMode)
                {
                    case EQueryType.SearchByID:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `SteamID` = '{cSteamID}'";
                        break;
                    case EQueryType.SearchByIP:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `IP` = '{PlayerInfoLib.Database.QueryById(cSteamID).IP}'";
                        break;
                    case EQueryType.SearchByHWID:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `HWID` = '{PlayerInfoLib.Database.QueryById(cSteamID).HWID}'";
                        break;
                    case EQueryType.SearchByHWIDAndIP:  
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `IP` = '{PlayerInfoLib.Database.QueryById(cSteamID).IP}' AND `HWID` = '{PlayerInfoLib.Database.QueryById(cSteamID).HWID}'";
                        break;
                }
                
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if(reader.Read())
                {
                    playerData = BuildBanPlayerData(reader);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return playerData;
        }

        internal void SaveToDB(BanPlayerData banPlayerData)
        {
            databaseConnection.ExecuteQuery(true, 
            $"INSERT INTO `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` (SteamID,HWID,IP,BanOfTime,Duration,Reason,IsUnbanned,Admin,ServerID) values('{banPlayerData.CSteamID}','{banPlayerData.HWID}','{Parser.getUInt32FromIP(banPlayerData.IP)}','{DateTime.Now}','{banPlayerData.Duration}','{banPlayerData.Reason}',{banPlayerData.IsUnbanned},'{banPlayerData.AdminID}','{banPlayerData.ServerID}') ON DUPLICATE KEY UPDATE `SteamID` = VALUES(`SteamID`), `HWID` = VALUES(`HWID`), `IP` = VALUES(`IP`), `BanOfTime` = VALUES(`BanOfTime`), `Duration` = VALUES(`Duration`), `Reason` = VALUES(`Reason`), `IsUnbanned` = VALUES(`IsUnbanned`), `Admin` = VALUES(`Admin`),  `ServerID` = VALUES(`ServerID`)");
        }
    }
}
