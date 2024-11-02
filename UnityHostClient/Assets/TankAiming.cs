using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmoothShakeFree;

public class TankAiming : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Transform canvasTransform;
    public Transform projectileSpawn;

    public void AimTank(Vector2 input) {
        if (input != Vector2.zero) 
        {
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void Fire() {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, transform.rotation, canvasTransform);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null) {
            projectileScript.Initialize(transform.parent.gameObject);
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            Vector2 direction = transform.up;
            rb.velocity = direction * projectileSpeed;
        }
        GameObject.Find("ShakeHolder").GetComponent<SmoothShake>().StartShake();
    }
}
