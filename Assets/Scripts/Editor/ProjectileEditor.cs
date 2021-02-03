using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var proj = target as Projectile;
        if (!proj.HasSplashAttack) return;
        proj.splashRadius = EditorGUILayout.Slider("Splash Radius", proj.splashRadius, 0, 1000);
        proj.splashDamage = EditorGUILayout.FloatField("Splash Damage", proj.splashDamage);
    }
}
