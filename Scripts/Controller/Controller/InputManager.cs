using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace Toony
{
    public class ToonyInputs : MonoBehaviour
    {

        protected float horizontal, vertical;
        protected bool jumpButton, jumpButtonIsReleased;

        private void Awake()
        {
            jumpButton = false;
            jumpButtonIsReleased = true;
            horizontal = vertical = 0f;
        }

        //public void Directional(InputAction.CallbackContext context)
        //{
        //    if (context.ReadValueAsObject() == null)
        //        return;

        //    Vector2 _d = (Vector2)context.ReadValueAsObject();
        //    horizontal = _d[0];
        //    vertical = _d[1];
        //}

        //public void Jumping(InputAction.CallbackContext context)
        //{
        //    if (context.started)
        //    {
        //        jumpButton = true;
        //        jumpButtonIsReleased = false;
        //    }
        //    else if (context.performed)
        //        jumpButton = false;
        //    else if (context.canceled)
        //    {
        //        jumpButton = false;
        //        jumpButtonIsReleased = true;
        //    }
        //}
    }
}
