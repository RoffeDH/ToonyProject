using System;
using UnityEngine;

namespace Toony
{
    public class ToonCharacterController : ToonyInputs
    {
        //Components
        protected Camera mainCamera;
        protected Locomotion locomotion;
        protected Spring spring;
        protected Jumping jumping;

        // Start is called before the first frame update
        void Start()
        {
            DefineComponents();
        }

        protected void DefineComponents()
        {
            mainCamera = Camera.main;
            spring = GetComponent<Spring>();
            jumping = GetComponent<Jumping>();
            locomotion = GetComponent<Locomotion>();
        }

        private void Update()
        {
            ToonCharacterImplementation();
        }

        protected void ToonCharacterImplementation()
        {
            spring.CalculateUprightPosition();
            Vector3 _move = Vector3.zero;
            _move += Vector3.ProjectOnPlane(mainCamera.transform.right, transform.up) * horizontal;
            _move += Vector3.ProjectOnPlane(mainCamera.transform.forward, transform.up) * vertical;
            locomotion.direction = Vector3.ClampMagnitude(_move, 1f);

            jumping.JumpKeyPressed(jumpButton);
            jumping.JumpKeyReleased(jumpButtonIsReleased);
        }

        void FixedUpdate()
        {
            jumping.CalculateJumpVariables();
            jumping.CayoteJumpController();
        }
    }
}
