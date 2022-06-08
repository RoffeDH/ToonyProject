using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Toony
{
    public class ToonyBase : MonoBehaviour
    {

        protected float horizontal, vertical;
        protected bool jumpButton, jumpButtonIsReleased;

        private void Awake()
        {
            jumpButton = false;
            jumpButtonIsReleased = true;
            horizontal = vertical = 0f;
        }
    }
}
