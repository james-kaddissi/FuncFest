using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject tankExplosionEffect;
    [SerializeField] private GameObject bulletExplosionEffect;
    private Transform canvasTransform;

    void Start()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Tank") {
            Instantiate(tankExplosionEffect, other.transform.position, other.transform.rotation, canvasTransform);
            Instantiate(bulletExplosionEffect, transform.position, transform.rotation, canvasTransform);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Environment") {
            Instantiate(bulletExplosionEffect, transform.position, transform.rotation, canvasTransform);
            Destroy(gameObject);
        }
    }
}
