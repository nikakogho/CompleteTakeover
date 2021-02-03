using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Net;
using CT.Instance;
using CT.Manager;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public abstract class Building : Attackable
{
    public BuildingData BaseData { get; private set; }
    public BuildingInstanceData BaseInstanceData { get; private set; }
    public BuildingData.BaseVersionData BaseVersion => BaseInstanceData.BaseCurrentData;
    public Transform center;
    public bool IsRepairing { get; private set; }

    public float Health { get; private set; }

    public override bool IsAlive => alive;

    public GameObject destroyedFX, fixedFX;

    public override Transform Center => center;

    bool alive = true;
    bool combatMode;

    AudioSource source;
    Collider col;

    public Base Base { get; private set; }
    public DefendingBase DefendingBase { get; private set; }

    public bool IsUpgrading => constructionManager.IsUpgrading(BaseInstanceData);

    public override Collider BoundsCollider => col;

    //DefendingBase _base;
    protected virtual int StartGold => 0;
    protected virtual int StartElixir => 0;
    protected int gold;
    protected int elixir;

    ConstructionManager constructionManager;

    protected virtual void Awake()
    {
        source = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
    }

    public virtual void ApplyInstanceData(Base _base, BuildingInstanceData instanceData)
    {
        Base = _base;
        combatMode = false;
        BaseInstanceData = instanceData;
        BaseData = instanceData.data;
        alive = !instanceData.destroyed;
        constructionManager = ConstructionManager.instance;
        if (instanceData.destroyed)
        {
            StartCoroutine(RepairRoutine());
        } 
        else Health = BaseVersion.health;
    }

    public virtual void ApplyCombatInstanceData(DefendingBase _base, BuildingInstanceData instanceData)
    {
        DefendingBase = _base;
        combatMode = true;
        BaseInstanceData = instanceData;
        BaseData = instanceData.data;
        alive = !instanceData.destroyed;
        if (instanceData.destroyed) return;
        Health = BaseVersion.health;
        gold = StartGold;
        elixir = StartElixir;
    }

    public override void TakeDamage(float damage)
    {
        if (!alive) return;

        int nDamage = (int)damage;

        int goldToTake = nDamage > gold ? gold : nDamage;
        int elixirToTake = nDamage > elixir ? elixir : nDamage;

        Health -= damage;
        if(Health <= 0)
        {
            goldToTake = gold;
            elixirToTake = elixir;
        }

        switch (BaseData.Type)
        {
            case BuildingData.BuildingType.Mine:
                DefendingBase.OnMineRobbed(transform.position, BaseInstanceData.ID, goldToTake > 0, goldToTake + elixirToTake);
                break;

            case BuildingData.BuildingType.Storage:
                DefendingBase.OnStorageRobbed(transform.position, BaseInstanceData.ID, goldToTake > 0, goldToTake + elixirToTake);
                break;

            case BuildingData.BuildingType.MainHall:
                DefendingBase.OnMainHallRobbed(transform.position, goldToTake, elixirToTake);
                break;
        }
        
        if (Health <= 0) GetDestroyed();
    }

    public override void HealBy(float value)
    {
        if (!alive) return;
        Health = Mathf.Clamp(Health + value, 0, BaseVersion.health);
    }

    public abstract void OnInteract();
    public abstract void OnUpgradeUISelected();

    #region Destroy and Repair

    void GetDestroyed()
    {
        alive = false;
        if (combatMode) DefendingBase.instance.OnDestroyed(this);
        BaseInstanceData.destroyed = true;
        source.clip = BaseVersion.destroySound;
        source.Play();
        SpawnDestroyEffect();
    }

    void SpawnDestroyEffect()
    {
        destroyedFX.SetActive(true);
        fixedFX.SetActive(false);
    }

    void SpawnFixEffect()
    {
        destroyedFX.SetActive(false);
        fixedFX.SetActive(true);
    }

    void GetFixed()
    {
        ClientSend.BuildingFixed(BaseInstanceData.ID);
        BaseInstanceData.destroyed = false;
        alive = true;
        source.clip = BaseVersion.repairSound;
        source.Play();
        SpawnFixEffect();
    }

    #endregion

    IEnumerator RepairRoutine()
    {
        SpawnDestroyEffect();
        IsRepairing = true;
        Health = 0;
        yield return new WaitForSeconds(BaseVersion.repairTime.TotalSeconds);
        Health = BaseVersion.health;
        IsRepairing = false;
        GetFixed();
    }

    public void Upgrade()
    {
        BaseInstanceData.level++;

        var building = Base.GenerateBuilding(BaseInstanceData);
        
        Base.Buildings.Remove(this);

        switch (BaseData.Type)
        {
            case BuildingData.BuildingType.ArmyHolder:
                Base.ArmyZones.Add(building as CampZone);
                Base.ArmyZones.Remove(this as CampZone);
                break;

            case BuildingData.BuildingType.Bunker:
                Base.Bunkers.Add(building as Bunker);
                Base.Bunkers.Remove(this as Bunker);
                break;

            case BuildingData.BuildingType.Decoration:
                Base.Decorations.Add(building as Decoration);
                Base.Decorations.Remove(this as Decoration);
                break;

            case BuildingData.BuildingType.Lab:
                Base.UpgradeLab(building as Lab);
                break;

            case BuildingData.BuildingType.MainHall:
                Base.UpgradeMainHaill(building as MainHall);
                break;

            case BuildingData.BuildingType.Mine:
                var oldMine = this as Mine;
                var newMine = building as Mine;
                Base.Mines.Add(newMine);
                Base.Mines.Remove(oldMine);
                List<Mine> mines;
                switch((BaseData as MineData).resourceType)
                {
                    case BuildingData.ResourceType.Gold: mines = Base.GoldMines; break;
                    case BuildingData.ResourceType.Elixir: mines = Base.ElixirMines; break;
                    default: mines = Base.GemMines; break;
                }
                mines.Add(newMine);
                mines.Remove(oldMine);
                break;

            case BuildingData.BuildingType.Storage:
                var oldStorage = this as Storage;
                var newStorage = building as Storage;
                //Base.Storages.Add(newStorage);
                //Base.Storages.Remove(oldStorage);
                List<Storage> storages;
                switch ((BaseData as StorageData).storageType)
                {
                    case StorageData.StoreType.Gold: storages = Base.GoldStorages; break;
                    case StorageData.StoreType.Elixir: storages = Base.ElixirStorages; break;
                    default: storages = null; break;
                }
                storages.Add(newStorage);
                storages.Remove(oldStorage);
                break;

            case BuildingData.BuildingType.TrainingZone:
                Base.TrainingZones.Add(building as TrainingZone);
                Base.TrainingZones.Remove(this as TrainingZone);
                break;

            case BuildingData.BuildingType.Trap:
                Base.Traps.Add(building as Trap);
                Base.Traps.Remove(this as Trap);
                break;

            case BuildingData.BuildingType.Turret:
                Base.Turrets.Add(building as Turret);
                Base.Turrets.Remove(this as Turret);
                break;

            case BuildingData.BuildingType.Wall:
                Base.Walls.Add(building as Wall);
                Base.Walls.Remove(this as Wall);
                break;
        }

        Destroy(gameObject);
    }

    void OnValidate()
    {
        if (center == null) center = transform;
    }
}
