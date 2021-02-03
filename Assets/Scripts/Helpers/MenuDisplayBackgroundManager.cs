using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Helper
{
    public class MenuDisplayBackgroundManager : MonoBehaviour
    {
        public Transform target;

        [ContextMenu("Apply")]
        void Apply()
        {
            foreach (var controller in GetComponentsInChildren<PlayerController>()) controller.transform.LookAt(target);
        }
    }
}