using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightsGenerator : MonoBehaviour {
    public float minZ = -250, maxZ = 250, minX = -250, maxX = 250;
    public float minY = 0, maxY = 0.6f;
    public int countMin = 50, countMax = 100;
    public float minIntensity = 0, maxIntensity = 20;
    public float minRange = 0, maxRange = 1;
    public Color[] colors;

    [ContextMenu("Generate")]
    void Generate()
    {
        RemoveOld();
        int count = Random.Range(countMin, countMax);
        for(int i = 0; i < count; i++)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            float z = Random.Range(minZ, maxZ);

            float range = Random.Range(minRange, maxRange);

            int colorIndex = Random.Range(0, colors.Length + 1);
            Color color = colorIndex < colors.Length ? colors[colorIndex] : RandColor();
            float intensity = Random.Range(minIntensity, maxIntensity);

            var obj = new GameObject("Light");
            obj.transform.parent = transform;
            obj.transform.position = new Vector3(x, y, z);

            var light = obj.AddComponent<Light>();
            light.type = LightType.Point;
            light.intensity = intensity;
            light.range = range;
            light.color = color;
        }
    }

    Color RandColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        return new Color(r, g, b);
    }

    void RemoveOld()
    {
        int len = transform.childCount;
        for (int i = len - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
