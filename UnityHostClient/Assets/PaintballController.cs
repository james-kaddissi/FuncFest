using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 700f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += -9.8f * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void UpdateInput(Vector2 moveDirection)
    {
        Debug.LogError("ERR: " + moveDirection);
        Vector3 movement = transform.right * moveDirection.x + transform.forward * moveDirection.y;

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
}
