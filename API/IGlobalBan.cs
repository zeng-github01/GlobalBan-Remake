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
         BanPlayerData BanPlayer(CSteamID cSteamID, int duration, string reason);

        EExecuteQuery UnBanPlayer(CSteamID cSteam);

        BanPlayerData GetBanPlayerData(CSteamID cSteamID,EQueryType searchMode);

    }
}
