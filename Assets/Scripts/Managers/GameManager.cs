using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Net;

namespace CT.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        #region Player

        public static PlayerData Player { get; private set; }
        public static Faction Faction { get; private set; }
        public static BaseData BaseData { get; private set; }
        public static ArmyData ArmyData { get; private set; }
        public static int LastColonyID { get; private set; } = -1;

        public static void ApplyPlayerHome(PlayerData playerData)
        {
            Player = playerData;
            Faction = Faction.GetFaction(Player.factionType);
            BaseData = Player.home;
            ArmyData = BaseData.army;
            LastColonyID = -1;
        }

        public static void ApplyPlayerColony(PlayerData playerData, int colonyID)
        {
            Player = playerData;
            Faction = Faction.GetFaction(Player.factionType);
            BaseData = Player.colonies.Find(col => col.ID == colonyID);
            ArmyData = BaseData.army;
            LastColonyID = colonyID;
        }

        #endregion
        
        public GameObject actionsPanel;
        public GameObject cheatUI;

        public GameObject goldCollectedPrefab, elixirCollectedPrefab;
        public Vector3 collectedUIOffset = Vector3.up * 6;

        public Base _base;

        bool actionChosen = false;

        void Awake()
        {
            instance = this;
            GenerateEnvironment();
            GenerateBase(BaseData);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) cheatUI.SetActive(!cheatUI.activeSelf);
        }

        void GenerateEnvironment()
        {
            var env = Instantiate(Faction.environmentPrefab, transform.position, transform.rotation);
            string envName = Faction.type + " Environment";
            env.name = envName;
        }

        void GenerateBase(BaseData data)
        {
            _base.Generate(data);
        }

        public void FinishConstruction(ConstructionData data)
        {
            if (data.isUpgradeOf == null)
            {
                int level = 1; //may do some change
                var instance = _base.AddBuilding(data.toConstruct, level, data.x, data.y);
                ClientSend.AddBuilding(_base.Data.ID, instance);
            }
            else
            {
                _base.UpgradeBuilding(data);
                ClientSend.UpgradeBuilding(data.isUpgradeOf);
            }
        }

        public void ActionUISelected(bool really)
        {
            if (actionChosen == really) return;
            actionChosen = really;
            actionsPanel.SetActive(!really);
        }
    }
}