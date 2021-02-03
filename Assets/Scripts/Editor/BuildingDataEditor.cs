using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CT.Data;

[CustomEditor(typeof(BuildingData))]
public class BuildingDataEditor : Editor {
    protected UnitData UnitField(string name, UnitData value)
    {
        return EditorGUILayout.ObjectField(name, value, typeof(UnitData), false) as UnitData;
    }

    protected GameObject PrefabField(string name, GameObject value)
    {
        return EditorGUILayout.ObjectField(name, value, typeof(GameObject), false) as GameObject;
    }
}

[CustomEditor(typeof(ArmyHoldData))]
public class ArmyHoldDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(BunkerData))]
public class BunkerDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(DefensiveData))]
public class DefensiveDataEditor : BuildingDataEditor
{
    DefensiveData.TurretType TurretTypeField(string name, DefensiveData.TurretType value)
    {
        return (DefensiveData.TurretType)EditorGUILayout.EnumPopup(name, value);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        var data = target as DefensiveData;
        switch (data.defenseType)
        {
            case DefensiveData.DefenseType.Turret:
                data.range = EditorGUILayout.Slider("Range", data.range, 0, 1000);
                data.fireRate = EditorGUILayout.Slider("Fire Rate", data.fireRate, 0, 100);
                data.turretType = TurretTypeField("Turret Type", data.turretType);
                switch (data.turretType)
                {
                    case DefensiveData.TurretType.Hit:
                        data.damage = EditorGUILayout.FloatField("Damage", data.damage);
                        break;

                    case DefensiveData.TurretType.Projectile:
                        data.projectile = PrefabField("Projectile", data.projectile);
                        break;

                    case DefensiveData.TurretType.Laser:
                        data.damage = EditorGUILayout.FloatField("Damage", data.damage);
                        //to do some kind of laser effects stuff
                        break;
                }
                break;

            case DefensiveData.DefenseType.UnitLauncher:
                data.unit = UnitField("Unit", data.unit);
                data.amount = EditorGUILayout.IntSlider("Amount", data.amount, 1, 1000);
                data.firstSpawnTime = EditorGUILayout.FloatField("First Spawn Time", data.firstSpawnTime);
                data.spawnDelta = EditorGUILayout.FloatField("Spawn Delta", data.spawnDelta);
                break;
        }
        */
    }
}

[CustomEditor(typeof(LabData))]
public class LabDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(ArmyHoldData))]
public class MainHallDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(ArmyHoldData))]
public class MineDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(ArmyHoldData))]
public class StorageDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(ArmyHoldData))]
public class TrainingZoneDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(WallData))]
public class WallDataEditor : BuildingDataEditor
{

}

[CustomEditor(typeof(TrapData))]
public class TrapDataEditor : BuildingDataEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        var trap = target as TrapData;
        if (trap.explosionRange > 0)
            trap.explosionDamage = EditorGUILayout.FloatField("Explosion Damage", trap.explosionDamage);
        */
    }
}

[CustomEditor(typeof(DecorationData))]
public class DecorationDataEditor : BuildingDataEditor
{

}