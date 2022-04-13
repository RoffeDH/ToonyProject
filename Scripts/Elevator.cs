using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Vector3 startingPosition;
    [SerializeField] Vector3 endPosition;
    [SerializeField] Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        endPosition = transform.position + endPosition;
        targetPosition = startingPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 _currentPosition = transform.position;
        float _distance = Vector3.Distance(_currentPosition, targetPosition);

        Vector3 _direction = (targetPosition - _currentPosition).normalized;

        if (_direction.y > 0 || (_direction.y == 0 && targetPosition == startingPosition))
            targetPosition = endPosition;
        else if (_direction.y < 0 || (_direction.y == 0 && targetPosition == endPosition))
            targetPosition = startingPosition;

        Vector3 _targetDirection = (targetPosition - _currentPosition).normalized;

        rb.MovePosition(transform.position + _targetDirection * Time.deltaTime);
    }
}
