using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Manager
{
    public abstract class UIManager : MonoBehaviour
    {
        public GameObject ui;

        protected static List<UIManager> allManagers;
        protected List<UIManager> otherManagers;

        protected abstract KeyCode ActivationKey { get; }

        GameManager gameManager;

        protected bool on = false;

        protected virtual void Awake()
        {
            if (allManagers == null) allManagers = new List<UIManager>();
            allManagers.Add(this);
        }

        protected virtual void Start()
        {
            gameManager = GameManager.instance;
            otherManagers = new List<UIManager>(allManagers);
            otherManagers.Remove(this);
        }

        public void OnManagerSelected()
        {
            on = !on;
            ui.SetActive(on);
            if (on) foreach (var manager in otherManagers) manager.Disable();
            else Disable();
            gameManager.ActionUISelected(on);
            OnUIToggled();
        }

        protected virtual void Disable()
        {
            on = false;
            if(ui != null && ui.activeSelf) ui.SetActive(false);
            gameManager.ActionUISelected(false);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(ActivationKey)) OnManagerSelected();
        }

        protected abstract void OnUIToggled();
    }
}