using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Data
{
    [CreateAssetMenu(fileName = "New Turret", menuName = "Faction/Building/Turret")]
    public class DefensiveData : BuildingData
    {
        public override BuildingType Type => BuildingType.Turret;
        public enum DefenseType { Turret, UnitLauncher }
        public enum TurretType { Hit, Projectile, Laser } //may do other turret types

        public VersionData original;
        public VersionData[] upgrades;

        public override BaseVersionData Original => original;

        public override BaseVersionData[] Upgrades => upgrades;

        [System.Serializable]
        public class VersionData : BaseVersionData
        {
            public DefenseType defenseType;

            //[Header("Turret")]
            public UnitData.CanAttack canAttack;

            public float AttackRate => defenseType == DefenseType.UnitLauncher ? (1f / spawnDelta) : fireRate;
            public float Damage => turretType == TurretType.Projectile ? projectile.GetComponent<Projectile>().damage : nonProjectileDamage;
            public float ExplosionRange => turretType == TurretType.Projectile ? projectile.GetComponent<Projectile>().splashRadius : nonProjectileDamage;
            public float ExplosionDamage => turretType == TurretType.Projectile ? projectile.GetComponent<Projectile>().splashDamage : nonProjectileDamage;

            [Header("Turret")]
            public TurretType turretType;
            public float range;
            [Range(0, 180)]
            public float hitAngle = 30;
            public GameObject projectile;
            public float nonProjectileDamage;
            public float fireRate;

            [Header("UnitLauncher")]
            public UnitData unit;
            public int amount;
            public int capacity;
            public float firstSpawnTime;
            public float spawnDelta;
        }
    }
}
