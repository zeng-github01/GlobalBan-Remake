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
                    var exere = GlobalBan.database.BanPlayer(caller.Id,cSteamID);
                    if (exere == API.Enum.EExecuteQuery.Sucessed)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                    }
                    else if (exere == API.Enum.EExecuteQuery.Failure)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                    }
                }
                else
                {
                    var banplayer = UnturnedPlayer.FromName(args[0]);
                    if (banplayer != null)
                    {
                        var exere = GlobalBan.database.BanPlayer(caller.Id,banplayer.CSteamID);
                        if (exere == API.Enum.EExecuteQuery.Sucessed)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                        }
                        else if (exere == API.Enum.EExecuteQuery.Failure)
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
                    if (uint.TryParse(args[1], out uint duration))
                    {
                        var exere = GlobalBan.database.BanPlayer(caller.Id,cSteamID,duration);
                        if (exere == API.Enum.EExecuteQuery.Sucessed)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                        }
                        else if (exere == API.Enum.EExecuteQuery.Failure)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                        }
                    }
                    else
                    {
                        var exere = GlobalBan.database.BanPlayer(caller.Id,cSteamID,0,args[1]);
                        if (exere == API.Enum.EExecuteQuery.Sucessed)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                        }
                        else if (exere == API.Enum.EExecuteQuery.Failure)
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
                        if (uint.TryParse(args[1], out uint duration))
                        {
                            var exere = GlobalBan.database.BanPlayer(caller.Id,banplayer.CSteamID, duration);
                            if (exere == API.Enum.EExecuteQuery.Sucessed)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            }
                            else if (exere == API.Enum.EExecuteQuery.Failure)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                            }
                        }
                        else
                        {
                            var exere = GlobalBan.database.BanPlayer(caller.Id,banplayer.CSteamID, 0,args[1]);
                            if (exere == API.Enum.EExecuteQuery.Sucessed)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            }
                            else if (exere == API.Enum.EExecuteQuery.Failure)
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
                    if (uint.TryParse(args[1], out uint duration))
                    {
                        var exere = GlobalBan.database.BanPlayer(caller.Id,cSteamID, duration, args[2]);
                        if (exere == API.Enum.EExecuteQuery.Sucessed)
                        {
                            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                        }
                        else if (exere == API.Enum.EExecuteQuery.Failure)
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
                        if (uint.TryParse(args[1], out uint duration))
                        {
                            var exere = GlobalBan.database.BanPlayer(caller.Id,banplayer.CSteamID, duration, args[2]);
                            if (exere == API.Enum.EExecuteQuery.Sucessed)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                            }
                            else if (exere == API.Enum.EExecuteQuery.Failure)
                            {
                                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                            }
                        }
                    }
                }
            }
            else
            {

            }
        }
    }
}
