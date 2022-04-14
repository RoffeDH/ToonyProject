using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class Spinner : Obstacle
    {
        [SerializeField] Transform spinner;
        [SerializeField] float speed;
        [SerializeField] Vector3 axis;
        [SerializeField] float force;
        [SerializeField] bool spin;
        Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!spin)
                return;

            float _speed = speed * Time.deltaTime;
            spinner.Rotate(axis, _speed);


        }

        private void OnCollisionEnter(Collision col)
        {
            ToonCharacterController controller = col.transform.GetComponent<ToonCharacterController>();
            if(controller != null)
            {
                controller.PhysicsHit();
            }

            Vector3 _colPos = col.transform.position;
            Vector3 _colDir = col.GetContact(0).point - _colPos;

            Vector3 _force = Vector3.zero;
            _force += -_colDir.normalized * force;

            Rigidbody _rb = col.transform.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.AddForce(_force, ForceMode.Impulse);
            }
        }

        public override void Trigger()
        {
            spin = true;
        }

        public override bool CanTrigger()
        {
            return !spin;
        }
    }
}
