using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitData))]
public class UnitDataEditor : Editor
{
    GameObject PrefabField(string name, GameObject value)
    {
        return EditorGUILayout.ObjectField(name, value, typeof(GameObject), false) as GameObject;
    }

    float FloatField(string name, float value, float max)
    {
        return EditorGUILayout.Slider(name, value, 0, max);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var data = target as UnitData;
        switch (data.type)
        {
            case UnitData.UnitType.Melee:
                data.damage = FloatField("Damage", data.damage, 10000);
                data.attackSpeed = FloatField("Attack Speed", data.attackSpeed, 1000);
                data.splashAttack = EditorGUILayout.Toggle("Splash Attack", data.splashAttack);
                if (data.splashAttack)
                {
                    data.splashRadius = FloatField("Splash Radius", data.splashRadius, 100);
                    data.splashDamage = FloatField("Splash Damage", data.splashDamage, 10000);
                }
                break;

            case UnitData.UnitType.Range:
                data.attackSpeed = FloatField("Fire Rate", data.attackSpeed, 1000);
                data.attackProjectile = PrefabField("Attack Projectile", data.attackProjectile);
                data.defenseProjectile = PrefabField("Defense Projectile", data.defenseProjectile);
                break;

            case UnitData.UnitType.Healer:
                data.healAmount = FloatField("Heal Amount", data.healAmount, 1000000);
                data.healDelta = FloatField("Heal Delta", data.healDelta, 600);
                data.splashAttack = EditorGUILayout.Toggle("Splash Heal", data.splashAttack);
                if (data.splashAttack)
                {
                    data.splashRadius = FloatField("Splash Radius", data.splashRadius, 100);
                    data.splashHeal = FloatField("Splash Heal Amount", data.splashHeal, 10000);
                }
                break;

            case UnitData.UnitType.Kamikaze:
                if (!data.splashAttack) data.splashAttack = true;
                data.splashRadius = FloatField("Explosion Radius", data.splashRadius, 100);
                data.splashDamage = FloatField("Damage", data.splashDamage, 10000);
                break;

            case UnitData.UnitType.Magic:
                data.attackSpeed = 1f / FloatField("Spell Cast Delta", 1f / data.attackSpeed, 100);
                data.spell = EditorGUILayout.TextField("Spell", data.spell);
                break;
        }
    }
}
