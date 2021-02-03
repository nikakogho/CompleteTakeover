using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CT.Data;
using CT.Manager;
using CT.Data.Instance;
using CT.Helper;

namespace CT.Net
{
    public enum ServerPackets
    {
        welcomeFromServer = 1, registerConfirm, loadPlayer, buildingAdded,
        //,processAttack, processDefense,
        giveFewRandomBases, giveAttackableBaseData,
        updateResources, updateGems,
        giveUpdatedPlayerData
        //,givePreloadUpdate
    }
    public enum ClientPackets
    {
        welcomeReceived = 1, registerCheck, loadPlayerRequest, addBuilding, upgradeBuilding,
        buildingFixed,
        reportAttack, reportDefense, defenderProcessedCombat,
        requestFewRandomBases, requestAttackableBaseData,
        addUnit, removeUnit, addResources, subtractResources, addGems, subtractGems,
        moveBuilding, updateMineStored, updateStorageValue,
        leavingBase, disconnecting, addBuilder,
        requestPlayerDataUpdate
        //,requestPreloadUpdate
    }

    public class Packet : IDisposable
    {
        List<byte> buffer;
        byte[] readableBuffer;
        int readPos;

        public Packet()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        public Packet(int id)
        : this()
        {
            Write(id);
        }

        public Packet(byte[] data)
        : this()
        {
            SetBytes(data);
        }

        #region Functions

        public void SetBytes(byte[] data)
        {
            Write(data);
            readableBuffer = buffer.ToArray();
        }

        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }

        public void InsertInt(int value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        public byte[] ToArray()
        {
            return readableBuffer = buffer.ToArray();
        }

        public int Length()
        {
            return buffer.Count;
        }

        public int UnreadLength()
        {
            return Length() - readPos;
        }

        public void Reset(bool should = true)
        {
            if (should)
            {
                buffer.Clear();
                readableBuffer = null;
                readPos = 0;
            }
            else readPos = -4;
        }

        #endregion

        #region Write Data

        #region Conventional

        public void Write(byte value)
        {
            buffer.Add(value);
        }

        public void Write(byte[] values)
        {
            buffer.AddRange(values);
        }

        public void Write(short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value == null);
            if (value == null) return;
            Write(value.Length);
            var bytes = Encoding.ASCII.GetBytes(value);
            Write(bytes);
        }

        #endregion

        #region Special

        public void Write(KeyValuePair<int, int> value)
        {
            Write(value.Key);
            Write(value.Value);
        }

        public void Write(DateTime value)
        {
            Write(value.Year);
            Write(value.Month);
            Write(value.Day);
            Write(value.Hour);
            Write(value.Minute);
            Write(value.Second);
        }

        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(GameTime value)
        {
            Write(value.hours);
            Write(value.minutes);
            Write(value.seconds);
        }

        public void Write(BuildingInstanceData value)
        {
            bool empty = value == null;
            Write(empty);
            if (empty) return;
            Write(value.ID);
            //Write(value.Base.ID); //for checking
            Write(value.data.name);
            Write(value.level);
            Write(value.tileX);
            Write(value.tileY);
            Write(value.destroyed);

            string type = value.data.Type.ToName();

            Write(type);
            switch (value.data.Type)
            {
                case BuildingData.BuildingType.ArmyHolder:
                    break;

                case BuildingData.BuildingType.Bunker:
                    //var bunker = value as BunkerInstanceData;
                    //Write(bunker.squads.Count);
                    //foreach (var unit in bunker.squads) Write(unit);
                    break;

                case BuildingData.BuildingType.Lab:
                    var lab = value as LabInstanceData;
                    Write(lab.workingOnUnit);
                    Write(lab.timeLeft);
                    break;

                case BuildingData.BuildingType.MainHall:
                    var hall = value as MainHallInstanceData;
                    Write(hall.storedGold);
                    Write(hall.storedElixir);
                    Write(hall.CurrentData.goldCapacity);
                    Write(hall.CurrentData.elixirCapacity);
                    break;

                case BuildingData.BuildingType.Mine:
                    var mine = value as MineInstanceData;
                    Write(mine.stored);
                    Write(mine.Data.resourceType == BuildingData.ResourceType.Gold);
                    break;

                case BuildingData.BuildingType.Storage:
                    var storage = value as StorageInstanceData;
                    Write(storage.stored);
                    Write(storage.Data.storageType == StorageData.StoreType.Gold);
                    Write(storage.CurrentData.capacity);
                    break;

                case BuildingData.BuildingType.TrainingZone:
                    //var zone = value as TrainingZoneInstanceData;
                    //Write(zone.trainingUnitName);
                    //Write(zone.timeLeft);
                    //Write(zone.slots.Count);
                    //foreach (var slot in zone.slots) Write(slot);
                    break;

                case BuildingData.BuildingType.Turret:
                    break;

                case BuildingData.BuildingType.Wall:
                    break;

                case BuildingData.BuildingType.Trap:
                    break;
            }
        }

