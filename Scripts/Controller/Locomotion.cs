using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class Locomotion : MonoBehaviour
    {
        public Vector3 direction { get; set; }

        Rigidbody rb;
        Spring spring;

        [SerializeField] float rotationSpeed = 4f;

        [Header("Locomotion controls")]
        [SerializeField] float maxSpeed;
        [SerializeField] float accel;
        [SerializeField] float maxAccel;
        [SerializeField] Vector3 forceScale;
        [SerializeField] AnimationCurve AccellerationFactorFromDot;
        [SerializeField] float timeOutTime;
        float moveDisableTimer;

        //bool canRotate = true;
        bool canTurn = true;
        bool canWalk = true;
        Vector3 goalVelocity;
        LocomotionSettings currentSettings;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            spring = GetComponent<Spring>();
        }

        private void FixedUpdate()
        {
            moveDisableTimer -= Time.deltaTime;
            if(moveDisableTimer < 0)
                LocomotionController();
        }

        private void LocomotionController()
        {
            //GroundVelocity
            Vector3 _groundVel = Vector3.zero;
            RaycastHit _rayHit = spring.GetRaycast();

            if (_rayHit.rigidbody != null)
            {
                _groundVel = _rayHit.rigidbody.velocity;
            }

            if (direction.magnitude > .1f && canTurn)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                ForceRotation(rotation);
            }

            if (!canWalk)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                return;
            }

            //Calculate velocity
            float _velDot = Vector3.Dot(direction, rb.velocity);
            float _accel = accel * AccellerationFactorFromDot.Evaluate(_velDot);
            Vector3 _goalVel = direction * maxSpeed;

            goalVelocity = Vector3.MoveTowards(goalVelocity, _goalVel + _groundVel, _accel * Time.deltaTime);

            //Actual force
            Vector3 _neededAccel = (goalVelocity - rb.velocity) / Time.deltaTime;

            _neededAccel = Vector3.ClampMagnitude(_neededAccel, maxAccel);

            rb.AddForce(Vector3.Scale(_neededAccel * rb.mass, forceScale));
        }

        public void ForceRotation(Quaternion _lookRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);
        }

        public void PhysicsHit()
        {
            moveDisableTimer = timeOutTime;
        }

        public void ChangeLocomotionSettings(LocomotionSettings _settings)
        {
            if (currentSettings == _settings)
                return;

            currentSettings = _settings;

            rotationSpeed = currentSettings.rotationSpeed;
            maxSpeed = currentSettings.maxSpeed;
            accel = currentSettings.accel;
            maxAccel = currentSettings.maxAccel;

            //canRotate = currentSettings.canRotate;
            canWalk = currentSettings.canWalk;
    }
    }
}