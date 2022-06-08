using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class ConveyerBelt : MonoBehaviour
    {
        [SerializeField] float speed;
        private bool turning = true;
        Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (turning)
                RotateBelt();
        }

        private void RotateBelt()
        {
            float _speed = speed * Time.deltaTime;
            Vector3 pos = rb.position;

            rb.position = transform.position + (-transform.forward * _speed);
            rb.MovePosition(pos);
        }

        public void StartStop()
        {
            turning = !turning;
        }

        public void StartBelt()
        {
            turning = true;
        }

        public void StopBelt()
        {
            turning = false;
        }
    }
}