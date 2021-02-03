using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.UI;
using CT.Data.Instance;

namespace CT.Instance
{
    public class Turret : Building//<DefensiveData, TurretInstanceData>
    {
        public DefensiveData Data => BaseData as DefensiveData;
        public TurretInstanceData InstanceData => BaseInstanceData as TurretInstanceData;
        public DefensiveData.VersionData VersionData => InstanceData.CurrentData;

        public LayerMask enemyMask;

        public Transform pivot;

        [Header("Projectile")]
        public Transform firePoint;

        [Header("Unit Launcher")]
        public Transform[] unitSpots;

        const float updateTargetDelta = 0.3f;

        float countdown;
        Unit target;

        Dictionary<Transform, Unit> units;

        const float rotateSpeed = 100;

        float HitAngle => VersionData.hitAngle;

        bool canAttackGround;
        bool canAttackAir;

        protected override void Awake()
        {
            base.Awake();
            units = new Dictionary<Transform, Unit>();
            foreach (var spot in unitSpots) units.Add(spot, null);
        }

        void Start()
        {
            switch (VersionData.defenseType)
            {
                case DefensiveData.DefenseType.Turret: countdown = 0; break;
                case DefensiveData.DefenseType.UnitLauncher: StartCoroutine(SpawnRoutine()); break;
            }
            switch (VersionData.canAttack)
            {
                case UnitData.CanAttack.Air: canAttackAir = true; canAttackGround = false; break;
                case UnitData.CanAttack.Ground: canAttackAir = false; canAttackGround = true; break;
                case UnitData.CanAttack.Any: canAttackAir = true; canAttackGround = true; break;
            }
            InvokeRepeating("UpdateTarget", updateTargetDelta, updateTargetDelta);
        }

        Transform GetSpawnPoint()
        {
            foreach (var spot in units) if (spot.Value == null) return spot.Key;
            return null;
        }

        IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(VersionData.firstSpawnTime);
            while (enabled)
            {
                for (int i = 0; i < VersionData.amount; i++) if (!SpawnUnit()) break;
                yield return new WaitForSeconds(VersionData.spawnDelta);
            }
        }

        bool SpawnUnit()
        {
            var spawnPoint = GetSpawnPoint();
            if (spawnPoint == null) return false;
            var obj = Instantiate(VersionData.unit.defendingPrefab, spawnPoint.position, spawnPoint.rotation, transform);
            var unit = obj.GetComponent<Unit>();
            units[spawnPoint] = unit;
            return true;
        }

        void Fire()
        {
            switch (VersionData.turretType)
            {
                case DefensiveData.TurretType.Hit: target.TakeDamage(VersionData.nonProjectileDamage); break;
                case DefensiveData.TurretType.Laser: target.TakeDamage(VersionData.nonProjectileDamage); break; //to do laser stuff
                case DefensiveData.TurretType.Projectile:
                    var obj = Instantiate(VersionData.projectile, firePoint.position, firePoint.rotation);
                    obj.transform.parent = firePoint;
                    obj.transform.LookAt(target.heart);
                    //var projectile = obj.GetComponent<Projectile>(); may do something with it
                    break;
            }
            countdown = 1f / VersionData.fireRate;
        }

        void UpdateTarget()
        {
            float minDist = float.MaxValue;
            target = null;
            foreach (var enemy in Physics.OverlapSphere(pivot.position, VersionData.range, enemyMask))
            {
                var unit = enemy.GetComponent<Unit>();
                float dist = Vector3.SqrMagnitude(pivot.position - unit.heart.position);
                if (dist >= minDist) continue;
                if (unit.data.unitAt == UnitData.UnitAt.Ground && !canAttackGround) continue;
                if (unit.data.unitAt == UnitData.UnitAt.Air && !canAttackAir) continue;
                minDist = dist;
                target = unit;
            }
        }

        void Update()
        {
            if (VersionData.defenseType == DefensiveData.DefenseType.UnitLauncher) return;
            if (countdown > 0) countdown -= Time.deltaTime;
            if (target != null)
            {
                Vector3 dir = target.heart.position - pivot.position;
                dir.y = 0;
                float angle = Vector3.Angle(pivot.forward, dir);
                dir.y = 0;
                float rotateValue = rotateSpeed * Time.deltaTime;
                pivot.rotation = Quaternion.Slerp(pivot.rotation, Quaternion.LookRotation(dir), rotateValue);

                if (countdown <= 0 && angle <= HitAngle) Fire();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (BaseData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pivot.position, VersionData.range);
        }

        void OnValidate()
        {
            if (pivot == null) pivot = transform;
            if (BaseData == null) return;
            if (InstanceData == null) return;
            if (VersionData.defenseType == DefensiveData.DefenseType.Turret) return;
            if (unitSpots.Length > VersionData.capacity)
            {
                var arr = new Transform[VersionData.capacity];
                for (int i = 0; i < VersionData.capacity; i++) arr[i] = unitSpots[i];
                unitSpots = arr;
            }
            else if (unitSpots.Length < VersionData.capacity)
            {
                var arr = new Transform[VersionData.capacity];
                for (int i = 0; i < unitSpots.Length; i++) arr[i] = unitSpots[i];
            }
        }

        public override void OnInteract()
        {
            OnUpgradeUISelected();
        }

        public override void OnUpgradeUISelected()
        {
            UpgradeUIManager.instance.Select(this);
        }
    }
}