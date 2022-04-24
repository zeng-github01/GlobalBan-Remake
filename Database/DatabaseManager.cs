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
    public class DatabaseManager :IGlobalBan
    {
        private DatabaseConnection databaseConnection = new DatabaseConnection();
        public DatabaseManager()
        {
            CheckSchema();
        }

        public void CheckSchema()
        {
            databaseConnection.ExecuteQuery(true, $"CREATE TABLE IF NOT EXISTS `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` (`SteamID` BIGINT NOT NULL, `HWID` varchar(128) NOT NULL,`IP` INT NOT NULL, `BanOfTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP, `Reason` varchar(512), `Duration` INT NOT NULL,`IsUnbanned` BOOLEAN NOT NULL ,`ServerID` smallint(5) NOT NULL)");
        }
        public ExecuteResult BanPlayer(CSteamID cSteamID,int duration = 0,string reason = null)
        {         
            var playerdata = PlayerInfoLib.Database.QueryById(cSteamID,false);
            try
            {
                databaseConnection.ExecuteQuery(true, $"INSERT IGNORE INTO `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` (SteamID,HWID,IP,BanOfTime,Duration,Reason,IsUnBanned,ServerID) values('{cSteamID}','{playerdata.HWID}','{Parser.getUInt32FromIP(playerdata.IP)}','{DateTime.Now}','{duration}','{reason}',false,'{playerdata.LastServerID}')");
                if(reason == null)
                {
                    Provider.kick(cSteamID,GlobalBan.Instance.Configuration.Instance.DefaultBanMessage);
                    return ExecuteResult.Sucessed;
                }
                else
                {
                    Provider.kick(cSteamID,reason);
                }
                return ExecuteResult.Sucessed;
            }
            catch(Exception ex)
            {
                
            }
            
            return ExecuteResult.Failure;

        }

        public ExecuteResult UnBanPlayer(CSteamID cSteamID)
        {
            try
            {
                databaseConnection.ExecuteQuery(true, $"UPDATE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}`SET `IsUnbanned` = '1' WHERE `SteamID`= '{cSteamID}';");
                return ExecuteResult.Sucessed;
            }
            catch (Exception ex)
            {

            }
            return ExecuteResult.Failure;
        }
        private BanPlayerData BuildBanPlayerData(MySqlDataReader reader)
        {
            return new BanPlayerData((CSteamID)reader.GetUInt64("SteamID"), reader.GetString("HWID"),Parser.getIPFromUInt32(reader.GetUInt32("IP")), reader.GetInt32("Duration"), reader.GetUInt16("ServerID"), reader.GetDateTime("BanOfTime"), reader.GetBoolean("IsUnbanned"),reader.GetString("Reason"));
        }

        public BanPlayerData GetBanPlayerData(CSteamID cSteamID,BanSearchMode searchMode)
        {
            BanPlayerData playerData = null;
            try
            {
                MySqlConnection connection =databaseConnection.CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                switch(searchMode)
                {
                    case BanSearchMode.SearchByID:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `SteamID` = '{cSteamID}'";
                        break;
                    case BanSearchMode.SearchByIP:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `IP` = '{Parser.getIPFromUInt32(Convert.ToUInt32(PlayerInfoLib.Database.QueryById(cSteamID).IP))}'";
                        break;
                    case BanSearchMode.SearchByHWID:
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `HWID` = '{PlayerInfoLib.Database.QueryById(cSteamID).HWID}'";
                        break;
                    case BanSearchMode.SearchByHWIDAndIP:  
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` where `IP` = '{Parser.getIPFromUInt32(Convert.ToUInt32(PlayerInfoLib.Database.QueryById(cSteamID).IP))}' AND `HWID` = '{PlayerInfoLib.Database.QueryById(cSteamID).HWID}'";
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
    }
}
