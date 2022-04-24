using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;

namespace GlobalBan.Commands
{
    public class UnbanCommand : IRocketCommand
    {
        public string Name => "unban";
        public string Help => "unabn a player from server";

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Syntax => "unabn <player>";

        public List<string> Permissions => new List<string>() { "globalban.unban" };
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length == 1)
            {
                if (ulong.TryParse(args[0], out ulong steamid))
                {
                    var unbanedplayer = new Steamworks.CSteamID(steamid);
                   var unbanresult = GlobalBan.database.UnBanPlayer(unbanedplayer);
                    if(unbanresult == API.Enum.ExecuteResult.Sucessed)
                    {
                        UnturnedChat.Say(caller, GlobalBan.Instance.Translate("player_unbanned"));
                    }
                }
                else
                {
                    var unbanplayer = UnturnedPlayer.FromName(args[0]);
                    if (unbanplayer != null)
                    {
                        GlobalBan.database.UnBanPlayer(unbanplayer.CSteamID);
                    }
                }


            }
        }
    }
}
