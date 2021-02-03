using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;

namespace CT.Helper
{
    public static class HelperStatics
    {
        static string[] names =
        {
            "Army Holder", "Bunker", "Lab", "Main Hall",
            "Mine", "Storage", "Training Zone", "Turret",
            "Wall", "Trap", "Decoration"
        };

        //public enum BuildingType
        //{
        //    ArmyHolder, Bunker, Lab, MainHall, Mine, Storage,
        //    TrainingZone, Turret, Wall, Trap, Decoration
        //}

        public static string ToName(this BuildingData.BuildingType type)
        {
            return names[(int)type];
        }

        public static string ToDisplayText(this GameTime time)
        {
            string hours = time.hours.ToString("00");
            string minutes = time.minutes.ToString("00");
            string seconds = time.seconds.ToString("00");
            return $"{hours}:{minutes}:{seconds}";
        }
    }
}