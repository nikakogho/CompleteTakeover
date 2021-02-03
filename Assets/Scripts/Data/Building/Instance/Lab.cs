using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.Manager;
using CT.UI;

namespace CT.Instance
{
    public class Lab : Building//<LabData, LabInstanceData>
    {
        public LabData Data => BaseData as LabData;
        public LabInstanceData InstanceData => BaseInstanceData as LabInstanceData;
        public LabData.VersionData VersionData => InstanceData.CurrentData;

        public UnitData WorkingOnUnit { get; private set; }
        public GameTime TimeLeft => InstanceData.timeLeft;

        public override void ApplyInstanceData(Base _base, BuildingInstanceData instanceData)
        {
            base.ApplyInstanceData(_base, instanceData);
            if (InstanceData.workingOnUnit != null)
                WorkingOnUnit = LoadManager.unitsDictionary[InstanceData.workingOnUnit];
        }

        public override void OnInteract()
        {
            //open ui
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}
