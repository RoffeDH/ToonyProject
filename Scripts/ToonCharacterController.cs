using System;
using UnityEngine;

namespace ToonPhysics
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InputManager))]
    public class ToonCharacterController : MonoBehaviour
    {
        //Components
        Rigidbody rb;
        InputManager input;
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
        Quaternion targetUprightRotation;

        [Header("Locomotion controls")]
        [SerializeField] float maxSpeed;
        [SerializeField] float accel;
        [SerializeField] float maxAccel;
        [SerializeField] Vector3 forceScale;
        [SerializeField] AnimationCurve AccellerationFactorFromDot;

        [Header("Jump/Fall control")]
        [SerializeField] float fallGravity;
        [SerializeField] float cayoteTime;
        [SerializeField] float jumpForce;
        [SerializeField] float jumpTime;

        float calcJumpForce;
        float cayoteTimeCounter;
        float jumpBufferCounter;
        bool canJump;
        bool isJumping;

        bool isGrounded;
        float moveDisableTimer = 0;
        Vector3 goalVelocity;

        Vector3 downDir = new Vector3(0, -1, 0);

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<InputManager>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            CalculateJumpVariables();
            CalculateUprightPosition();
        }

        private void CalculateJumpVariables()
        {
            float timeToApex = jumpTime / 2;
            calcJumpForce = (2 * jumpForce / timeToApex);
        }

        void FixedUpdate()
        {
            SpringController();
            UprightForceController();
            LocomotionController();
            CayoteJumpController();
        }

        //private void JumpController()
        //{
        //    float _modifiedGravity = 1;
        //    if (input.IsJumpKeyPressed() && isGrounded)
        //    {
        //        rb.AddForce(Vector3.up * calcJumpForce, ForceMode.Impulse);
        //    }
        //    else if (!input.IsJumpKeyPressed() && !isGrounded)
        //    {
        //        _modifiedGravity = fallGravity / Time.deltaTime;
        //        _modifiedGravity = Mathf.Clamp(_modifiedGravity, 1, fallGravity);
        //    }
        //    else if (isGrounded)
        //    {
        //        _modifiedGravity = 1;
        //    }

        //    rb.velocity += Physics.gravity * _modifiedGravity * Time.deltaTime;
        //}

        private void CayoteJumpController()
        {
            CayoteTimeCalculator();
            float _modifiedGravity = 1;
            if (input.IsJumpKeyPressed() && canJump)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * calcJumpForce, ForceMode.Impulse);
            }
            else if (!input.IsJumpKeyPressed() && !isGrounded)
            {
                Debug.Log("Jump");
                _modifiedGravity = fallGravity / Time.deltaTime;
                _modifiedGravity = Mathf.Clamp(_modifiedGravity, 1, fallGravity);
            }
            else if (isGrounded)
            {
                isJumping = false;
                _modifiedGravity = 1;
            }

            rb.velocity += Physics.gravity * _modifiedGravity * Time.deltaTime;
        }

        private void CayoteTimeCalculator()
        {
            bool _jumpKeyReleased = true;

            if (!isGrounded)
                _jumpKeyReleased = input.IsJumpKeyReleased();

            cayoteTimeCounter -= Time.deltaTime;

            if (isGrounded)
                cayoteTimeCounter = cayoteTime;

            canJump = cayoteTimeCounter > 0 && _jumpKeyReleased && !isJumping;

            if (canJump)
            {
                isJumping = input.IsJumpKeyPressed();
            }

            if (!isGrounded)
                canJump = false;
        }

        private void LocomotionController()
        {
            //GroundVelocity
            Vector3 _groundVel = Vector3.zero;

            Vector3 _move = Vector3.zero; //m_UnitGoal;
            _move += Vector3.ProjectOnPlane(mainCamera.transform.right, transform.up).normalized * input.GetHorizontalMovementInput();
            _move += Vector3.ProjectOnPlane(mainCamera.transform.forward, transform.up).normalized * input.GetVerticalMovementInput();
            _move = _move.normalized;

            if (moveDisableTimer > 0)
            {
                _move = Vector3.zero;
                moveDisableTimer -= Time.deltaTime;
            }

            if (_move.magnitude > .1f)
                transform.rotation = Quaternion.LookRotation(_move, Vector3.up);

            //Calculate velocity
            float _velDot = Vector3.Dot(_move, rb.velocity);
            float _accel = accel * AccellerationFactorFromDot.Evaluate(_velDot);
            Vector3 _goalVel = _move * maxSpeed;

            goalVelocity = Vector3.MoveTowards(goalVelocity, _goalVel + _groundVel, _accel * Time.deltaTime);

            //Actual force
            Vector3 _neededAccel = (goalVelocity - rb.velocity) / Time.deltaTime;

            _neededAccel = Vector3.ClampMagnitude(_neededAccel, maxAccel);

            rb.AddForce(Vector3.Scale(_neededAccel * rb.mass, forceScale));
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

                isGrounded = true;

                Debug.DrawLine(transform.position, _rayHit.point, Color.green);
            }
            else
            {
                isGrounded = false;
            }
        }
    }
}
