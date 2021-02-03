using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Instance
{
    public class MainHallInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.MainHall;

        public int storedGold;
        public int storedElixir;

        static int StartGold => 1000;
        static int StartElixir => 1000;

        public MainHallData Data => data as MainHallData;
        public MainHallData.VersionData CurrentData => BaseCurrentData as MainHallData.VersionData;
        public MainHallData.VersionData NextData => BaseNextData as MainHallData.VersionData;
        public MainHallData.VersionData LastData => BaseLastData as MainHallData.VersionData;

        public int GoldCapacity => CurrentData.goldCapacity;
        public int ElixirCapacity => CurrentData.elixirCapacity;

        public MainHallInstanceData(int id, MainHallData data, int level, int gold, int elixir, int tileX, int tileY, bool destroyed)
            : base(id, data, level, tileX, tileY, destroyed)
        {
            storedGold = gold;
            storedElixir = elixir;
        }

        public MainHallInstanceData(MainHallData data, int level, int tileX, int tileY)
        : this(0, data, level, StartGold, StartElixir, tileX, tileY, false)
        {

        }
    }
}