using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    float h, v;
    bool j, a, jIsReleased;

    private void Start()
    {
        j = a = false;
        jIsReleased = true;
        h = v = 0f;
    }

    public void Horizontal(InputAction.CallbackContext context)
    {
        float? i = (float?)context.ReadValueAsObject();

        if (i == null)
        {
            h = 0;
        }
        else
        {
            h = (float)i;
        }
    }

    public void Vertical(InputAction.CallbackContext context)
    {
        float? i = (float?)context.ReadValueAsObject();

        if (i == null)
        {
            v = 0;
        }
        else
        {
            v = (float)i;
        }
    }

    public void Jumping(InputAction.CallbackContext context)
    {
        if (context.started)
        { 
            j = true;
            jIsReleased = false;
        }
        else if (context.performed)
            j = false;
        else if (context.canceled)
        {
            j = false;
            jIsReleased = true;
        }
    }
    
    public void Attacking(InputAction.CallbackContext context)
    {
        a = (bool)context.ReadValueAsButton();
    }

    public float GetHorizontalMovementInput()
    {
        return h;
    }

    public  float GetVerticalMovementInput()
    {
        return v;
    }

    public Vector3 GetMovementVector3()
    {
        return new Vector3(GetHorizontalMovementInput(), 0, GetVerticalMovementInput());
    }

    public bool IsJumpKeyPressed()
    {
        return j;
    }

    public bool IsJumpKeyReleased()
    {
        //Debug.Log("KEY IS RELEASED!");
        return jIsReleased;
    }

    public bool IsAttackKeyPressed()
    {
        return a;
    }
}
