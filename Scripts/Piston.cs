using UnityEngine;
using System.Collections;

namespace Toony
{
    public class Piston : Obstacle
    {
        [SerializeField] float extensionLength;
        [SerializeField] float timeExtended;
        [SerializeField] float speedExtending;
        [SerializeField] float speedRetracting;
        [SerializeField] Vector3 force;
        Rigidbody rb;
        Quaternion orientation;
        [SerializeField] bool extended;
        float timer;
        bool canTrigger;


        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            extended = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            orientation = transform.rotation;

            if (extended)
            {
                canTrigger = false;
                if (transform.localPosition.y <= extensionLength)
                {
                    rb.MovePosition(rb.position + orientation * Vector3.up * speedExtending * Time.deltaTime);
                    extended = true;
                }

                timer -= Time.deltaTime;

                if (timer < 0)
                {
                    extended = false;
                }
            }

            if (!extended & transform.localPosition.y >= 0.1f) //retract back to start position
            {
                rb.MovePosition(rb.position + orientation * Vector3.down * speedRetracting * Time.deltaTime);
                timer = timeExtended;
                extended = false;
            }

            if(transform.localPosition.y < 0.1f)
            {
                canTrigger = true;
            }
        }

        private void OnTriggerStay(Collider col)
        {
            Rigidbody _rb = col.attachedRigidbody;
            if(_rb != null && extended)
            {
                _rb.AddForce(force, ForceMode.Impulse);
            }
        }

        public override void Trigger()
        {
            extended = true;
        }
        public override bool CanTrigger()
        {
            return canTrigger;
        }
    }
}