using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public float boostForce = 100f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 boostDirection = -transform.right;

            rb.AddForce(boostDirection.normalized * boostForce, ForceMode2D.Impulse);
        }
        
    }
}
