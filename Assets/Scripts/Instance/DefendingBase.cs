using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Manager.Combat;
using System.Linq;
using CT.UI.Manager;

public class DefendingBase : MonoBehaviour {
    AICombatManager aiManager;
    PlayerCombatManager playerManager;

    bool IsPlayer => aiManager == null;

    BaseData data;
    List<Building> buildings;
    List<Building> standingBuildings;

    public List<Building> DestroyedBuildings { get; private set; }

    public static DefendingBase instance;
    
    public static int lostTotalGold;
    public static int lostTotalElixir;

    public Dictionary<int, int> RobbedMines { get; private set; }
    public Dictionary<int, int> RobbedStorages { get; private set; }

    int mainHallTakenGold;
    int mainHallTakenElixir;

    void Awake()
    {
        instance = this;
        lostTotalGold = lostTotalElixir = 0;
        DestroyedBuildings = new List<Building>();
        RobbedMines = new Dictionary<int, int>();
        RobbedStorages = new Dictionary<int, int>();
        mainHallTakenGold = mainHallTakenElixir = 0;
    }

	void Start () {
        aiManager = AICombatManager.Instance;
        playerManager = PlayerCombatManager.Instance;
	}

    bool IsValidType(BuildingData.BuildingType type)
    {
        switch (type)
        {
            case BuildingData.BuildingType.Wall:
            case BuildingData.BuildingType.Trap:
            case BuildingData.BuildingType.Decoration:
                return false;

            default: return true;
        }
    }

    public void Apply(BaseData data)
    {
        this.data = data;
        buildings = new List<Building>();
        int buildingCount = data.buildings.Count;
        //Debug.Log("Buildings count: " + buildingCount);
        foreach (var building in data.buildings) GenerateBuilding(building);
        standingBuildings = buildings.Where(b => !b.BaseInstanceData.destroyed && 
                            IsValidType(b.BaseData.Type)).ToList();
    }

    void GenerateBuilding(BuildingInstanceData instanceData)
    {
        float x = instanceData.tileX * BaseData.TileDistance;
        float y = 0.03f;
        float z = instanceData.tileY * BaseData.TileDistance;
        Vector3 pos = transform.position + new Vector3(x, y, z);
        var obj = Instantiate(instanceData.BaseCurrentData.prefab, pos, Quaternion.identity, transform);
        var building = obj.GetComponent<Building>();
        building.ApplyCombatInstanceData(this, instanceData);
        buildings.Add(building);
    }

    public void OnMineRobbed(Vector3 pos, int id, bool isGold, int amount)
    {
        if (amount == 0) return;

        if (isGold)
        {
            SpawnTakenGoldUI(pos, amount);
            lostTotalGold += amount;
        }
        else
        {
            SpawnTakenElixirUI(pos, amount);
            lostTotalElixir += amount;
        }

        if (RobbedMines.ContainsKey(id)) RobbedMines[id] += amount;
        else RobbedMines.Add(id, amount);
    }

    public void OnStorageRobbed(Vector3 pos, int id, bool isGold, int amount)
    {
        if (amount == 0) return;

        if (isGold)
        {
            SpawnTakenGoldUI(pos, amount);
            lostTotalGold += amount;
        }
        else
        {
            SpawnTakenElixirUI(pos, amount);
            lostTotalElixir += amount;
        }

        if (RobbedStorages.ContainsKey(id)) RobbedStorages[id] += amount;
        else RobbedStorages.Add(id, amount);
    }

    public void OnMainHallRobbed(Vector3 pos, int gold, int elixir)
    {
        if (gold == 0 && elixir == 0) return;

        mainHallTakenGold += gold;
        mainHallTakenElixir += elixir;
        if (gold > 0)
        {
            SpawnTakenGoldUI(pos, gold);
            lostTotalGold += gold;
        }
        if(elixir > 0)
        {
            SpawnTakenElixirUI(pos, elixir);
            lostTotalElixir += elixir;
        }
    }

    void SpawnTakenGoldUI(Vector3 pos, int amount)
    {
        LootedFromBuildingUIManager.instance.AddGoldLoot(pos, amount);
    }

    void SpawnTakenElixirUI(Vector3 pos, int amount)
    {
        LootedFromBuildingUIManager.instance.AddElixirLoot(pos, amount, data.Faction);
    }

    public void OnDestroyed(Building building)
    {
        if (!standingBuildings.Contains(building)) return;
        standingBuildings.Remove(building);
        DestroyedBuildings.Add(building);
        if(standingBuildings.Count == 0)
        {
            if (IsPlayer) playerManager.BaseDestroyed();
            else aiManager.BaseDestroyed();
        }
    }
}
