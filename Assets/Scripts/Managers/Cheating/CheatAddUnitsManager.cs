using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.UI.Cheating;

namespace CT.Manager
{
    public class CheatAddUnitsManager : MonoBehaviour {
        public GameObject prefab;

        List<CheatAddUnitUI> options;

        public CheatAddUnitsManager()
        {
            options = new List<CheatAddUnitUI>();
        }

        void InitForUnit(UnitData unit)
        {
            var obj = Instantiate(prefab, transform);
            var ui = obj.GetComponent<CheatAddUnitUI>();
            ui.Init(unit);
        }

        void Generate()
        {
            foreach (var unit in LoadManager.unitsDictionary) InitForUnit(unit.Value);
        }

        void Start()
        {
            if (options.Count == 0) Generate();
        }
    }
}