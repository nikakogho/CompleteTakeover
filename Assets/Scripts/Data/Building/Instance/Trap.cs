using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Data.Instance;
using CT.UI;

namespace CT.Instance
{
    public class Trap : Building//<TrapData, TrapInstanceData>
    {
        public TrapData Data => BaseData as TrapData;
        public TrapInstanceData InstanceData => BaseInstanceData as TrapInstanceData;
        public TrapData.VersionData VersionData => InstanceData.CurrentData;

        public LayerMask enemyMask;
        bool used = false;

        void OnTriggerEnter(Collider other)
        {
            if (used) return;
            used = true;

            var attackable = other.GetComponent<Attackable>();

            if (attackable != null) attackable.TakeDamage(VersionData.damage);

            Explode();

            GetComponent<Collider>().enabled = false;
        }

        void Explode()
        {
            if (VersionData.explosionRange == 0) return;
            foreach (var col in Physics.OverlapSphere(transform.position, VersionData.explosionRange, enemyMask))
                col.GetComponent<Attackable>().TakeDamage(VersionData.explosionDamage);
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