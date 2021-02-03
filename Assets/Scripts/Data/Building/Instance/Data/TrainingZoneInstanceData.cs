using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Data.Instance
{
    public class TrainingZoneInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.TrainingZone;

        public string trainingUnitName;
        public GameTime timeLeft;

        public List<TrainSlot> slots;

        static List<TrainSlot> StartingSlots => new List<TrainSlot>();

        public TrainingZoneData.VersionData CurrentData => BaseCurrentData as TrainingZoneData.VersionData;
        public TrainingZoneData.VersionData NextData => BaseNextData as TrainingZoneData.VersionData;
        public TrainingZoneData.VersionData LastData => BaseLastData as TrainingZoneData.VersionData;

        public TrainingZoneInstanceData(int id, TrainingZoneData data, int level, string unitName, 
            GameTime timeLeft, List<TrainSlot> slots, int tileX, int tileY, bool destroyed)
        :base(id, data, level, tileX, tileY, destroyed)
        {
            trainingUnitName = unitName;
            this.timeLeft = timeLeft;
            this.slots = slots;
        }

        public TrainingZoneInstanceData(TrainingZoneData data, int level, int tileX, int tileY)
        : this(0, data, level, null, GameTime.Zero, StartingSlots, tileX, tileY, false)
        {

        }

        //[Serializable]
        public class TrainSlot
        {
            public UnitData unit;
            public int amount;

            public GameTime SlotTime => unit.trainTime * amount;
            static GameTime OneGemCostTime => GameTime.Hour;

            public int GemCost => (SlotTime / OneGemCostTime) + (SlotTime % OneGemCostTime == GameTime.Zero ? 0 : 1);

            public TrainSlot(ArmySquad squad)
            :this(squad.unit, squad.amount)
            {

            }

            public TrainSlot(UnitData unit, int amount)
            {
                this.unit = unit;
                this.amount = amount;
            }
        }
    }
}
