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
                    var exere = GlobalBan.database.BanPlayer(cSteamID);
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
                        var exere = GlobalBan.database.BanPlayer(banplayer.CSteamID);
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
                   
                    var exere = GlobalBan.database.BanPlayer(cSteamID,int.Parse(args[1]));
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
                        var exere = GlobalBan.database.BanPlayer(banplayer.CSteamID, int.Parse(args[1]));
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
            else if(args.Length == 3)
            {
                if (Extensions.isCSteamID(args[0],out CSteamID cSteamID))
                {
                    var exere= GlobalBan.database.BanPlayer(cSteamID,int.Parse(args[1]), args[2]);
                    if(exere == API.Enum.EExecuteQuery.Sucessed)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_banned"));
                    }
                    else if(exere == API.Enum.EExecuteQuery.Failure)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("error_occured"), UnityEngine.Color.red);
                    }
                }
                else
                {
                    var banplayer = UnturnedPlayer.FromName(args[0]);
                    if (banplayer != null)
                    {
                        var exere = GlobalBan.database.BanPlayer(banplayer.CSteamID, int.Parse(args[1]), args[2]);
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
            else
            {

            }
        }
    }
}
