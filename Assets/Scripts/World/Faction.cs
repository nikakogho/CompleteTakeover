using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Manager;

public class Faction : Object
{                    //     0             1            2         3       4      5       6
    public enum Type { AncientHuman, ModernHuman, FutureHuman, Zombie, Robot, Alien, Mystical }

    #region Static

    public static Faction[] Factions { get; private set; }

    public static void InitFactions(LoadManager.FactionStat[] stats)
    {
        Factions = new Faction[System.Enum.GetNames(typeof(Type)).Length];
        for (int i = 0; i < Factions.Length; i++) Factions[i] = new Faction(stats[i]);
    }

    public static Faction GetFaction(Type type)
    {
        return Factions[(int)type];
    }

    #endregion

    #region Stats
    
    public Type type;
    public string elixirName;
    public Sprite elixirIcon;
    public Sprite icon;
    public Sprite playerDefaultIcon;
    public GameObject environmentPrefab;
    public BuildingData mainHallData;
    public BuildingData[] buildings;
    public UnitData[] units;

    #endregion

    public Faction(LoadManager.FactionStat stat)
    : this(stat.type, stat.elixirName, stat.elixirIcon, stat.factionIcon, stat.playerDefaultIcon, stat.environmentPrefab, stat.mainHallData, stat.buildings, stat.units)
    {

    }

    Faction(Type type, string elixirName, Sprite elixirIcon, Sprite icon, Sprite playerDefaultIcon,GameObject environmentPrefab, BuildingData mainHallData, BuildingData[] buildings, UnitData[] units)
    {
        this.type = type;
        this.elixirName = elixirName;
        this.elixirIcon = elixirIcon;
        this.icon = icon;
        this.playerDefaultIcon = playerDefaultIcon;
        this.environmentPrefab = environmentPrefab;
        this.mainHallData = mainHallData;
        this.buildings = buildings;
        this.units = units;
    }
}
