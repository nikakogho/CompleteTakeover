using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CT.Data;
using CT.Manager.Combat;
using CT.Net;

namespace CT.Manager
{
    public class AttackManager : MonoBehaviour
    {
        public Base _base;

        public string attackSceneName = "attack";

        public static AttackManager instance;

        void Awake()
        {
            instance = this;
        }

        public void FindRandomTarget()
        {
            ClientSend.RequestFewRandomBases(5, GameManager.Player.username);
        }

        public void ReceiveFewPossibleTargets(int[] ids)
        {
            if (ids.Length == 0)
            {
                Debug.Log("Target not found!");
                //to do some kind of UI
                return;
            }
            int id = ids[Random.Range(0, ids.Length)];
            ClientSend.RequestAttackableBaseData(id);
        }

        public void ReceiveAttackableBaseData(BaseData data)
        {
            var army = _base.GetArmy();
            PlayerCombatManager.Init(data, _base.Data, army, false);
            SceneManager.LoadScene(attackSceneName);
        }
    }
}