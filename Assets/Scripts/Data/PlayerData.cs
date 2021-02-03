using System;
using System.Collections.Generic;
using UnityEngine;
using CT.Helper;

namespace CT.Data
{
    public class PlayerData
    {
        public string username;
        public byte[] password;
        public Sprite icon;
        public Faction.Type factionType;

        public BaseData home;
        public List<BaseData> colonies;

        public Faction Faction => Faction.GetFaction(factionType);
        
        public int gems;

        public DateTime lastOnline;

        /*
        public PlayerData(string username, string password, int a, Faction.Type factionType, Sprite icon = null)
        {
            this.username = username;
            this.password = Hasher.Hash(password);
            this.factionType = factionType;
            this.icon = icon;
            lastOnline = DateTime.Now;
        }
        */

        public PlayerData(string username, byte[] password, Faction.Type factionType, Sprite icon,
                          int gems, DateTime lastOnline, BaseData home, List<BaseData> colonies)
        {
            this.username = username;
            this.password = password;
            this.factionType = factionType;
            this.icon = icon;
            this.gems = gems;
            this.lastOnline = lastOnline;
            this.home = home;
            this.colonies = colonies;

            home.ApplyPlayer(this);
            foreach (var colony in colonies) colony.ApplyPlayer(this);
        }
    }
}