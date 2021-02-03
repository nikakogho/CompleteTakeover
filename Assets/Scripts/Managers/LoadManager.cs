using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using CT.Helper;
using CT.Data;
using CT.Net;

namespace CT.Manager
{
    public class LoadManager : MonoBehaviour
    {
        public Sprite goldIcon;
        public FactionStat[] factionStats = new FactionStat[System.Enum.GetNames(typeof(Faction.Type)).Length];

        public InputField registerUsernameField, registerPasswordField, repeatPasswordField;
        public InputField loadUsernameField, loadPasswordField;
        public Text registerFailText, loadFailText;
        public Dropdown factionDropdown;

        public Text connectedText;

        public string mainGameScene;

        public IconStuff[] icons;
        public BuildingData[] buildings;

        public static Dictionary<string, Sprite> iconsDictionary;
        public static Dictionary<string, BuildingData> buildingsDictionary;
        public static Dictionary<string, UnitData> unitsDictionary;

        public static LoadManager instance;

        void Awake()
        {
            instance = this;

            InitDictionaries();
            InitFactions();

            DontDestroyOnLoad(gameObject);
        }

        #region Init

        void InitDictionaries()
        {
            iconsDictionary = new Dictionary<string, Sprite>();
            buildingsDictionary = new Dictionary<string, BuildingData>();
            unitsDictionary = new Dictionary<string, UnitData>();
            foreach (var icon in icons) iconsDictionary.Add(icon.name, icon.icon);
            foreach (var building in buildings) buildingsDictionary.Add(building.name, building);
        }

        void InitFactions()
        {
            Faction.InitFactions(factionStats);
            factionDropdown.ClearOptions();
            factionDropdown.AddOptions(System.Enum.GetNames(typeof(Faction.Type)).ToList());

            foreach (var stat in factionStats)
            {
                iconsDictionary.Add(stat.type + " Player Default", stat.playerDefaultIcon);
                buildingsDictionary.Add(stat.type + " Main Hall", stat.mainHallData);
                foreach (var unit in stat.units) unitsDictionary.Add(unit.name, unit);
                foreach (var building in stat.buildings) buildingsDictionary.Add(building.name, building);
            }
        }

        #endregion

        #region Game Load

        public void FailedToLoad()
        {
            loadFailText.text = "Failed to load!";
        }

        public void FailedToRegister(string message = "Failed to register!")
        {
            registerFailText.text = message;
        }

        public static void RequestPlayerDataUpdate()
        {
            ClientSend.RequestPlayerDataUpdate(GameManager.Player.username);
        }

        public void LoadPlayer(PlayerData player)
        {
            if (GameManager.LastColonyID == -1) GameManager.ApplyPlayerHome(player);
            else GameManager.ApplyPlayerColony(player, GameManager.LastColonyID);
            SceneManager.LoadScene(mainGameScene);
        }

        #endregion

        #region Button Clicks

        public void NewGame()
        {
            string username = registerUsernameField.text;
            var factionType = (Faction.Type)factionDropdown.value;
            string password = registerPasswordField.text;
            string faction = factionType.ToString();

            if (password != repeatPasswordField.text)
            {
                FailedToRegister("Passwords don't match!");
                return;
            }

            ClientSend.RegisterRequest(username, password, faction);
            /*
            var player = NewGame(username, factionType, password);
            if (player == null) FailedLoading("Username or Password invalid", true);
            else LoadPlayer(player);
            */
        }

        public void LoadGame()
        {
            string username = loadUsernameField.text;
            string password = loadPasswordField.text;
            //Debug.Log("Passing pass " + password);
            var passBytes = Hasher.Hash(password);

            ClientSend.LoginRequest(username, passBytes);

            /*
            var player = LoadGame(username, password);
            if (player == null) FailedLoading("Username or Password Invalid");
            else LoadPlayer(player);
            */
        }

        public void LoadGameFromRegistration()
        {
            string username = registerUsernameField.text;
            string password = registerPasswordField.text;
            var passBytes = Hasher.Hash(password);

            ClientSend.LoginRequest(username, passBytes);
        }

        public void Exit()
        {
            Application.Quit();
        }

        #endregion

        void OnValidate()
        {
            int len = System.Enum.GetNames(typeof(Faction.Type)).Length;
            if (factionStats.Length != len)
            {
                var stats = new FactionStat[len];
                if (factionStats.Length > len) for (int i = 0; i < len; i++) stats[i] = factionStats[i];
                else
                {
                    for (int i = 0; i < factionStats.Length; i++) stats[i] = factionStats[i];
                    for (int i = factionStats.Length; i < len; i++) stats[i] = new FactionStat();
                }
                factionStats = stats;
            }
            for (int i = 0; i < len; i++)
                if (factionStats[i].type != (Faction.Type)i) factionStats[i].type = (Faction.Type)i;
        }

        [System.Serializable]
        public class FactionStat
        {
            public Faction.Type type;
            public string elixirName;
            public Sprite elixirIcon;
            public Sprite factionIcon;
            public Sprite playerDefaultIcon;
            public GameObject environmentPrefab;
            public BuildingData mainHallData;
            public BuildingData[] buildings;
            public UnitData[] units;
        }

        [System.Serializable]
        public class IconStuff
        {
            public string name;
            public Sprite icon;
        }
    }
}