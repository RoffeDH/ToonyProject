using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float _speed = speed * Time.deltaTime;
        Vector3 pos = rb.position;

        rb.position = transform.position + (-transform.forward * _speed);
        rb.MovePosition(pos);
    }
}
