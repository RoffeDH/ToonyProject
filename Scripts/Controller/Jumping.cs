using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toony
{
    public class Jumping : MonoBehaviour
    {
        Rigidbody rb;
        Spring spring;

        [Header("Jump/Fall control")]
        [SerializeField] float cayoteTime;
        [SerializeField] float jumpForce;
        [SerializeField] float jumpTime;
        [SerializeField] float fallGravity;

        float calcJumpForce;
        float cayoteTimeCounter;
        float jumpBufferCounter;
        bool isJumpKeyPressed;
        bool isJumpKeyReleased;
        bool canJump;
        bool isJumping;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            spring = GetComponent<Spring>();
        }

        public void CalculateJumpVariables()
        {
            float timeToApex = jumpTime / 2;
            calcJumpForce = (2 * jumpForce / timeToApex);
        }

        public void CayoteJumpController()
        {
            CayoteTimeCalculator();

            float _modifiedGravity = 1;

            if (isJumping)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * calcJumpForce, ForceMode.Impulse);
            }

            if (!isJumpKeyPressed)
            {
                _modifiedGravity = ModifiedGravity();
            }
            else if (spring.GetIsGrounded())
            {
                isJumping = false;
                _modifiedGravity = 1;
            }

            rb.velocity += Physics.gravity * _modifiedGravity * Time.deltaTime;
        }

        private void CayoteTimeCalculator()
        {
            bool _jumpKeyReleased = true;
            if (!spring.GetIsGrounded())
                _jumpKeyReleased = isJumpKeyReleased;

            cayoteTimeCounter -= Time.deltaTime;
            jumpBufferCounter -= Time.deltaTime;

            if (spring.GetIsGrounded())
                cayoteTimeCounter = cayoteTime;
            else if (_jumpKeyReleased)
                jumpBufferCounter = cayoteTime;

            canJump = (cayoteTimeCounter > 0 && _jumpKeyReleased && !isJumping) || (jumpBufferCounter > 0 && _jumpKeyReleased && !spring.GetIsGrounded());

            if (canJump)
            {
                isJumping = isJumpKeyPressed;
            }

            if (!spring.GetIsGrounded() && jumpBufferCounter < 0)
                isJumping = false;
        }

        public float ModifiedGravity()
        {
            float _gravity = 0;
            _gravity = fallGravity / Time.deltaTime;
            _gravity = Mathf.Clamp(_gravity, 1, fallGravity);
            return _gravity;
        }

        public void JumpKeyPressed(bool b)
        {
            isJumpKeyPressed = b;
        }

        public void JumpKeyReleased(bool b)
        {
            isJumpKeyReleased = b;
        }
    }
}