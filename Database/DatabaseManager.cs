using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using GlobalBan.API;
using GlobalBan.API.Enum;
using Steamworks;
using SDG.Unturned;
using Rocket.Core.Logging;
using PlayerInfoLibrary;
using Newtonsoft.Json;

namespace GlobalBan.Database
{
    public class DatabaseManager : IGlobalBan
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
                databaseConnection.ExecuteQuery(true,
                    $"ALTER TABLE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` ADD primary key (`SteamID`);");
                GlobalBan.Instance.Configuration.Instance.TableVersion = 2;
            }

            if (GlobalBan.Instance.Configuration.Instance.TableVersion == 2)
            {
                databaseConnection.ExecuteQuery(false, $"ALTER TABLE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` Modify `HWID` JSON NOT NULL");
                GlobalBan.Instance.Configuration.Instance.TableVersion = 3;
            }

            if(GlobalBan.Instance.Configuration.Instance.TableVersion == 3)
            {
                databaseConnection.ExecuteQuery(false, $"ALTER TABLE `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` DROP COLUMN `Admin`");
                GlobalBan.Instance.Configuration.Instance.TableVersion++;
            }

            GlobalBan.Instance.Configuration.Save();
        }
        public BanPlayerData BanPlayer(CSteamID cSteamID,int duration = 0,string reason = null)
        {
            try
            {
                var playerdata = PlayerInfoLib.Database.QueryById(cSteamID, false);
                var banPlayerData = new BanPlayerData(cSteamID, playerdata.HWID, playerdata.IP,duration, playerdata.LastServerID, DateTime.Now, false, reason);
                SaveToDB(banPlayerData);
                if (string.IsNullOrEmpty(reason))
                {
                    Provider.kick(cSteamID,GlobalBan.Instance.Configuration.Instance.DefaultBanMessage);
                }
                else
                {
                    Provider.kick(cSteamID,reason);
                }
                return banPlayerData;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;

        }

        public EExecuteQuery UnBanPlayer(CSteamID cSteamID)
        {
            var banplayerdata = GetBanPlayerData(cSteamID, EQueryType.SearchByID);
            var unbanplayerdata = new BanPlayerData(cSteamID, banplayerdata.HWID, banplayerdata.IP, banplayerdata.Duration, banplayerdata.ServerID, banplayerdata.BanOfTime, true, banplayerdata.Reason);
            try
            {
                SaveToDB(unbanplayerdata);
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
            return new BanPlayerData((CSteamID)reader.GetUInt64("SteamID"), JsonConvert.DeserializeObject<List<string>>(reader.GetString("HWID")),Parser.getIPFromUInt32(reader.GetUInt32("IP")), reader.GetInt32("Duration"), reader.GetUInt16("ServerID"), reader.GetDateTime("BanOfTime"), reader.GetBoolean("IsUnbanned"),reader.GetString("Reason"));
        }

        public BanPlayerData GetBanPlayerData(CSteamID cSteamID,EQueryType searchMode)
        {
            BanPlayerData playerData = null;
            MySqlConnection connection = databaseConnection.CreateConnection();
            try
            {

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
                        command.CommandText = $"SELECT * from `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` WHERE JSON_OVERLAPS(`HWID`, '{JsonConvert.SerializeObject(PlayerInfoLib.Database.QueryById(cSteamID).HWID)}')";
                        break;
                }
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    playerData = BuildBanPlayerData(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                connection.Close();
            }
            return playerData;
        }

        internal void SaveToDB(BanPlayerData banPlayerData)
        {
            databaseConnection.ExecuteQuery(true, 
            $"INSERT INTO `{GlobalBan.Instance.Configuration.Instance.DatabaseTableName}` (SteamID,HWID,IP,BanOfTime,Duration,Reason,IsUnbanned,ServerID) values('{banPlayerData.CSteamID}','{JsonConvert.SerializeObject(banPlayerData.HWID)}','{Parser.getUInt32FromIP(banPlayerData.IP)}','{banPlayerData.BanOfTime}','{banPlayerData.Duration}','{banPlayerData.Reason}',{banPlayerData.IsUnbanned},'{banPlayerData.ServerID}') ON DUPLICATE KEY UPDATE `SteamID` = VALUES(`SteamID`), `HWID` = VALUES(`HWID`), `IP` = VALUES(`IP`), `BanOfTime` = VALUES(`BanOfTime`), `Duration` = VALUES(`Duration`), `Reason` = VALUES(`Reason`), `IsUnbanned` = VALUES(`IsUnbanned`),  `ServerID` = VALUES(`ServerID`)");
        }
    }
}
