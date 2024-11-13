using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private float move;
    public float moveSpeed;
    private float rotation;
    public float rotationSpeed;
    private Rigidbody2D rb;
    private float targetMove;
    private bool isReversed;
    public GameObject forwardBeam;
    public GameObject reverseBeam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 2f;
        rotationSpeed = 50f;
        targetMove = 0f;
        isReversed = false;
        reverseBeam.SetActive(false);
        forwardBeam.SetActive(true);
    }

    public void MoveTank(Vector2 input)
    {
        Debug.Log("MoveTank: " + input);
        
        rotation = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        rotation += isReversed ? 180 : 0;
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

    public void Reverse()
    {
        isReversed = !isReversed;
        Debug.Log("reverse called");
    }

    private void FixedUpdate()
    {
        if (isReversed)
        {
            forwardBeam.SetActive(false);
            reverseBeam.SetActive(true);
        }
        else
        {
            forwardBeam.SetActive(true);
            reverseBeam.SetActive(false);
        }
        move = Mathf.Lerp(move, targetMove, Time.fixedDeltaTime * 10);
        Vector3 moveDirection = transform.right * move * Time.fixedDeltaTime;
        if (isReversed)
        {
            moveDirection = Quaternion.Euler(0, 0, 180) * moveDirection;
        }
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
