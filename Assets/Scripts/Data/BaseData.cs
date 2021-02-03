using CT.Data.Instance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace CT.Data
{
    public class BaseData
    {
        public readonly int ID;
        public PlayerData Player { get; private set; }
        public string PlayerUsername { get; private set; }
        public bool IsHome { get; private set; }
        public bool IsColony => !IsHome;
        public ArmyData army;
        public List<BuildingInstanceData> buildings;
        public List<DefenseReport> defenses;
        public List<AttackReport> attacks;

        public int builders;
        public List<ConstructionData> constructions;

        public Faction Faction { get; private set; }

        public DateTime lastVisited;

        public BuildingInstanceData this[int x, int y]
        {
            get
            {
                foreach (var building in buildings)
                    if (Mathf.Clamp(x, building.tileX, building.tileX + building.data.tileWidth - 1) == x &&
                        Mathf.Clamp(y, building.tileY, building.tileY + building.data.tileHeight - 1) == y)
                        return building;
                return null;
            }
        }

        public const float TileDistance = 5;
        public const int Width = 60;  //[-100 200]
        public const int Height = 50; //[-100 150]

        #region Buildings

        public MainHallInstanceData MainHall { get; private set; }
        public LabInstanceData Lab { get; private set; }

        public List<ArmyHolderInstanceData> ArmyHolders { get; private set; }
        public List<BunkerInstanceData> Bunkers { get; private set; }

        public List<MineInstanceData> Mines { get; private set; }

        public List<MineInstanceData> GoldMines { get; private set; }
        public List<MineInstanceData> ElixirMines { get; private set; }
        public List<MineInstanceData> GemMines { get; private set; }

        public List<StorageInstanceData> Storages { get; private set; }

        public List<StorageInstanceData> GoldStorages { get; private set; }
        public List<StorageInstanceData> ElixirStorages { get; private set; }

        public List<TrainingZoneInstanceData> TrainingZones { get; private set; }
        public List<TurretInstanceData> Turrets { get; private set; }
        public List<WallInstanceData> Walls { get; private set; }
        public List<TrapInstanceData> Traps { get; private set; }
        public List<DecorationInstanceData> Decorations { get; private set; }

        #endregion

        #region Hourly Yields

        float GetHourlyYield(BuildingData.ResourceType type)
        {
            float count = 0;
            foreach (var building in buildings)
            {
                if (building.destroyed) continue;
                if (building.data.Type != BuildingData.BuildingType.Mine) continue;
                var data = building.data as MineData;
                var mine = building as MineInstanceData;
                if (data.resourceType != type) continue;
                count += mine.CurrentData.HourlyYield;
            }
            return count;
        }

        public float GoldPerHour => GetHourlyYield(BuildingData.ResourceType.Gold);
        public float ElixirPerHour => GetHourlyYield(BuildingData.ResourceType.Elixir);
        public float GemsPerHour => GetHourlyYield(BuildingData.ResourceType.Gems);

        #endregion

        #region Stored

        public int StoredArmySize
        {
            get
            {
                int total = 0;
                foreach (var data in army.squads) total += data.unit.capacity * data.amount;
                return total;
            }
        }

        public int StoredGold
        {
            get
            {
                int total = MainHall.storedGold;
                //foreach (var mine in GoldMines) total += mine.stored;
                foreach (var storage in GoldStorages) total += storage.stored;
                return total;
            }
        }

        public int StoredElixir
        {
            get
            {
                int total = MainHall.storedElixir;
                //foreach (var mine in ElixirMines) total += mine.stored;
                foreach (var storage in ElixirStorages) total += storage.stored;
                return total;
            }
        }

        #endregion

        #region Capacity

        public int ArmyCapacity
        {
            get
            {
                int total = 0;
                foreach (var zone in ArmyHolders) total += zone.CurrentData.capacity;
                return total;
            }
        }

        public int GoldCapacity
        {
            get
            {
                int total = MainHall.GoldCapacity;
                foreach (var storage in GoldStorages) total += storage.CurrentData.capacity;
                return total;
            }
        }

        public int ElixirCapacity
        {
            get
            {
                int total = MainHall.ElixirCapacity;
                foreach (var storage in ElixirStorages) total += storage.CurrentData.capacity;
                return total;
            }
        }

        #endregion

        public void AddBuilding(BuildingInstanceData building)
        {
            buildings.Add(building);
            switch (building.Type)
            {
                case BuildingInstanceData.BuildingType.ArmyHolder:
                    ArmyHolders.Add(building as ArmyHolderInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.Bunker:
                    Bunkers.Add(building as BunkerInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.Lab:
                    Lab = building as LabInstanceData;
                    break;

                case BuildingInstanceData.BuildingType.MainHall:
                    MainHall = building as MainHallInstanceData;
                    break;

                case BuildingInstanceData.BuildingType.Wall:
                    Walls.Add(building as WallInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.TrainingZone:
                    TrainingZones.Add(building as TrainingZoneInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.Trap:
                    Traps.Add(building as TrapInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.Turret:
                    Turrets.Add(building as TurretInstanceData);
                    break;

                case BuildingInstanceData.BuildingType.Mine:
                    var mine = building as MineInstanceData;
                    Mines.Add(mine);
                    switch (mine.Data.resourceType)
                    {
                        case BuildingData.ResourceType.Gold: GoldMines.Add(mine); break;
                        case BuildingData.ResourceType.Elixir: ElixirMines.Add(mine); break;
                        case BuildingData.ResourceType.Gems: GemMines.Add(mine); break;
                    }
                    break;

                case BuildingInstanceData.BuildingType.Storage:
                    var storage = building as StorageInstanceData;
                    Storages.Add(storage);
                    switch (storage.Data.storageType)
                    {
                        case StorageData.StoreType.Gold: GoldStorages.Add(storage); break;
                        case StorageData.StoreType.Elixir: ElixirStorages.Add(storage); break;
                    }
                    break;

                case BuildingInstanceData.BuildingType.Decoration:
                    Decorations.Add(building as DecorationInstanceData);
                    break;
            }
        }

        public BaseData(int id, DateTime lastVisited, ArmyData army, List<BuildingInstanceData> buildings, 
            int builders, List<ConstructionData> constructions, bool isHome = false)
        {
            ID = id;
            this.lastVisited = lastVisited;
            this.army = army;
            this.buildings = new List<BuildingInstanceData>();

            #region Buildings

            #region Init Lists
            ArmyHolders = new List<ArmyHolderInstanceData>();
            Bunkers = new List<BunkerInstanceData>();

            Mines = new List<MineInstanceData>();

            GoldMines = new List<MineInstanceData>();
            ElixirMines = new List<MineInstanceData>();
            GemMines = new List<MineInstanceData>();

            Storages = new List<StorageInstanceData>();

            GoldStorages = new List<StorageInstanceData>();
            ElixirStorages = new List<StorageInstanceData>();

            TrainingZones = new List<TrainingZoneInstanceData>();
            Turrets = new List<TurretInstanceData>();
            Walls = new List<WallInstanceData>();
            Traps = new List<TrapInstanceData>();

            Decorations = new List<DecorationInstanceData>();
            #endregion

            foreach (var building in buildings) AddBuilding(building);

            #endregion

            this.builders = builders;
            this.constructions = constructions;

            IsHome = isHome;
            defenses = new List<DefenseReport>();
            attacks = new List<AttackReport>();
        }

        public BaseData(int id, string username, string factionName, List<BuildingInstanceData> buildings,
            int builders, List<ConstructionData> constructions, bool isHome)
        :this(id, DateTime.Now, new ArmyData(), buildings, builders, constructions, isHome)
        {
            PlayerUsername = username;
            Faction = Faction.GetFaction((Faction.Type)Enum.Parse(typeof(Faction.Type), factionName));
        }

        public void ApplyPlayer(PlayerData player)
        {
            if (Player != null) throw new Exception("Player already applied!");
            Player = player;
            PlayerUsername = player.username;
            Faction = player.Faction;
        }
    }
}