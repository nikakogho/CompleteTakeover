using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CT.Helper
{
    public class BaseCameraController : MonoBehaviour
    {
        public float moveSpeed = 8;

        float moveX = 0;
        float moveY = 0;
        float moveZ = 0;

        void Update()
        {
            float multiplier = Input.GetKey(KeyCode.LeftShift) ? 3 : 1;

            moveX += Input.GetAxis("Horizontal") * multiplier;
            moveZ += Input.GetAxis("Vertical") * multiplier;
        }

        void FixedUpdate()
        {
            if(moveX != 0 || moveZ != 0)
            {
                Vector3 move = new Vector3(moveX, moveY, moveZ);
                transform.position += move * moveSpeed * Time.deltaTime;
                moveX = moveZ = 0;
            }
        }
    }
}