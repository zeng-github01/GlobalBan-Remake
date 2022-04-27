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

namespace GlobalBan
{
    public class GlobalBan :RocketPlugin<Configuration>
    {
        public static GlobalBan Instance;
        public Dictionary<UnturnedPlayer, string> BannedReason = new Dictionary<UnturnedPlayer, string>();
        public static Database.DatabaseManager database;
        protected override void Load()
        {
            Instance = this;
            database = new Database.DatabaseManager();
            U.Events.OnBeforePlayerConnected += Events_OnBeforePlayerConnected;
            //U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            Logger.Log($"{Name} has been loaded");
        }


        private void Events_OnBeforePlayerConnected(UnturnedPlayer player)
        {
            try
            {
                if (CheckIfBanned(player))
                {
                    if (BannedReason.TryGetValue(player, out string reason))
                    {
                        Provider.kick(player.CSteamID, reason);
                        BannedReason.Remove(player);

                    }
                    else
                    {
                        Provider.kick(player.CSteamID, GlobalBan.Instance.Configuration.Instance.DefaultBanMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        protected override void Unload()
        {
            U.Events.OnBeforePlayerConnected -= Events_OnBeforePlayerConnected;
            //U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            BannedReason.Clear();
            Logger.Log($"{Name} has been unloaded");
        }
        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            try
            {
                if (CheckIfBanned(player))
                {
                    if (BannedReason.TryGetValue(player, out string reason))
                    {
                        Provider.kick(player.CSteamID, reason);
                        BannedReason.Remove(player);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static bool CheckIfBanned(UnturnedPlayer player)
        {
            bool banned = false;
            foreach (EQueryType mode in Enum.GetValues(typeof(EQueryType)))
            {
                if(!GlobalBan.Instance.Configuration.Instance.BanIPandHWID && mode == EQueryType.SearchByHWIDAndIP)
                {
                    continue;
                }
                else if(GlobalBan.Instance.Configuration.Instance.BanIPandHWID && (mode == EQueryType.SearchByIP || mode == EQueryType.SearchByHWID))
                {
                    continue;
                }

                var bandata = database.GetBanPlayerData(player.CSteamID, mode);
                if(bandata != null)
                {
                    if (bandata.IsUnbanned || (bandata.Duration > 0 && bandata.BanOfTime.AddSeconds(bandata.Duration) < DateTime.Now))
                    {
                        banned = false;
                        break;
                    }
                    else if(!bandata.IsUnbanned ||(bandata.Duration > 0 && bandata.BanOfTime.AddSeconds(bandata.Duration) > DateTime.Now))
                    {
                        if (!GlobalBan.Instance.BannedReason.ContainsKey(player))
                        {
                            if (bandata.Reason != string.Empty)
                            {
                                if (Instance.BannedReason.ContainsKey(player))
                                {
                                    Instance.BannedReason.Remove(player);
                                    Instance.BannedReason.Add(player, bandata.Reason);
                                }
                                else
                                {
                                    Instance.BannedReason.Add(player, bandata.Reason);
                                }
                            }
                            if (!Instance.BannedReason.ContainsKey(player))
                            {
                                Instance.BannedReason.Add(player, Instance.Configuration.Instance.DefaultBanMessage);
                            }
                            else
                            {
                                Instance.BannedReason.Remove(player);
                                Instance.BannedReason.Add(player, bandata.Reason);
                            }

                        }
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
