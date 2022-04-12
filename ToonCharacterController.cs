using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonPhysics
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputController))]
    public class ToonCharacterController : MonoBehaviour
    {
        //Components
        Rigidbody rb;
        InputController input;
        Camera mainCamera;

        [Header("Spring controls")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float springTargetHeight;
        [SerializeField] float springMaxLength;
        [SerializeField] float springStrength;
        [SerializeField] float springDamper;

        [Header("Joint controls")]
        [SerializeField] float uprightSpringStrength;
        [SerializeField] float uprightSpringDamper;
        public bool editTargetUprightRotation;
        public Quaternion targetUprightRotation = Quaternion.Euler(Vector3.up);

        [Header("Locomotion controls")]
        [SerializeField] float fallGravity;
        [SerializeField] float maxSpeed;
        [SerializeField] float accelleration;
        [SerializeField] float maxAccel;
        [SerializeField] Vector3 forceScale;
        [SerializeField] float jumpHeight;
        [SerializeField] float jumpForce;
        [SerializeField] AnimationCurve AccellerationFactorFromDot;

        bool canJump;
        float moveDisableTimer = 0;
        Vector3 neededAccel;

        Vector3 downDir = new Vector3(0, -1, 0);

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<InputController>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            CalculateUprightPosition();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            SpringController();

            if (editTargetUprightRotation)
                UprightForceController();

            LocomotionController();
            JumpController();
        }

        private void JumpController()
        {
            if (input.IsJumpKeyPressed() && canJump)
            {
                Vector3 _jumpForce = Vector3.up * jumpForce * Time.deltaTime;
                rb.AddForce(_jumpForce, ForceMode.Impulse);
            }
        }

        private void LocomotionController()
        {
            Vector3 _move = Vector3.zero; //m_UnitGoal;
            _move += Vector3.ProjectOnPlane(mainCamera.transform.right, transform.up).normalized * input.GetHorizontalMovementInput();
            _move += Vector3.ProjectOnPlane(mainCamera.transform.forward, transform.up).normalized * input.GetVerticalMovementInput();

            if (_move.magnitude > .1f)
                transform.rotation = Quaternion.LookRotation(_move, Vector3.up);

            rb.AddForce(_move * accelleration);

            //Vector3 _targetVelocity = Vector3.zero;

            //if (moveDisableTimer > 0)
            //{
            //    _move = Vector3.zero;
            //    moveDisableTimer -= Time.deltaTime;
            //}

            //if (_move.magnitude > 1)
            //    _move.Normalize();

            ////Vector3 unitVel = _targetVelocity.normalized;

            ////float velDot = Vector3.Dot(_move, unitVel);

            ////float accel = accelleration * AccellerationFactorFromDot.Evaluate(velDot);

            //_targetVelocity = _move * maxSpeed;

            //Vector3 _goalVel = Vector3.MoveTowards(targetVelocity, _targetVelocity, accelleration * Time.deltaTime);

            //Vector3 _neededAccel = (_goalVel - rb.velocity) / Time.deltaTime;

            //neededAccel = Vector3.ClampMagnitude(_neededAccel, maxAccel);

            //rb.AddForce(Vector3.Scale(neededAccel * rb.mass, forceScale));
        }

        private void UprightForceController()
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion goalRot = UtilsMath.ShortestRotation(targetUprightRotation, currentRotation);

            Vector3 rotAxis;
            float rotDegrees;

            goalRot.ToAngleAxis(out rotDegrees, out rotAxis);
            rotAxis.Normalize();

            float rotRadians = rotDegrees * Mathf.Deg2Rad;

            rb.AddTorque((rotAxis * (rotRadians * uprightSpringStrength)) - (rb.angularVelocity * uprightSpringDamper));
        }

        private void CalculateUprightPosition()
        {
            Vector3 _currentRot = transform.rotation.eulerAngles;
            targetUprightRotation = Quaternion.Euler(new Vector3(0, _currentRot.y, 0));
        }

        private void SpringController()
        {
            Ray _ray = new Ray(transform.position, downDir);
            RaycastHit _rayHit;
            bool _rayDidHit = Physics.Raycast(_ray, out _rayHit, springMaxLength, groundLayer);

            if (_rayDidHit)
            {
                Vector3 _vel = rb.velocity;
                Vector3 _rayDir = downDir;

                float _rayDirVel = Vector3.Dot(_rayDir, _vel);

                float x = _rayHit.distance - springTargetHeight;

                float _springForce = (x * springStrength) - (_rayDirVel * springDamper);

                rb.AddForce(_rayDir * _springForce);

                canJump = true;

                Debug.DrawLine(transform.position, _rayHit.point, Color.green);
            }
            else
            {
                if(rb.velocity.y < 0)
                    rb.velocity += Physics.gravity * fallGravity * Time.deltaTime;

                canJump = false;
            }
        }
    }
}
