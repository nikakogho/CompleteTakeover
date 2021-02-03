using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Linq;
using CT.Data;
using CT.Manager;
using CT.UI;

namespace CT.Net
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet packet)
        {
            string message = packet.ReadString();
            int id = packet.ReadInt();

            //Debug.Log($"Message from server: {message}");
            Client.instance.id = id;
            ClientSend.WelcomeReceived();

            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

            var text = LoadManager.instance.connectedText;

            text.text = message;
            text.color = Color.green;
        }

        public static void RegisterConfirm(Packet packet)
        {
            bool successful = packet.ReadBool();

            if (successful) LoadManager.instance.LoadGameFromRegistration();
            else LoadManager.instance.FailedToRegister();
        }

        public static void LoadPlayer(Packet packet)
        {
            var player = packet.ReadPlayerData();
            //Debug.Log("Is player null: " + (player == null));
            if (player == null) LoadManager.instance.FailedToLoad();
            else LoadManager.instance.LoadPlayer(player);
        }
        
        public static void ReceiveUpdatedPlayerData(Packet packet)
        {
            var player = packet.ReadPlayerData();
            if (player == null) Debug.LogError("Player not found!");
            else LoadManager.instance.LoadPlayer(player);
        }

        public static void BuildingAdded(Packet packet)
        {
            int oldID = packet.ReadInt();
            int newID = packet.ReadInt();
            GameManager.BaseData.buildings.Where(b => b.ID == oldID).First().ID = newID;
            //Debug.Log($"Building with ID {oldID} became building with ID {newID}");
        }

        public static void GetFewRandomTargetBases(Packet packet)
        {
            int count = packet.ReadInt();
            int[] ids = new int[count];
            for (int i = 0; i < count; i++) ids[i] = packet.ReadInt();

            AttackManager.instance.ReceiveFewPossibleTargets(ids);
        }

        public static void ReceiveAttackableBaseData(Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();
            string faction = packet.ReadString();
            bool home = packet.ReadBool();

            //int gold = packet.ReadInt();   //may do something with it
            //int elixir = packet.ReadInt(); //may do something with it

            int buildingCount = packet.ReadInt();
            var buildings = new List<BuildingInstanceData>();
            for (int i = 0; i < buildingCount; i++) buildings.Add(packet.ReadBuildingInstance());

            //int conCount = packet.ReadInt();
            //var constructions = new List<ConstructionData>();
            //for (int i = 0; i < conCount; i++) constructions.Add(packet.ReadConstructionData(buildings));

            var data = new BaseData(id, username, faction, buildings, 0, new List<ConstructionData>(), home);

            AttackManager.instance.ReceiveAttackableBaseData(data);
        }

        public static void UpdateResources(Packet packet)
        {
            int baseID = packet.ReadInt();
            int hallGold = packet.ReadInt();
            int hallElixir = packet.ReadInt();
            
            if(GameManager.BaseData.ID != baseID)
            {
                Debug.LogError("Base ID mismatch!");
                return;
            }

            var _base = GameManager.instance._base;

            _base.MainHall.SetResources(hallGold, hallElixir);

            int minesCount = packet.ReadInt();

            for (int i = 0; i < minesCount; i++)
            {
                int id = packet.ReadInt();
                int value = packet.ReadInt();
                _base.Data.Mines.Where(m => m.ID == id).First().stored = value;
            }

            int storagesCount = packet.ReadInt();

            for (int i = 0; i < storagesCount; i++)
            {
                int id = packet.ReadInt();
                int value = packet.ReadInt();
                _base.Data.Storages.Where(s => s.ID == id).First().stored = value;
            }

            BaseResourcesUI.instance.UpdateValues();
        }

        public static void UpdateGems(Packet packet)
        {
            int gems = packet.ReadInt();

            GameManager.Player.gems = gems;

            BaseResourcesUI.instance.UpdateValues();
        }
    }
}