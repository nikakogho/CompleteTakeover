using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Manager;

namespace CT.UI
{
    public class InvasionButton : MonoBehaviour
    {
        public Text text;

        InvasionManager manager;
        InvasionManager.Invasion invasion;

        public void Init(InvasionManager manager, InvasionManager.Invasion invasion)
        {
            this.manager = manager;
            this.invasion = invasion;
            text.text = invasion.name;
        }

        public void Invade()
        {
            manager.Launch(invasion);
        }
    }
}