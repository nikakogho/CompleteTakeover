using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI
{
    public class GameTimeTextManager : MonoBehaviour
    {
        public Text text;

        public void SetTime(GameTime time)
        {
            string seconds = string.Format("{00}", time.seconds);
            string minutes = string.Format("{00}", time.minutes);
            string hours = time.hours.ToString();
            text.text = $"{hours}:{minutes}:{seconds}";
        }
    }
}