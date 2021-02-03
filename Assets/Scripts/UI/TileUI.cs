using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CT.Data;
using CT.Manager;

namespace CT.UI
{
    public class TileUI : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Image image;

        Base _base;
        public Building Building { get; private set; }
        public Construction Construction { get; private set; }
        GridManager manager;

        public bool IsFree => Building == null && Construction == null;

        Color normalColor;
        Color buildingHereColor;

        public void Init(GridManager manager, int x, int y)
        {
            this.manager = manager;
            _base = manager._base;
            X = x;
            Y = y;
            Building = _base[x, y];
            Construction = ConstructionManager.instance[x, y];
            if (Building != null)
                Construction = ConstructionManager.instance.GetUpgradeOf(Building.BaseInstanceData);
            normalColor = manager.tileNormalColor;
            var color = Building == null ? normalColor : manager.StartBuildingHereColor;
            if (Construction != null) color = manager.ConstructionHereColor;
            SetColor(color);
            buildingHereColor = manager.NowBuildingHereColor;
        }

        public void ResetColor()
        {
            var color = Building == null ? normalColor : buildingHereColor;
            if (Construction != null) color = manager.ConstructionHereColor;
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetFree()
        {
            Building = null;
            Construction = null;
            ResetColor();
        }

        public void OnClick()
        {
            //if (Building != null) return; // may do something else
            manager.OnTileClicked(X, Y);
        }
    }
}