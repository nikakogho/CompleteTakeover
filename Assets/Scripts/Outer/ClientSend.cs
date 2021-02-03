using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Helper;
using CT.UI;
using System;

namespace CT.Net
{
    public class ClientSend : MonoBehaviour
    {
        static void SendTCPData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.tcp.SendData(packet);
        }

        static void SendUDPData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.udp.SendData(packet);
        }

        #region Packets

        public static void WelcomeReceived()
        {
            using (var packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                packet.Write(Client.instance.id);
                packet.Write("User 6 here!");

                SendTCPData(packet);
            }
        }

        #region Account

        public static void RegisterRequest(string username, string password, string faction)
        {
            using (var packet = new Packet((int)ClientPackets.registerCheck))
            {
                packet.Write(username);
                packet.Write(password);
                packet.Write(faction);

                SendTCPData(packet);
            }
        }

        public static void LoginRequest(string username, byte[] password)
        {
            using (var packet = new Packet((int)ClientPackets.loadPlayerRequest))
            {
                packet.Write(username);
                packet.Write(password.Length);
                packet.Write(password);

                SendTCPData(packet);
            }
        }

        #endregion

        public static void AddBuilding(int baseID, BuildingInstanceData building)
        {
            using (var packet = new Packet((int)ClientPackets.addBuilding))
            {
                packet.Write(baseID);
                packet.Write(building);

                SendTCPData(packet);
            }
        }

        public static void UpgradeBuilding(BuildingInstanceData building)
        {
            using (var packet = new Packet((int)ClientPackets.upgradeBuilding))
            {
                packet.Write(building.ID);
                packet.Write(building.data.Type.ToName());

                var data = building.BaseCurrentData;

                switch (building.data.Type)
                {
                    case BuildingData.BuildingType.Storage:
                        var storage = data as StorageData.VersionData;
                        packet.Write(storage.capacity);
                        break;

                    case BuildingData.BuildingType.MainHall:
                        var hall = data as MainHallData.VersionData;
                        packet.Write(hall.goldCapacity);
                        packet.Write(hall.elixirCapacity);
                        break;

                    //may do other cases
                }

                SendTCPData(packet);
            }
        }

        #region Combat Process

        public static void ProcessAttack(int defenderBaseID, int attackerBaseID, string attackerUsername, bool wasRevenge, int takenGold, 
            int takenElixir, IEnumerable<ArmySquad> deployedArmy, IEnumerable<int> destroyedBuildingIDs,
            Dictionary<int, int> robbedMines, Dictionary<int, int> robbedStorages)
        {
            using (var packet = new Packet((int)ClientPackets.reportAttack))
            {
                packet.Write(attackerBaseID);
                packet.Write(defenderBaseID);
                packet.Write(attackerUsername);
                packet.Write(wasRevenge);
                packet.Write(takenGold);
                packet.Write(takenElixir);

                packet.Write(deployedArmy.Count());
                foreach (var squad in deployedArmy)
                {
                    packet.Write(squad.name);
                    packet.Write(squad.amount);
                }

                packet.Write(destroyedBuildingIDs.Count());
                foreach (int id in destroyedBuildingIDs) packet.Write(id);

                packet.Write(robbedMines.Count);
                foreach (var mine in robbedMines) packet.Write(mine);

                packet.Write(robbedStorages.Count);
                foreach (var storage in robbedStorages) packet.Write(storage);

                SendTCPData(packet);
            }
        }

        public static void RequestPlayerDataUpdate(string username)
        {
            using(var packet = new Packet((int)ClientPackets.requestPlayerDataUpdate))
            {
                packet.Write(username);
                SendTCPData(packet);
            }
        }

        public static void ProcessDefense(int baseID, string invasionName, int amassedGold, int amassedElixir,
            IEnumerable<ArmySquad> deployedArmy, IEnumerable<int> destroyedBuildingIDs,
            Dictionary<int, int> robbedMines, Dictionary<int, int> robbedStorages)
                     //id   lost
        {
            using (var packet = new Packet((int)ClientPackets.reportDefense))
            {
                packet.Write(baseID);
                packet.Write(invasionName);
                packet.Write(amassedGold);
                packet.Write(amassedElixir);

                packet.Write(deployedArmy.Count());
                foreach (var squad in deployedArmy)
                {
                    packet.Write(squad.name);
                    packet.Write(squad.amount);
                }

                packet.Write(destroyedBuildingIDs.Count());
                foreach (int id in destroyedBuildingIDs) packet.Write(id);

                packet.Write(robbedMines.Count);
                foreach (var mine in robbedMines) packet.Write(mine);

                packet.Write(robbedStorages.Count);
                foreach (var storage in robbedStorages) packet.Write(storage);

                SendTCPData(packet);
            }
        }

        public static void ConfirmDefenderProcessed(bool defenseFromAI, int id)
        {
            using (var packet = new Packet((int)ClientPackets.defenderProcessedCombat))
            {
                packet.Write(defenseFromAI);
                packet.Write(id);

                SendTCPData(packet);
            }
        }

        #endregion

        public static void BuildingFixed(int id)
        {
            using (var packet = new Packet((int)ClientPackets.buildingFixed))
            {
                packet.Write(id);

                SendTCPData(packet);
            }
        }

        #region Attack

        public static void RequestFewRandomBases(int count, string username)
        {
            using(var packet = new Packet((int)ClientPackets.requestFewRandomBases))
            {
                packet.Write(count);
                packet.Write(username);

                SendTCPData(packet);
            }
        }

        public static void RequestAttackableBaseData(int id)
        {
            using (var packet = new Packet((int)ClientPackets.requestAttackableBaseData))
            {
                packet.Write(id);

                SendTCPData(packet);
            }
        }

        #endregion

        #region Units

        public static void AddUnit(string name, int baseID)
        {
            using (var packet = new Packet((int)ClientPackets.addUnit))
            {
                packet.Write(name);
                packet.Write(baseID);

                SendTCPData(packet);
            }
        }

        public static void RemoveUnit(string name, int baseID)
        {
            using (var packet = new Packet((int)ClientPackets.removeUnit))
            {
                packet.Write(name);
                packet.Write(baseID);

                SendTCPData(packet);
            }
        }

        #endregion

        #region Resources

        public static void AddResources(int baseID, int gold, int elixir)
        {
            BaseResourcesUI.instance.UpdateValues();

            using (var packet = new Packet((int)ClientPackets.addResources))
            {
                packet.Write(baseID);
                packet.Write(gold);
                packet.Write(elixir);

                SendTCPData(packet);
            }
        }

        public static void SubtractResources(int baseID, int gold, int elixir)
        {
            BaseResourcesUI.instance.UpdateValues();

            using (var packet = new Packet((int)ClientPackets.subtractResources))
            {
                packet.Write(baseID);
                packet.Write(gold);
                packet.Write(elixir);

                SendTCPData(packet);
            }
        }

        public static void AddGems(string username, int gems)
        {
            BaseResourcesUI.instance.UpdateValues();

            using (var packet = new Packet((int)ClientPackets.addGems))
            {
                packet.Write(username);
                packet.Write(gems);

                SendTCPData(packet);
            }
        }

        public static void SubtractGems(string username, int gems)
        {
            BaseResourcesUI.instance.UpdateValues();

            using (var packet = new Packet((int)ClientPackets.subtractGems))
            {
                packet.Write(username);
                packet.Write(gems);

                SendTCPData(packet);
            }
        }

        #endregion

        public static void MoveBuilding(int id, int x, int y)
        {
            using (var packet = new Packet((int)ClientPackets.moveBuilding))
            {
                packet.Write(id);
                packet.Write(x);
                packet.Write(y);

                SendTCPData(packet);
            }
        }

        #region Storage

        public static void UpdateMineStored(int id, int value)
        {
            using(var packet = new Packet((int)ClientPackets.updateMineStored))
            {
                packet.Write(id);
                packet.Write(value);

                SendTCPData(packet);
            }
        }

        public static void UpdateStorageValue(int id, int value)
        {
            using (var packet = new Packet((int)ClientPackets.updateStorageValue))
            {
                packet.Write(id);
                packet.Write(value);

                SendTCPData(packet);
            }
        }

        #endregion

        public static void AddBuilder(int baseID)
        {
            using(var packet = new Packet((int)ClientPackets.addBuilder))
            {
                packet.Write(baseID);

                SendTCPData(packet);
            }
        }

        public static void OnDisconnect(int baseID)
        {
            using(var packet = new Packet((int)ClientPackets.disconnecting))
            {
                //to do

                SendTCPData(packet);
            }
        }

        #endregion
    }
}