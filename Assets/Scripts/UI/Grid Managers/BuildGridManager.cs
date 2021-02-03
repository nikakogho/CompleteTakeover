using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.Manager.UI;

namespace CT.UI
{
    public class BuildGridManager : GridManager
    {
        Transform ghostParent;

        public GameObject choicesUI;

        public override Color StartBuildingHereColor => tileInvalidColor;
        public override Color NowBuildingHereColor => tileInvalidColor;
        public override Color ConstructionHereColor => tileInvalidColor;

        public static bool placeWorks;

        BuildingData toBuild;
        GameObject ghost;
        BuildOptionManager bom;

        public bool BuyWithResources { get; private set; }
        public BuildingData Chosen => toBuild;
        public int BuildX { get; private set; }
        public int BuildY { get; private set; }
        public Vector3 BuildPos => ghost.transform.position;
        public Quaternion BuildRot => ghost.transform.rotation;
        public static BuildGridManager instance;

        public BuildGridManager()
        {
            instance = this;
        }

        void Start()
        {
            bom = BuildOptionManager.instance;
        }

        public void ThinkToBuild(BuildingData chosen, bool buyWithResources)
        {
            BuyWithResources = buyWithResources;
            if (grid != null) DestroyGrid();
            GenerateGrid();
            //else { } // may do destroy or reset grid
            if (ghost != null) Destroy(ghost);
            toBuild = chosen;
            ghost = Instantiate(chosen.Original.ghostPrefab, bottomLeftPoint.position, bottomLeftPoint.rotation);
            ghostParent = bottomLeftPoint;
            ghost.transform.parent = ghostParent; //may do change parent
            SetGhostPos(0, 0);
            choicesUI.SetActive(true);
        }

        void OnDisable()
        {
            choicesUI.SetActive(false);
            if (ghost != null) Destroy(ghost);
        }

        bool CanFit(int x, int y)
        {
            if (x + toBuild.tileWidth > BaseData.Width || y + toBuild.tileHeight > BaseData.Height) return false;
            for (int x0 = 0; x0 < toBuild.tileWidth; x0++)
                for (int y0 = 0; y0 < toBuild.tileHeight; y0++)
                    if (!grid[x + x0, y + y0].IsFree) return false;
            return true;
        }
        
        void SetGhostPos(int x, int y)
        {
            BuildX = x;
            BuildY = y;
            if (ghost == null) return; //may do Debug.Log() or some other form of message
            Vector3 offset = new Vector3(x, 0, y) * BaseData.TileDistance;
            ghost.transform.localPosition = offset;
            ResetGrid();
            bool canFit = CanFit(x, y);
            placeWorks = canFit;
            Color color = canFit ? tileValidColor : tileInvalidColor;
            int endX = Mathf.Min(x + toBuild.tileWidth, BaseData.Width);
            int endY = Mathf.Min(y + toBuild.tileHeight, BaseData.Height);
            for (int X = x; X < endX; X++)
                for (int Y = y; Y < endY; Y++)
                    grid[X, Y].SetColor(color);
        }

        public override void OnTileClicked(int x, int y)
        {
            if (!grid[x, y].IsFree) return;
            SetGhostPos(x, y);
        }
        
        public void OnBuild(Construction construction)
        {
            var ui = bom.options[construction.Data.toConstruct];
            ui.UpdateAmounts(ui.CurrentAmount + 1, ui.AllowedAmount);
        }
    }
}