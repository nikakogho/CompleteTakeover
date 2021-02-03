using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Manager
{
    public class CombatSpeedManager : MonoBehaviour
    {
        public GameObject _1xUI, _2xUI, _4xUI;

        public static CombatSpeedManager instance;

        void Awake()
        {
            instance = this;
        }

        void Set(int index)
        {
            bool[] values = new bool[3];
            values[0] = values[1] = values[2] = false;
            if(index >= 0) values[index] = true;
            _1xUI.SetActive(values[0]);
            _2xUI.SetActive(values[1]);
            _4xUI.SetActive(values[2]);
        }

        public void SetToZero()
        {
            Time.timeScale = 0;
        }

        public void SetToOne()
        {
            Time.timeScale = 1;
            Set(0);
        }

        public void SetToTwo()
        {
            Time.timeScale = 2;
            Set(1);
        }

        public void SetToFour()
        {
            Time.timeScale = 4;
            Set(2);
        }

        public void OnBattleOver()
        {
            SetToZero();
            gameObject.SetActive(false);
        }
    }
}
