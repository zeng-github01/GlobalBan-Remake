using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Steamworks;

namespace GlobalBan.API
{
    public class BanPlayerData
    {
        public CSteamID CSteamID;
        public List<string> HWID;
        public string IP;
        public int Duration;
        public int ServerID;
        public DateTime BanOfTime;
        public bool IsUnbanned;
        public string Reason;
        internal BanPlayerData(CSteamID cSteamID,List<string> hwid,string ip,int duration,int serverid,DateTime dateTime,bool isunbaned,string reason)
        {
            CSteamID = cSteamID;
            HWID = hwid;
            IP = ip;
            Duration = duration;
            ServerID = serverid;
            BanOfTime = dateTime;
            IsUnbanned = isunbaned;
            Reason = reason;
        }

        internal BanPlayerData()
        {
            CSteamID = CSteamID.Nil;
        }

    }
}