        #endregion

        #endregion

        #region Read Data

        #region Conventional

        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte value = readableBuffer[readPos];
                if (moveReadPos) readPos++;
                return value;
            }
            else throw new Exception("Could not read value of type 'byte'!");
        }

        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, length).ToArray();
                if (moveReadPos) readPos += length;
                return value;
            }
            else throw new Exception("Could not read value of type 'byte[]'!");
        }

        public short ReadShort(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short value = BitConverter.ToInt16(readableBuffer, readPos);
                if (moveReadPos) readPos += 2; //short is 2 bytes
                return value;
            }
            else throw new Exception("Could not read value of type 'short'!");
        }

        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int value = BitConverter.ToInt32(readableBuffer, readPos);
                if (moveReadPos) readPos += 4; //int is 4 bytes
                return value;
            }
            else throw new Exception("Could not read value of type 'int'!");
        }

        public long ReadLong(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long value = BitConverter.ToInt64(readableBuffer, readPos);
                if (moveReadPos) readPos += 8;
                return value;
            }
            else throw new Exception("Could not read value of type 'long'!");
        }

        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float value = BitConverter.ToSingle(readableBuffer, readPos);
                if (moveReadPos) readPos += 4;
                return value;
            }
            else throw new Exception("Could not read value of type 'float'!");
        }

        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(readableBuffer, readPos);
                if (moveReadPos) readPos++;
                return value;
            }
            else throw new Exception("Could not read value of type 'bool'!");
        }

        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                bool empty = ReadBool();
                if (empty) return null;
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);
                if (moveReadPos && value.Length > 0) readPos += length;
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        #endregion

        #region Special

        public DateTime ReadDateTime()
        {
            int year = ReadInt();
            int month = ReadInt();
            int day = ReadInt();
            int hour = ReadInt();
            int minute = ReadInt();
            int second = ReadInt();
            return new DateTime(year, month, day, hour, minute, second);
        }

        public GameTime ReadGameTime()
        {
            return new GameTime(ReadInt(), ReadInt(), ReadInt());
        }

        public ConstructionData ReadConstructionData(List<BuildingInstanceData> buildings)
        {
            int id = ReadInt();
            //int baseID = ReadInt(); may do add at some point

            var time = ReadGameTime();
            int x = ReadInt();
            int y = ReadInt();

            bool isUpgrade = ReadBool();

            if (isUpgrade)
            {
                int toUpgradeID = ReadInt();
                var toUpgrade = buildings.Where(b => b.ID == toUpgradeID).First();
                return new ConstructionData(toUpgrade, time);
            }

            string buildingName = ReadString();

            var building = LoadManager.buildingsDictionary[buildingName];
            //var buildingOn = buildings.Where(b => b.ID == buildingOnID).FirstOrDefault();

            return new ConstructionData(id, building, time, x, y);
        }

        public BuildingInstanceData ReadBuildingInstance() 
        {
            bool empty = ReadBool();
            if (empty) return null;
            int id = ReadInt();
            //int baseID = ReadInt(); // may do use at some point
            string dataName = ReadString();

            int level = ReadInt();

            int tileX = ReadInt();
            int tileY = ReadInt();
            bool destroyed = ReadBool();

            string type = ReadString();

            var data = LoadManager.buildingsDictionary[dataName];

            switch (type)
            {
                case "Army Holder":
                    var armyHolderData = data as ArmyHoldData;
                    return new ArmyHolderInstanceData(id, armyHolderData, level, tileX, tileY, destroyed);

                case "Bunker":
                    var bunkerData = data as BunkerData;
                    int squadsCount = ReadInt();
                    var squads = new List<ArmySquad>();
                    for (int i = 0; i < squadsCount; i++)
                        squads.Add(ReadSquad());
                    return new BunkerInstanceData(id, bunkerData, level, squads, tileX, tileY, destroyed);

                case "Lab":
                    var lab = data as LabData;
                    string unit = ReadString();
                    GameTime timeLeft = ReadGameTime();
                    return new LabInstanceData(id, lab, level, unit, timeLeft, tileX, tileY, destroyed);

                case "Main Hall":
                    var hall = data as MainHallData;
                    int gold = ReadInt();
                    int elixir = ReadInt();
                    return new MainHallInstanceData(id, hall, level, gold, elixir, tileX, tileY, destroyed);

                case "Mine":
                    var mine = data as MineData;
                    int stored = ReadInt();
                    return new MineInstanceData(id, mine, level, stored, tileX, tileY, destroyed);
                
                case "Storage":
                    var storage = data as StorageData;
                    stored = ReadInt();
                    return new StorageInstanceData(id, storage, level, stored, tileX, tileY, destroyed);

                case "Training Zone":
                    var zone = data as TrainingZoneData;
                    string trainingUnit = ReadString();
                    GameTime trainTimeLeft = ReadGameTime();
                    int slotsCount = ReadInt();
                    var slots = new List<TrainingZoneInstanceData.TrainSlot>();
                    for (int i = 0; i < slotsCount; i++)
                        slots.Add(new TrainingZoneInstanceData.TrainSlot(ReadSquad()));
                    return new TrainingZoneInstanceData(id, zone, level, trainingUnit, trainTimeLeft, slots, tileX, tileY, destroyed);

                case "Turret":
                    var turret = data as DefensiveData;
                    return new TurretInstanceData(id, turret, level, tileX, tileY, destroyed);

                case "Trap":
                    var trap = data as TrapData;
                    return new TrapInstanceData(id, trap, level, tileX, tileY, destroyed);

                case "Wall":
                    var wall = data as WallData;
                    return new WallInstanceData(id, wall, level, tileX, tileY, destroyed);

                case "Decoration":
                    var deco = data as DecorationData;
                    return new DecorationInstanceData(id, deco, level, tileX, tileY, destroyed);
            }

            return null;
        }

        public ArmySquad ReadSquad()
        {
            int id = ReadInt();
            string name = ReadString();
            int amount = ReadInt();
            return new ArmySquad(id, name, amount);
        }

        public ArmyData ReadArmy()
        {
            int squadCount = ReadInt();
            var squads = new List<ArmySquad>();
            for (int i = 0; i < squadCount; i++) squads.Add(ReadSquad());
            return new ArmyData(squads);
        }

        public DefenseReport ReadDefenseReport(BaseData _base)
        {
            int id = ReadInt();
            string invasionName = ReadString();

            int lostGold = ReadInt();
            int lostElixir = ReadInt();

            int armyCount = ReadInt();
            var army = new List<ArmySquad>();
            for (int i = 0; i < armyCount; i++) army.Add(new ArmySquad(ReadString(), ReadInt()));

            return new DefenseReport(id, _base, invasionName, lostGold, lostElixir, army);
        }

        public AttackReport ReadAttackReport(BaseData _base)
        {
            int id = ReadInt();
            string attackerUsername = ReadString();
            int takenGold = ReadInt();
            int takenElixir = ReadInt();

            int armyCount = ReadInt();
            var army = new List<ArmySquad>();
            for (int i = 0; i < armyCount; i++) army.Add(new ArmySquad(ReadString(), ReadInt()));

            return new AttackReport(id, _base, attackerUsername, takenGold, takenElixir, army);
        }

        public BaseData ReadBase()
        {
            bool empty = ReadBool();
            if (empty) return null;
            int id = ReadInt();
            //string username = ReadString(); //may do for checking
            bool isHome = ReadBool();

            DateTime lastVisited = ReadDateTime();

            var army = ReadArmy();

            int buildingsCount = ReadInt();
            var buildings = new List<BuildingInstanceData>();
            for (int i = 0; i < buildingsCount; i++) buildings.Add(ReadBuildingInstance());

            int builders = ReadInt();

            int constructionCount = ReadInt();
            var constructions = new List<ConstructionData>();
            for (int i = 0; i < constructionCount; i++) constructions.Add(ReadConstructionData(buildings));

            var data = new BaseData(id, lastVisited, army, buildings, builders, constructions, isHome);

            int defenseCount = ReadInt();
            for (int i = 0; i < defenseCount; i++) data.defenses.Add(ReadDefenseReport(data));

            int attackCount = ReadInt();
            for (int i = 0; i < attackCount; i++) data.attacks.Add(ReadAttackReport(data));

            return data;
        }

        public PlayerData ReadPlayerData()
        {
            bool empty = ReadBool();
            if (empty) return null;
            //Debug.Log("Player past the empty!");
            string username = ReadString();
            int pasLen = ReadInt();
            byte[] password = ReadBytes(pasLen);
            Sprite icon = LoadManager.iconsDictionary[ReadString()];
            Faction.Type faction = (Faction.Type)Enum.Parse(typeof(Faction.Type), ReadString());

            int gems = ReadInt();
            DateTime lastOnline = ReadDateTime();
            int coloniesCount = ReadInt();
            BaseData homeBase = null;
            List<BaseData> colonies = new List<BaseData>(coloniesCount);
            for (int i = 0; i < coloniesCount; i++)
            {
                var baseData = ReadBase();
                if (baseData.IsHome) homeBase = baseData;
                else colonies.Add(baseData);
            }
            //Debug.Log("Player almost retrieved!");
            var data = new PlayerData(username, password, faction, icon, gems, lastOnline, homeBase, colonies);
            if (data == null) Debug.Log("Something went wrong");
            //else Debug.Log("Player created!");
            return data;
        }

        #endregion

        #endregion

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}