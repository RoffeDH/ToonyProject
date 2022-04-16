using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    Rigidbody rb;
    public bool trigger = false;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (trigger)
            anim.Play("Spring");
    }

    public void NullAnimation()
    {
        anim.Play("Empty");
        trigger = false;
    }
}
