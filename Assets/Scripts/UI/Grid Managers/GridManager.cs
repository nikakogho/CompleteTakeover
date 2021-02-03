using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;

namespace CT.UI
{
    public abstract class GridManager : MonoBehaviour
    {
        public Base _base;
        public GameObject tilePrefab;

        public Transform bottomLeftPoint;

        [Header("Tile Colors")]
        public Color tileNormalColor = Color.grey;
        public Color tileValidColor = Color.green;
        public Color tileInvalidColor = Color.red;

        public abstract Color NowBuildingHereColor { get; }
        public abstract Color StartBuildingHereColor { get; }
        public abstract Color ConstructionHereColor { get; }

        protected PlayerData player;
        protected Faction faction;

        protected TileUI[,] grid;

        public static Vector3 ToPosition(int x, int y)
        {
            return BuildGridManager.instance.bottomLeftPoint.position + new Vector3(x, 0, y) * BaseData.TileDistance;
        }

        protected void DestroyGrid()
        {
            if (grid == null) return;
            foreach (var tile in grid) if(tile != null) Destroy(tile.gameObject);
            grid = null;
        }

        protected void GenerateGrid()
        {
            grid = new TileUI[BaseData.Width, BaseData.Height];
            for (int y = 0; y < BaseData.Height; y++) for (int x = 0; x < BaseData.Width; x++) SpawnTile(x, y);
        }

        protected void ResetGrid()
        {
            foreach (var tile in grid) tile.ResetColor();
        }

        void SpawnTile(int x, int y)
        {
            var obj = Instantiate(tilePrefab, transform.position, transform.rotation, transform);
            var tile = obj.GetComponent<TileUI>();
            tile.Init(this, x, y);
            grid[x, y] = tile;
        }

        public abstract void OnTileClicked(int x, int y);
    }
}