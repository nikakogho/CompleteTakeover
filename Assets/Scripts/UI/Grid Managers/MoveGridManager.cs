using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager;

namespace CT.UI
{
    public class MoveGridManager : GridManager
    {
        public Building Moving { get; private set; }
        public int ID => Moving.BaseInstanceData.ID;
        public int MoveX { get; private set; }
        public int MoveY { get; private set; }
        [HideInInspector]public Vector3 originalPos;
        [HideInInspector]public Quaternion originalRot;
        public Vector3 MovePos => Moving.transform.position;
        public Quaternion MoveRot => Moving.transform.rotation;
        public bool Valid { get; private set; }

        public override Color StartBuildingHereColor => tileValidColor;
        public override Color NowBuildingHereColor => tileInvalidColor;
        public override Color ConstructionHereColor => tileInvalidColor;

        public Button confirmButton;

        public static MoveGridManager instance;
        MoveManager manager;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            manager = MoveManager.instance;
            //GenerateGrid();
        }

        void ThinkToMove(Building building, int x, int y)
        {
            //Debug.Log("Thinking to move " + building.name);
            //if (grid != null) DestroyGrid();
            //GenerateGrid();
            Moving = building;
            originalPos = Moving.transform.position;
            originalRot = Moving.transform.rotation;
            manager.OnBuildingSelected();
            Moving.transform.position = ToPosition(x, y);
            MoveX = building.BaseInstanceData.tileX;
            MoveY = building.BaseInstanceData.tileY;
            SetBuildingPos(x, y);
        }

        void OnEnable()
        {
            if (grid != null) DestroyGrid();
            GenerateGrid();
        }

        void OnDisable()
        {
            OnCancel();
        }

        public void OnCancel()
        {
            if (Moving == null) return;
            Moving.transform.position = originalPos;
            Moving.transform.rotation = originalRot;
            Moving = null;
        }

        void SetBuildingPos(int x, int y)
        {
            int oldX = MoveX;
            int oldY = MoveY;
            MoveX = x;
            MoveY = y;
            Moving.transform.position = ToPosition(x, y);
            ResetGrid();
            var data = Moving.BaseData;
            for (int x0 = 0; x0 < data.tileWidth; x0++)
                for (int y0 = 0; y0 < data.tileHeight; y0++)
                    grid[oldX + x0, oldY + y0].SetFree();
            Valid = true;
            int endX = Mathf.Min(x + data.tileWidth, BaseData.Width);
            int endY = Mathf.Min(y + data.tileHeight, BaseData.Height);
            for (int X = x; X < endX && Valid; X++)
                for (int Y = y; Y < endY; Y++)
                    if (grid[X, Y].Building != null && grid[X, Y].Building != Moving ||
                        grid[X, Y].Construction != null)
                    {
                        Valid = false;
                        break;
                    }
            var color = Valid ? tileValidColor : tileInvalidColor;
            for (int X = x; X < endX; X++)
                for (int Y = y; Y < endY; Y++)
                    grid[X, Y].SetColor(color);
            confirmButton.interactable = Valid;
        }

        public override void OnTileClicked(int x, int y)
        {
            var tile = grid[x, y];
            //Debug.Log("Moving something: " + (Moving != null));
            if(Moving == null)
            {
                if (tile.Building == null) { }// Debug.Log("Tile has no building!");
                else if (tile.Construction != null) { }// Debug.Log("Construction Here!");
                else ThinkToMove(tile.Building, x, y);
            }
            else
            {
                SetBuildingPos(x, y);
            }
        }
    }
}