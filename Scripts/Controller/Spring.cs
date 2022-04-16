using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class Spring : MonoBehaviour
    {
        Rigidbody rb;
        [SerializeField] LayerMask groundLayer;

        [Header("Spring controls")]
        [SerializeField] float springTargetHeight;
        [SerializeField] float springMaxLength;
        [SerializeField] float springStrength;
        [SerializeField] float springDamper;
        
        [Header("Joint controls")]
        [SerializeField] float uprightSpringStrength;
        [SerializeField] float uprightSpringDamper;
        Quaternion targetUprightRotation;

        Vector3 downDir = new Vector3(0, -1, 0);
        RaycastHit rayHit;
        bool isGrounded;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            CalculateUprightPosition();
            UprightForceController();
            SpringController();
        }

        private void SpringController()
        {

            Ray _ray = new Ray(transform.position, downDir);
            RaycastHit _rayHit;
            isGrounded = false;


            bool _rayDidHit = Physics.Raycast(_ray, out _rayHit, springMaxLength, groundLayer);
            if (!_rayDidHit)
                return;

            isGrounded = Vector3.Distance(_rayHit.point, transform.position) < springTargetHeight;

            Debug.DrawLine(transform.position, transform.position + downDir * springMaxLength, Color.red);

            if (isGrounded)
            {
                rayHit = _rayHit;
                Vector3 _vel = rb.velocity;
                Vector3 _rayDir = transform.TransformDirection(downDir);
                Debug.DrawLine(transform.position, _rayHit.point, Color.green);

                Vector3 _otherVel = Vector3.zero;
                Rigidbody _hitBody = _rayHit.rigidbody;

                if (_hitBody != null)
                {
                    _otherVel = _hitBody.velocity;
                }

                float _rayDirVel = Vector3.Dot(_rayDir, _vel);
                float _otherDirVel = Vector3.Dot(_rayDir, _otherVel);

                float _relVel = _rayDirVel - _otherDirVel;

                float x = _rayHit.distance - springTargetHeight;

                float _springForce = (x * springStrength) - (_relVel * springDamper);

                //Debug.DrawLine(origin.position, origin.position + (_rayDir * _springForce), Color.yellow);

                rb.AddForce(_rayDir * _springForce);

                if (_hitBody != null)
                {
                    _hitBody.AddForceAtPosition(_rayDir * -_springForce, _rayHit.point);
                }
            }
            else
            {
                rayHit = new RaycastHit();
            }
        }

        public void UprightForceController()
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

        public void CalculateUprightPosition()
        {
            Vector3 _currentRot = transform.rotation.eulerAngles;
            targetUprightRotation = Quaternion.Euler(new Vector3(0, _currentRot.y, 0));
        }

        public RaycastHit GetRaycast()
        {
            return rayHit;
        }

        public bool GetIsGrounded()
        {
            return isGrounded;
        }
    }
}
