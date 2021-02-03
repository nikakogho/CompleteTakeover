using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class Unit : Attackable {
    public UnitData data;
    public float Health { get; private set; }
    public Transform heart;
    public LayerMask enemyMask;
    [HideInInspector] public Transform firePoint;
    [HideInInspector] public Transform impactSpot;
    [HideInInspector] public LayerMask comradeMask;

    public GameObject deathEffect;
    public float deathEffectLifetime = 5;

    public bool IsDead { get; private set; }

    public override Transform Center => heart;

    public bool isDefense;
    bool patrols = false;
    bool setToGoHome;
    Vector3 patrolPoint;
    float seeRange = 10000000;

    MoveAgent agent;
    Animator anim;
    Collider col;

    bool ready = true;

    public override bool IsAlive => !IsDead;
    public override Collider BoundsCollider => col;

    Attackable target;
    const float updateTargetDelta = 0.5f;

    public Vector3 ClosestPointToTarget => target.BoundsCollider.ClosestPointOnBounds(transform.position);
    public float DistanceFromClosestPoint => Vector3.Distance(transform.position, ClosestPointToTarget);
    public bool IsInAttackRange => target == null ? false : DistanceFromClosestPoint < data.attackRange;

    void Awake()
    {
        Health = data.health;
        col = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        agent = GetComponent<MoveAgent>();
        InvokeRepeating("UpdateTarget", 0.2f, updateTargetDelta);
    }

    void UpdateTarget()
    {
        float minDist = float.MaxValue;
        target = null;
        foreach(var col in Physics.OverlapSphere(transform.position, seeRange, enemyMask))
        {
            float dist = Vector3.SqrMagnitude(col.transform.position - transform.position);
            if (dist >= minDist) continue;
            var attackable = col.GetComponent<Attackable>();
            if (!attackable.IsAlive) continue;
            minDist = dist;
            target = attackable;
        }
        if (target == null) return;
        setToGoHome = false;
        agent.FollowTargetAtDist(target.Center, data.attackRange - 0.2f);
    }

    void Update()
    {
        if (target == null)
        {
            if (!patrols) return;
            if (setToGoHome) return;
            setToGoHome = true;
            agent.SetDestination(patrolPoint);
            return;
        }

        if (ready && IsInAttackRange)
        {
            Attack();
            StartCoroutine(GetReadyRoutine());
        }
    }

    [ContextMenu("To Walking")]
    void ConvertToWalking()
    {
        DestroyImmediate(GetComponent<Collider>());
        DestroyImmediate(GetComponent<MoveAgent>());
        gameObject.AddComponent<CampMoveAgent>();
        DestroyImmediate(this);
    }

    public void ApplyDefenseData(Vector3 patrolSpot, float seeRange)
    {
        patrols = true;
        patrolPoint = patrolSpot;
        this.seeRange = seeRange;
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead) return;
        anim.SetTrigger("Take Damage");
        Health -= damage;
        if (Health <= 0) Die();
    }

    public override void HealBy(float value)
    {
        if (IsDead) return;
        Health = Mathf.Clamp(Health + value, 0, data.health);
    }

    public void Attack()
    {
        if (IsDead) return;
        if (!ready) return;

        switch (data.type)
        {
            case UnitData.UnitType.Melee: MeleeAttack(); break;
            case UnitData.UnitType.Range: RangeAttack(); break;
            case UnitData.UnitType.Kamikaze: KamikazeAttack(); break;
            case UnitData.UnitType.Magic: MagicalAttack(); break;
            case UnitData.UnitType.Healer: HealerHeal(); break;
        }
    }

    IEnumerator GetReadyRoutine()
    {
        ready = false;
        float waitTime = 10;
        switch (data.type)
        {
            case UnitData.UnitType.Melee:
            case UnitData.UnitType.Range:
            case UnitData.UnitType.Magic:
                waitTime = 1f / data.attackSpeed;
                break;

            case UnitData.UnitType.Healer:
                waitTime = data.healDelta;
                break;
        }
        yield return new WaitForSeconds(waitTime);
        ready = true;
    }

    #region Attack

    void SplashAction(Vector3 center, bool heal = false)
    {
        if (!data.splashAttack) return;
        LayerMask mask = heal ? comradeMask : enemyMask;
        foreach (var col in Physics.OverlapSphere(center, data.splashRadius, mask))
        {
            var attackable = col.GetComponent<Attackable>();
            if (attackable == null) continue;
            if (heal) attackable.HealBy(data.splashHeal);
            else attackable.TakeDamage(data.splashDamage);
        }
    }

    void MeleeAttack()
    {
        anim.SetTrigger("Attack");
        target.TakeDamage(data.damage);
        SplashAction(transform.position);
    }

    void KamikazeAttack()
    {
        anim.SetTrigger("Kaboom!");
        SplashAction(transform.position);
        Die();
    }

    void RangeAttack()
    {
        anim.SetTrigger("Attack");
        var prefab = isDefense ? data.defenseProjectile : data.attackProjectile;
        var projectile = Instantiate(prefab, firePoint.position, firePoint.rotation);
        projectile.transform.LookAt(target.Center);
    }

    void MagicalAttack()
    {
        anim.SetTrigger("Attack");
        //to do invoke from special script
    }

    void HealerHeal()
    {
        anim.SetTrigger("Heal");
        target.HealBy(data.healAmount);
        SplashAction(target.transform.position, true);
    }

    #endregion

    void DeathEffectStuff()
    {
        if (deathEffect == null) return;
        var effect = Instantiate(deathEffect, transform.position, transform.rotation);
        effect.name = name + " Corpse";
        Destroy(effect, deathEffectLifetime);
    }

    void Die()
    {
        IsDead = true;
        DeathEffectStuff();
        Destroy(gameObject);
    }

    void OnValidate()
    {
        if (heart == null) heart = transform;
    }

    void OnGizmosDrawSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(heart.position, data.attackRange);
    }
}
