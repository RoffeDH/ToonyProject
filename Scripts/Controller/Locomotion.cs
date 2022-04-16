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

        [Header("Locomotion controls")]
        [SerializeField] float maxSpeed;
        [SerializeField] float accel;
        [SerializeField] float maxAccel;
        [SerializeField] Vector3 forceScale;
        [SerializeField] AnimationCurve AccellerationFactorFromDot;
        [SerializeField] float timeOutTime;
        float moveDisableTimer;

        bool canRotate = true;
        Vector3 goalVelocity;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            spring = GetComponent<Spring>();
        }

        private void FixedUpdate()
        {
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

            if (direction.magnitude > .1f && canRotate)
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

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

        public void OverrideLookRotation(Vector3? target = null)
        {
            canRotate = true;
            if (target == null)
                return;

            canRotate = false;
            Vector3 dir = (Vector3)target - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        public void PhysicsHit()
        {
            moveDisableTimer = timeOutTime;
        }
    }
}