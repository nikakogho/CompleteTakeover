using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CT.UI
{
    public class ResourcesTakenUI : MonoBehaviour
    {
        public CanvasGroup group;
        public Image iconImage;
        public Text amountText;

        const float startMoveSpeed = 8;
        float moveSpeed;
        public float lifeTime = 5;
        float countdown;

        bool moving = false;

        public void Init(int amount, Sprite icon)
        {
            iconImage.sprite = icon;
            amountText.text = $"+{amount}";
        }

        public void MoveUp()
        {
            moving = true;
            countdown = lifeTime;
            moveSpeed = startMoveSpeed;
        }

        void Update()
        {
            if (!moving) return;

            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            float fill = countdown / lifeTime;
            group.alpha = fill;
            moveSpeed = startMoveSpeed * fill;
            countdown -= Time.deltaTime;
            if(countdown <= 0)
            {
                moving = false;
                Destroy(gameObject);
            }
        }
    }
}
