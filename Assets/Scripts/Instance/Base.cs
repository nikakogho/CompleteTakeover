using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CT.Data;
using CT.Data.Instance;
using CT.UI;
using CT.Instance;
using CT.Army;
using CT.Net;
using CT.Manager;

public class Base : MonoBehaviour {
    public BaseData Data { get; private set; }
    public List<Building> Buildings { get; private set; }

    #region Buildings

    public MainHall MainHall { get; private set; }
    public Lab Lab { get; private set; }
    public List<CampZone> ArmyZones { get; private set; }
    public List<Bunker> Bunkers { get; private set; }
    public List<Mine> Mines { get; private set; }
    public List<Mine> GoldMines { get; private set; }
    public List<Mine> ElixirMines { get; private set; }
    public List<Mine> GemMines { get; private set; }
    public List<Storage> GoldStorages { get; private set; }
    public List<Storage> ElixirStorages { get; private set; }
    public List<TrainingZone> TrainingZones { get; private set; }
    public List<Turret> Turrets { get; private set; }
    public List<Wall> Walls { get; private set; }
    public List<Trap> Traps { get; private set; }
    public List<Decoration> Decorations { get; private set; }

    #endregion

    public float tileDistance = BaseData.TileDistance;

    public Transform reportsParent;
    public GameObject defenseReportPrefab, attackReportPrefab;

    public Building this[int x, int y]
    {
        get
        {
            foreach (var building in Buildings)
                if (building.BaseInstanceData.tileX <= x && building.BaseInstanceData.tileX + building.BaseData.tileWidth > x)
                    if (building.BaseInstanceData.tileY <= y && building.BaseInstanceData.tileY + building.BaseData.tileHeight > y)
                        return building;

            return null;
        }
    }

    #region Resources

    public int Gold => Data.StoredGold;
    public int Elixir => Data.StoredElixir;
    public int GoldCapacity => Data.GoldCapacity;
    public int ElixirCapacity => Data.ElixirCapacity;
    public int CanTakeGold => GoldCapacity - Gold;
    public int CanTakeElixir => ElixirCapacity - Elixir;

    #endregion

    bool init = false;

    public static Base active;

    void Awake()
    {
        active = this;
    }

