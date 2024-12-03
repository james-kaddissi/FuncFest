using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockoutController : MonoBehaviour
{
    private Vector2 aimDirection;
    private float aimPower;
    private float rotationSpeed = 0.4f;
    private float launchDelay = 1f;

    private bool wasLaunched = false;

    private SpriteRenderer spriteRenderer;
    public Sprite closedSprite;
    public Sprite openSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if(wasLaunched) {
            spriteRenderer.sprite = openSprite;
            if(GetComponent<Rigidbody2D>().velocity.magnitude == 0) {
                wasLaunched = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        } else {
            spriteRenderer.sprite = closedSprite;
        }
        if(GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01f) {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void UpdateInput(Vector2 input)
    {
        if(input.magnitude != 0) {
            aimDirection = input;
        }
    }

    public void UpdatePower(float power)
    {
        aimPower = power;
    }

    public void Launch()
    {
        StartCoroutine(RotateAndLaunch());
    }

    private IEnumerator RotateAndLaunch()
    {
        Vector2 currentDirection = transform.up;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, -aimDirection);

        float timeElapsed = 0f;

        while (timeElapsed < rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(Vector3.forward, currentDirection), targetRotation, timeElapsed / rotationSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        yield return new WaitForSeconds(launchDelay);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = aimDirection.normalized * aimPower;
            wasLaunched = true;
        }
    }
}
