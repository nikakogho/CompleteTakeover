using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Manager;

namespace CT.UI.Manager
{
    public class LootedFromBuildingUIManager : MonoBehaviour
    {
        public GameObject goldPrefab, elixirPrefab;
        Dictionary<Vector3, Queue<ResourcesTakenUI>> loots;

        public Vector3 offset = Vector3.up * 5;

        public static LootedFromBuildingUIManager instance;

        void Awake()
        {
            instance = this;
            loots = new Dictionary<Vector3, Queue<ResourcesTakenUI>>();
            InvokeRepeating("LootUpdate", 1, 0.2f);
        }

        void SpawnLoot(Vector3 pos, int amount, GameObject prefab, Sprite icon)
        {
            Vector3 spawnPos = pos + offset;
            //Debug.Log($"{pos} + {offset} = {spawnPos}");
            var obj = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().position = spawnPos;
            var ui = obj.GetComponent<ResourcesTakenUI>();
            ui.Init(amount, icon);

            if (loots.ContainsKey(pos)) loots[pos].Enqueue(ui);
            else loots.Add(pos, new Queue<ResourcesTakenUI>(new ResourcesTakenUI[] { ui }));
        }

        public void AddGoldLoot(Vector3 pos, int amount)
        {
            SpawnLoot(pos, amount, goldPrefab, LoadManager.instance.goldIcon);
        }

        public void AddElixirLoot(Vector3 pos, int amount, Faction defenderFaction)
        {
            SpawnLoot(pos, amount, elixirPrefab, defenderFaction.elixirIcon);
        }

        void LootUpdate()
        {
            foreach(var spot in loots)
            {
                var queue = spot.Value;
                if (queue.Count == 0) continue;
                var ui = queue.Dequeue();
                ui.MoveUp();
            }
        }
    }
}
