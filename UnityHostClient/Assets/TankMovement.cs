using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private float move;
    private float moveSpeed;
    private float rotation;
    private float rotationSpeed;

    void Start()
    {
        moveSpeed = 25f;
        rotationSpeed = 50f;
    }

    public void MoveTank(Vector2 input)
    {
        Debug.Log("MoveTank: " + input);
        
        rotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if(input.magnitude > 0.1f)
        {
            move = moveSpeed * Time.deltaTime;
        }
        else
        {
            move = 0;
        }
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotation);
        Vector3 moveDirection = transform.right;
        transform.Translate(moveDirection * move, Space.World);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
