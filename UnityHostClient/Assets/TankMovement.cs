using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private float move;
    public float moveSpeed;
    private float rotation;
    private float rotationSpeed;
    private Rigidbody2D rb;
    private float targetMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 2f;
        rotationSpeed = 50f;
        targetMove = 0f;
    }

    public void MoveTank(Vector2 input)
    {
        Debug.Log("MoveTank: " + input);
        
        rotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if(input.magnitude > 0.1f)
        {
            targetMove = moveSpeed * Time.deltaTime * 10000;
        }
        else
        {
            targetMove = 0;
            move = 0;
        }
    }

    private void FixedUpdate()
    {
        move = Mathf.Lerp(move, targetMove, Time.fixedDeltaTime * 10);
        Vector3 moveDirection = transform.right * move * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + new Vector2(moveDirection.x, moveDirection.y));

        if (move > 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rotation = transform.rotation.eulerAngles.z;
        }
    }
}
