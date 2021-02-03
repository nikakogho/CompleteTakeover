using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor {
    Transform TransformField(string name, Transform value)
    {
        return EditorGUILayout.ObjectField(name, value, typeof(Transform), true) as Transform;
    }

    LayerMask MaskField(string name, LayerMask value)
    {
        int concat = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(value);
        var temp = EditorGUILayout.MaskField(name, concat, InternalEditorUtility.layers);
        return InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(temp);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var unit = target as Unit;
        var data = unit.data;
        if (data == null) return;

        switch (data.type)
        {
            case UnitData.UnitType.Kamikaze:
            case UnitData.UnitType.Melee:
                if (data.splashAttack) unit.impactSpot = TransformField("Impact Spot", unit.impactSpot);
                break;

            case UnitData.UnitType.Range:
                unit.firePoint = TransformField("Fire Point", unit.firePoint);
                break;

            case UnitData.UnitType.Healer:
                unit.comradeMask = MaskField("Comrade Mask", unit.comradeMask);
                break;
        }
    }
}
