using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Data.Instance
{
    public class MineInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.Mine;

        public int stored;
        public MineData Data => data as MineData;
        public MineData.VersionData CurrentData => BaseCurrentData as MineData.VersionData;
        public MineData.VersionData NextData => BaseNextData as MineData.VersionData;
        public MineData.VersionData LastData => BaseLastData as MineData.VersionData;
        public int Capacity => CurrentData.capacity;

        static int StartStored => 0;

        public float FilledPortion => (float)stored / Capacity;
        public GameTime FillTimeLeft => new GameTime((int)(CurrentData.fillTime.TotalSeconds * FilledPortion));

        public MineInstanceData(int id, MineData data, int level, int storedAmount, int tileX, int tileY, bool destroyed)
        :base(id, data, level, tileX, tileY, destroyed)
        {
            if (data.Type != BuildingData.BuildingType.Mine)
                throw new ArgumentException("Building Data must be Mine!");
            stored = storedAmount;
        }
        
        public MineInstanceData(MineData data, int level, int tileX, int tileY)
        : this(0, data, level, StartStored, tileX, tileY, false)
        {

        }
    }
}
