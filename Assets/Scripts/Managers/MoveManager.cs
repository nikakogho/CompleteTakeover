using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.UI;
using CT.Net;

namespace CT.Manager
{
    public class MoveManager : UIManager {
        public GameObject gridUI;

        public GameObject choicesPanel;

        public static MoveManager instance;

        protected override KeyCode ActivationKey => KeyCode.M;

        MoveGridManager manager;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {
            base.Start();
            manager = MoveGridManager.instance;
        }

        protected override void OnUIToggled()
        {
            gridUI.SetActive(on);
        }

        public void Confirm()
        {
            if (manager == null) manager = MoveGridManager.instance;
            if (!manager.Valid)
            {
                Cancel();
                return;
            }
            var building = manager.Moving;
            building.BaseInstanceData.tileX = manager.MoveX;
            building.BaseInstanceData.tileY = manager.MoveY;
            manager.originalPos = building.transform.position;
            manager.originalRot = building.transform.rotation;
            ClientSend.MoveBuilding(manager.ID, manager.MoveX, manager.MoveY);
            Disable();
        }

        public void Cancel()
        {
            if (manager == null) manager = MoveGridManager.instance;
            manager.OnCancel();
            Disable();
        }

        public void OnBuildingSelected()
        {
            choicesPanel.SetActive(true);
        }

        protected override void Disable()
        {
            base.Disable();
            if(choicesPanel != null && choicesPanel.activeSelf) choicesPanel.SetActive(false);
            if(gridUI != null && gridUI.activeSelf) gridUI.SetActive(false);
        }
    }
}
