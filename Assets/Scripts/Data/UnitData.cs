using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Faction/Unit")]
public class UnitData : ScriptableObject {
    new public string name;
    public Sprite icon;
    public float health, attackRange, moveSpeed;
    public enum UnitAt { Ground, Air }
    public UnitAt unitAt;
    public enum CanAttack { Ground, Air, Any }
    public CanAttack canAttack;
    public enum AttackPriority { None, Ground, Air, Resource, Defense, Unit, DefenseAndUnit, Wall }
    public AttackPriority attackPriority;
    public enum UnitType { Melee, Range, Healer, Kamikaze, Magic }
    public UnitType type;
    [HideInInspector] public float healAmount, healDelta;
    [HideInInspector] public float damage, attackSpeed;
    [HideInInspector] public bool splashAttack;
    [HideInInspector] public float splashRadius, splashDamage, splashHeal;
    [HideInInspector] public GameObject attackProjectile, defenseProjectile;
    [HideInInspector] public string spell;
    public GameTime unlockTime, trainTime;
    public int unlockCost, trainCost;
    [Range(1, 100)] public int capacity;
    public GameObject attackingPrefab, defendingPrefab, campWalkerPrefab;
}