    void InitBuildingLists()
    {
        MainHall = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.MainHall).FirstOrDefault() as MainHall;
        Lab = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.MainHall).FirstOrDefault() as Lab;
        TrainingZones = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.TrainingZone).Select(b => b as TrainingZone).ToList();
        ArmyZones = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.ArmyHolder).Select(b => b as CampZone).ToList();
        Bunkers = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Bunker).Select(b => b as Bunker).ToList();

        #region Mines
        Mines = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Mine).Select(b => b as Mine).ToList();
        GoldMines = Mines.Where(m => m.Data.resourceType == BuildingData.ResourceType.Gold).ToList();
        ElixirMines = Mines.Where(m => m.Data.resourceType == BuildingData.ResourceType.Elixir).ToList();
        GemMines = Mines.Where(m => m.Data.resourceType == BuildingData.ResourceType.Gems).ToList();
        #endregion

        #region Storages
        var storages = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Storage).Select(b => b as Storage);
        GoldStorages = storages.Where(s => s.Data.storageType == StorageData.StoreType.Gold).ToList();
        ElixirStorages = storages.Where(s => s.Data.storageType == StorageData.StoreType.Elixir).ToList();
        #endregion

        Turrets = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Turret).Select(b => b as Turret).ToList();
        Walls = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Wall).Select(b => b as Wall).ToList();
        Traps = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Trap).Select(b => b as Trap).ToList();
        Decorations = Buildings.Where(b => b.BaseData.Type == BuildingData.BuildingType.Decoration).Select(b => b as Decoration).ToList();
    }

    public void Generate(BaseData data)
    {
        Data = data;
        Buildings = new List<Building>();
        foreach (var building in data.buildings) GenerateBuilding(building);
        InitBuildingLists();
        DisplayArmy();
        InitReports();
        AccountForPassedTime();
        Invoke("InitConstructionManager", 0.5f);
        init = true;
    }

    void InitConstructionManager()
    {
        ConstructionManager.instance.InitCurrentConstructions();
    }

    void AccountForPassedTime()
    {
        var span = System.DateTime.Now - Data.lastVisited;
        var passed = new GameTime(span);

        var mineNewValues = new Dictionary<int, int>();
        //var trainingZonesToUpdate = new List<TrainingZoneInstanceData>();

        foreach(var mine in Mines)
        {
            var fillTime = mine.FillTimeLeft;
            var addingTime = fillTime > passed ? passed : fillTime;
            float addProportion = addingTime.TotalHours / mine.VersionData.fillTime.TotalHours;
            int toAdd = (int)(mine.Capacity * addProportion);
            mine.InstanceData.stored += toAdd;
            mineNewValues.Add(mine.InstanceData.ID, mine.Stored);
        }

        foreach(var zone in TrainingZones)
        {
            /* they wouldn't be synced
             * hmm maybe some kind of an ordered multiset and go through it while capacity and time fit
            var time = passed;
            if (zone.Training == null) continue;
            trainingZonesToUpdate.Add(zone.InstanceData);
            var fillTime = zone.TimeLeft;
            if(time <= fillTime)
            {
                zone.AddTime(time);
                continue;
            }
            */
        }
    }

    #region Reports

    void InitReport(DefenseReport report)
    {
        var obj = Instantiate(defenseReportPrefab, reportsParent);
        var ui = obj.GetComponent<DefenseReportUI>();
        ui.Init(report);
    }

    void InitReport(AttackReport report)
    {
        var obj = Instantiate(attackReportPrefab, reportsParent);
        var ui = obj.GetComponent<AttackReportUI>();
        ui.Init(report);
    }

    void InitReports()
    {
        if (Data.defenses.Count == 0 && Data.attacks.Count == 0) return;

        reportsParent.gameObject.SetActive(true);

        foreach (var report in Data.defenses) InitReport(report);
        foreach (var report in Data.attacks) InitReport(report);

        Data.defenses.Clear();
        Data.attacks.Clear();
    }

    #endregion

    #region Army

    void DisplayArmy()
    {
        foreach(var squad in Data.army.squads)
        {
            var unit = squad.unit;
            for(int i = 0; i < squad.amount; i++)
            {
                int index = 0;
                while(ArmyZones[index].FreeRoom < unit.capacity) index++;
                ArmyZones[index].AddUnit(unit);
            }
        }
    }

    #region Unit

    public Transform AddUnit(UnitData unit)
    {
        if(unit == null)
        {
            Debug.Log("No unit passed!");
            return null;
        }
        Transform added = null;
        int index = 0;
        while (index < ArmyZones.Count && ArmyZones[index].FreeRoom < unit.capacity) index++;
        if (index < ArmyZones.Count) added = ArmyZones[index].AddUnit(unit);
        else
        {
            Debug.Log("Could Not Add Unit!");
            return null;
        }
        ClientSend.AddUnit(unit.name, Data.ID);
        var squad = Data.army.squads.Where(s => s.unit == unit).FirstOrDefault();
        if (squad == null) Data.army.squads.Add(new ArmySquad(unit));
        else squad.amount++;

        return added;
    }

    public bool RemoveUnit(UnitData unit)
    {
        if (unit == null)
        {
            Debug.Log("No unit passed!");
            return false;
        }



        return true;
    }

    #endregion

    public PlayerAttackArmy GetArmy()
    {
        var squads = new List<ArmySquad>();
        foreach (var squad in Data.army.squads) squads.Add(new ArmySquad(squad));

        var army = new PlayerAttackArmy { squads = squads };

        return army;
    }

    #endregion

    #region Building

    public int GetBuildingCount(BuildingData data)
    {
        int builtCount = Buildings.Where(b => b.BaseData == data).Count();
        int constructingCount = ConstructionManager.instance.GetBeingBuiltCount(data);

        return builtCount + constructingCount;
    }

    BuildingInstanceData GetInstanceFromData(BuildingData data, int level, int x, int y)
    {
        switch (data.Type)
        {
            case BuildingData.BuildingType.ArmyHolder:
                return new ArmyHolderInstanceData(data as ArmyHoldData, level, x, y);
            case BuildingData.BuildingType.Bunker:
                return new BunkerInstanceData(data as BunkerData, level, x, y);
            case BuildingData.BuildingType.Lab:
                return new LabInstanceData(data as LabData, level, x, y);
            case BuildingData.BuildingType.MainHall:
                return new MainHallInstanceData(data as MainHallData, level, x, y);
            case BuildingData.BuildingType.Mine:
                return new MineInstanceData(data as MineData, level, x, y);
            case BuildingData.BuildingType.Storage:
                return new StorageInstanceData(data as StorageData, level, x, y);
            case BuildingData.BuildingType.TrainingZone:
                return new TrainingZoneInstanceData(data as TrainingZoneData, level, x, y);
            case BuildingData.BuildingType.Trap:
                return new TrapInstanceData(data as TrapData, level, x, y);
            case BuildingData.BuildingType.Turret:
                return new TurretInstanceData(data as DefensiveData, level, x, y);
            case BuildingData.BuildingType.Wall:
                return new WallInstanceData(data as WallData, level, x, y);
        }

        return null;
    }

    public BuildingInstanceData AddBuilding(BuildingData data, int level, int x, int y)
    {
        var instance = GetInstanceFromData(data, level, x, y);
        Data.AddBuilding(instance);
        var building = GenerateBuilding(instance);

        switch (data.Type)
        {
            case BuildingData.BuildingType.ArmyHolder: ArmyZones.Add(building as CampZone); break;
            case BuildingData.BuildingType.Bunker: Bunkers.Add(building as Bunker); break;
            case BuildingData.BuildingType.Lab: Lab = building as Lab; break;
            case BuildingData.BuildingType.MainHall: MainHall = building as MainHall; break;

            case BuildingData.BuildingType.Mine:
                var mine = building as Mine;
                Mines.Add(mine);
                switch (mine.Data.resourceType)
                {
                    case BuildingData.ResourceType.Gold: GoldMines.Add(mine); break;
                    case BuildingData.ResourceType.Elixir: ElixirMines.Add(mine); break;
                    case BuildingData.ResourceType.Gems: GemMines.Add(mine); break;
                }
                break;

            case BuildingData.BuildingType.Storage:
                var storage = building as Storage;
                //Storages.Add(storage);
                switch (storage.Data.storageType)
                {
                    case StorageData.StoreType.Gold: GoldStorages.Add(storage); break;
                    case StorageData.StoreType.Elixir: ElixirStorages.Add(storage); break;
                }
                break;

            case BuildingData.BuildingType.TrainingZone: TrainingZones.Add(building as TrainingZone); break;
            case BuildingData.BuildingType.Turret: Turrets.Add(building as Turret); break;
            case BuildingData.BuildingType.Wall: Walls.Add(building as Wall); break;
            case BuildingData.BuildingType.Trap: Traps.Add(building as Trap); break;
            case BuildingData.BuildingType.Decoration: Decorations.Add(building as Decoration); break;
        }

        return instance;
    }

    public void UpgradeBuilding(ConstructionData con)
    {
        var building = Buildings.Where(b => b.BaseInstanceData == con.isUpgradeOf).FirstOrDefault();
        building.Upgrade();
    }

    public Building GenerateBuilding(BuildingInstanceData data)
    {
        float x = data.tileX * tileDistance;
        float y = 0;
        float z = data.tileY * tileDistance;
        Vector3 pos = transform.position + new Vector3(x, y, z);
        Quaternion rotation = Quaternion.identity; //may do rotatable at least by 90 degrees

        var obj = Instantiate(data.BaseCurrentData.prefab, pos, rotation);
        obj.name = data.FullName;
        obj.transform.parent = transform;

        var building = obj.GetComponent<Building>();

        building.ApplyInstanceData(this, data);

        Buildings.Add(building);

        return building;
    }

    #endregion

    #region Upgrade

    public void UpgradeLab(Lab newLab)
    {
        Lab = newLab;
    }

    public void UpgradeMainHaill(MainHall newHall)
    {
        MainHall = newHall;
    }

    #endregion

    #region Gizmos

    void OnDrawGizmosSelected()
    {
        DrawTileGrid();
    }

    void DrawTileGrid()
    {
        if(!init)
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < BaseData.Width; x++)
            {
                for(int z = 0; z < BaseData.Height; z++)
                {
                    float X = (x + 0.5f) * tileDistance;
                    float Y = 0.1f;
                    float Z = (z + 0.5f) * tileDistance;
                    Vector3 center = new Vector3(X, Y, Z) + transform.position;
                    Vector3 size = new Vector3(tileDistance, Y, tileDistance);
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
        else
        {
            for (int x = 0; x < BaseData.Width; x++)
            {
                for (int z = 0; z < BaseData.Height; z++)
                {
                    bool free = Data[x, z] == null;
                    Gizmos.color = free ? Color.green : Color.red;
                    float X = (x + 0.5f) * tileDistance;
                    float Y = 0.1f * (free ? 1 : 2);
                    float Z = (z + 0.5f) * tileDistance;
                    Vector3 center = new Vector3(X, Y, Z) + transform.position;
                    Vector3 size = new Vector3(tileDistance, Y, tileDistance);
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }

    #endregion
}