using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    public float power = 100f;
    public float maxPower = 100f;
    private Rigidbody2D rb;
    private bool isShooting = false;
    private Vector2 shotDirection;
    public Transform pivot;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isShooting)
        {
            rb.AddForce(shotDirection * power);
            isShooting = false;
        }
        pivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shotDirection.y, shotDirection.x) * Mathf.Rad2Deg - 90);
    }

    public void UpdateInput(Vector2 input)
    {
        if(input == Vector2.zero)
        {
            return;
        }
        shotDirection = input;
    }

    public void ShootBall()
    {
        rb.AddForce(shotDirection * power, ForceMode2D.Impulse);
        isShooting = true;
        Debug.Log("Ball shot");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Hole")) {
            gameObject.SetActive(false);
        }
    }
}
