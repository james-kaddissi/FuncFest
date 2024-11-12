using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 700f;
    public float maxDistance = 20f;
    public GameObject targetCircle; 
    public GameObject projectilePrefab;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private bool readyToShoot = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        readyToShoot = true;
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
        Vector3 movement = transform.right * moveDirection.x + transform.forward * moveDirection.y;

        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    public void UpdateShoot(Vector2 aimDirection)
    {
        Vector3 targetDirection = new Vector3(aimDirection.x, 0, aimDirection.y);

        if (targetDirection.sqrMagnitude > 0.01f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            CreateTargetCircle(targetDirection, aimDirection.magnitude);
        }
    }

    private void CreateTargetCircle(Vector3 targetDirection, float distance) {
        float targetDistance = Mathf.Clamp(distance * maxDistance, 0f, maxDistance);

        Vector3 targetPosition = transform.position + targetDirection.normalized * targetDistance;

        targetPosition.y = 0f;

        if (targetCircle != null)
        {
            targetCircle.transform.position = targetPosition;
            targetCircle.SetActive(true);
            if (readyToShoot){
                LaunchProjectile(targetPosition);
            }
        }
    }
    void ResetReadyToShoot()
    {
        readyToShoot = true;
    }

    private void LaunchProjectile(Vector3 targetPosition)
    {
        readyToShoot = false;
        Invoke("ResetReadyToShoot", 1f);
        // Ensure the target position is on the ground (y = 0)
        targetPosition.y = 0f;

        // Calculate the horizontal distance (range) to the target
        float horizontalDistance = Vector3.Distance(transform.position, targetPosition);

        // Acceleration due to gravity
        float gravity = Physics.gravity.y; // Should be negative, typically -9.81

        // Calculate the required initial velocity to hit the target
        float initialVelocity = Mathf.Sqrt(horizontalDistance * Mathf.Abs(gravity));

        // Calculate the horizontal and vertical velocity components
        float velocityX = initialVelocity * Mathf.Cos(45f * Mathf.Deg2Rad); // Convert 45 degrees to radians
        float velocityY = initialVelocity * Mathf.Sin(45f * Mathf.Deg2Rad);

        // Set the initial velocity vector (we assume player is facing the target)
        Vector3 launchVelocity = transform.forward * velocityX;
        launchVelocity.y = velocityY;

        // Instantiate and launch the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position + Vector3.up * 0.75f, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = launchVelocity;
        }
        else
        {
            Debug.LogError("Rigidbody not found on the projectile prefab.");
        }
    }

}
