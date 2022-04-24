using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBan.API.Enum;
using Rocket.Unturned.Player;
using Steamworks;

namespace GlobalBan.API
{
    public interface IGlobalBan
    {
         ExecuteResult BanPlayer(CSteamID cSteamID, int duration, string reason);

        ExecuteResult UnBanPlayer(CSteamID cSteam);

        BanPlayerData GetBanPlayerData(CSteamID cSteamID,BanSearchMode searchMode);

    }
}
