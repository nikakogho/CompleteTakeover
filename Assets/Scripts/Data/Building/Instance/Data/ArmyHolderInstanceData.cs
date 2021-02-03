using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data.Instance
{
    public class ArmyHolderInstanceData : BuildingInstanceData
    {
        public override BuildingType Type => BuildingType.ArmyHolder;

        public ArmyHoldData.VersionData CurrentData => BaseCurrentData as ArmyHoldData.VersionData;
        public ArmyHoldData.VersionData NextData => BaseNextData as ArmyHoldData.VersionData;
        public ArmyHoldData.VersionData LastData => BaseLastData as ArmyHoldData.VersionData;

        public ArmyHolderInstanceData(int id, ArmyHoldData data, int level, int tileX, int tileY, bool destroyed)
        : base(id, data, level, tileX, tileY, destroyed)
        {

        }

        public ArmyHolderInstanceData(ArmyHoldData data, int level, int tileX, int tileY)
        :this(0, data, level, tileX, tileY, false)
        {

        }
    }
}