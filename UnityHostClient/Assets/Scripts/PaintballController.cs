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
    private Vector3 movement;

    public Transform outlineSprite;
    public float minScale = 0.83f; 
    public float maxScale = 1.17f;  
    public float pulseSpeed = 2f;
    private Vector3 initialScale;

    public Transform targetSprite;
    public float rotationSpeed = 50f;
    public float minScaleTarget = 0.9f;
    public float maxScaleTarget = 1.1f;
    public Vector3 initialScaleTarget;

    public GameObject pointer;
    private RectTransform pointerRectTransform;
    public float bobSpeed = 2f;
    public float bobHeight = 1f;
    private Vector2 initialPosition;

    public Animator animator;
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsStandingFiring = Animator.StringToHash("IsStandingFiring");
    private static readonly int IsForwardMovingFiring = Animator.StringToHash("IsForwardMovingFiring");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        readyToShoot = true;
        initialScale = outlineSprite.localScale;
        initialScaleTarget = targetSprite.localScale;
        pointerRectTransform = pointer.GetComponent<RectTransform>();
        initialPosition = pointer.GetComponent<RectTransform>().anchoredPosition;
        animator.SetBool(IsIdle, true);
        animator.SetBool(IsStandingFiring, false);
        animator.SetBool(IsForwardMovingFiring, false);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        pointer.GetComponent<RectTransform>().anchoredPosition = new Vector2(initialPosition.x, newY);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += -9.8f * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        controller.Move(movement * moveSpeed * Time.deltaTime);
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        outlineSprite.localScale = initialScale * scale;
        targetSprite.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        float scaleTarget = Mathf.Lerp(minScaleTarget, maxScaleTarget, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        targetSprite.localScale = initialScaleTarget * scaleTarget;

    }

    public void UpdateInput(Vector2 moveDirection)
    {
        movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        
    }

    public void UpdateShoot(Vector2 aimDirection)
    {
        Vector3 targetDirection = new Vector3(aimDirection.x, 0, aimDirection.y);

        if (targetDirection.sqrMagnitude > 0.01f) 
        {
            animator.SetBool(IsIdle, false);
            if (movement.sqrMagnitude > 0.01f)
            {
                animator.SetBool(IsForwardMovingFiring, true);
                animator.SetBool(IsStandingFiring, false);
            }
            else
            {
                animator.SetBool(IsForwardMovingFiring, false);
                animator.SetBool(IsStandingFiring, true);
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            CreateTargetCircle(targetDirection, aimDirection.magnitude);
        }
        else
        {
            animator.SetBool(IsIdle, true);
            animator.SetBool(IsStandingFiring, false);
            animator.SetBool(IsForwardMovingFiring, false);
        }
    }

    private void CreateTargetCircle(Vector3 targetDirection, float distance) {
        float targetDistance = Mathf.Clamp(distance * maxDistance, 0f, maxDistance);

        Vector3 targetPosition = transform.position + targetDirection.normalized * targetDistance;

        targetPosition.y = 0.05f;

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
        Invoke("ResetReadyToShoot", 0.2f);
        targetPosition.y = 0f;

        float horizontalDistance = Vector3.Distance(transform.position, targetPosition);

        float gravity = Physics.gravity.y;

        float launchAngle = 20f * Mathf.Deg2Rad;

        float initialVelocity = -5f + Mathf.Sqrt(horizontalDistance * Mathf.Abs(gravity) / Mathf.Sin(2 * launchAngle));

        float velocityX = initialVelocity * Mathf.Cos(launchAngle);
        float velocityY = initialVelocity * Mathf.Sin(launchAngle);

        Vector3 launchVelocity = transform.forward * velocityX;
        launchVelocity.y = velocityY;

        Transform firingPoint = transform.Find("FiringPoint");
        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity);
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
