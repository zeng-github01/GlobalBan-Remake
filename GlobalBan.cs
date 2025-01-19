using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Rocket.Unturned.Events;
using Rocket.Unturned;
using Rocket.Core.Logging;
using Rocket.API.Collections;
using PlayerInfoLibrary;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using GlobalBan.API.Enum;
using GlobalBan.API;
using Rocket.Unturned.Permissions;
using Steamworks;

namespace GlobalBan
{
    public class GlobalBan :RocketPlugin<Configuration>
    {
        public static GlobalBan Instance;
        //public Dictionary<UnturnedPlayer, string> BannedReason = new Dictionary<UnturnedPlayer, string>();
        public static Database.DatabaseManager database;
        protected override void Load()
        {
            Instance = this;
            database = new Database.DatabaseManager();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            Logger.Log($"{Name} has been loaded");
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if (CheckIfBanned(player.CSteamID, out BanPlayerData banPlayerData))
            {
                if (!string.IsNullOrEmpty(banPlayerData.Reason))
                {
                    Provider.kick(player.CSteamID, banPlayerData.Reason);
                }
                else
                {
                    Provider.kick(player.CSteamID, Configuration.Instance.DefaultBanMessage);
                }
            }
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            Logger.Log($"{Name} has been unloaded");
        }

        public static bool CheckIfBanned(CSteamID cSteamID,out BanPlayerData banPlayerData)
        {
            bool banned = false;
            banPlayerData = null;
            foreach (EQueryType mode in Enum.GetValues(typeof(EQueryType)))
            {
                banPlayerData = database.GetBanPlayerData(cSteamID, mode);
                if(banPlayerData != null)
                {
                    if (banPlayerData.IsUnbanned || (banPlayerData.Duration > 0 && banPlayerData.BanOfTime.AddSeconds(banPlayerData.Duration) < DateTime.Now))
                    {
                        break;
                    }
                    else if(!banPlayerData.IsUnbanned ||(banPlayerData.Duration > 0 && banPlayerData.BanOfTime.AddSeconds(banPlayerData.Duration) > DateTime.Now))
                    {
                        banned = true;
                        break;
                    }
                }
            }
            return banned;
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"player_banned","Player have been banned" },
            {"player_unbanned","Player have been unbanned" },
            {"error_occured","" }
        };
    }
}
