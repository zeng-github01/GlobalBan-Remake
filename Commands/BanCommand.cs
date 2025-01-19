using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using Steamworks;
using Rocket.Unturned.Chat;
using PlayerInfoLibrary;
using Rocket.Core.Logging;

namespace GlobalBan.Commands
{
    public class BanCommand : IRocketCommand
    {
        public string Name => "Ban";

        public string Help => "ban a player from server";

        public string Syntax => "ban <player> [duration] [reason]";

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public List<string> Permissions => new List<string>() {"globalban.ban" };

        public List<string> Aliases => new List<string>() { };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length == 1)
            {
                if (Extensions.isCSteamID(args[0],out CSteamID cSteamID))
                {
                    var banPlayer = GlobalBan.database.BanPlayer(cSteamID);
                    if (banPlayer != null)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                        Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                    }
                    else
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                    }
                }
                else
                {
                    var banplayer = UnturnedPlayer.FromName(args[0]);
                    if (banplayer != null)
                    {
                        var banPlayer = GlobalBan.database.BanPlayer(banplayer.CSteamID);
                        if (banPlayer != null)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                        }
                        else
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                        }
                    }
                }
            }
            else if(args.Length == 2)
            {
                if (Extensions.isCSteamID(args[0], out CSteamID cSteamID ))
                {
                    if (int.TryParse(args[1], out int duration))
                    {
                        var banPlayer = GlobalBan.database.BanPlayer(cSteamID,duration);
                        if (banPlayer != null)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                        }
                        else
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                        }
                    }
                    else
                    {
                        var banPlayer = GlobalBan.database.BanPlayer(cSteamID,0,args[1]);
                        if (banPlayer != null)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                        }
                        else
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                        }
                    }
                }
                else
                {
                    var banplayer = UnturnedPlayer.FromName(args[0]);
                    if (banplayer != null)
                    {
                        if (int.TryParse(args[1], out int duration))
                        {
                            var banPlayer = GlobalBan.database.BanPlayer(banplayer.CSteamID, duration);
                            if (banPlayer != null)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                                Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                            }
                        }
                        else
                        {
                            var banPlayer = GlobalBan.database.BanPlayer(banplayer.CSteamID, 0);
                            if (banPlayer != null)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                                Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                            }
                        }
                    }
                }
            }
            else if(args.Length == 3)
            {
                if (Extensions.isCSteamID(args[0],out CSteamID cSteamID))
                {
                    if (int.TryParse(args[1], out int duration))
                    {
                        var banPlayer = GlobalBan.database.BanPlayer(cSteamID, duration, args[2]);
                        if (banPlayer != null)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                        }
                        else
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                        }
                    }
                }
                else
                {
                    var banplayer = UnturnedPlayer.FromName(args[0]);
                    if (banplayer != null)
                    {
                        if (int.TryParse(args[1], out int duration))
                        {
                            var banPlayer = GlobalBan.database.BanPlayer(banplayer.CSteamID, duration, args[2]);
                            if (banPlayer != null)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                                Logger.Log($"({cSteamID.m_SteamID}) was banned by Admin {caller.DisplayName} ({caller.Id})");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                            }
                        }
                    }
                }
            }
        }
    }
}
