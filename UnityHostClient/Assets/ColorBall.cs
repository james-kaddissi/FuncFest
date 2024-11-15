using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBall : MonoBehaviour
{
    public Material[] materials;
    int randomIndex;
    public GameObject explosionPrefab;
    public GameObject paintSplatterPrefab;

    private float timeSinceInstantiation;
    private bool isInstantiated;

    void Start() {
        if (materials.Length > 0) {
            Renderer renderer = GetComponent<Renderer>();
            randomIndex = Random.Range(0, materials.Length);
            renderer.material = materials[randomIndex];
        }

        timeSinceInstantiation = 0f;
        isInstantiated = true;
    }

    void Update() {
        if (isInstantiated) {
            timeSinceInstantiation += Time.deltaTime;
            if (timeSinceInstantiation > 0.1f) {
                isInstantiated = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="Player" && isInstantiated) {
            return;
        }
        if(other.gameObject.tag=="Player") {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
        }
        GameObject instance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        var mainModule = ps.main;
        mainModule.startColor = materials[randomIndex].color;
        if (other.gameObject.CompareTag("Obstacle"))
        {
            ContactPoint contact = other.contacts[0];
            Vector3 hitPoint = contact.point;

            Vector3 spawnPosition = hitPoint + contact.normal * 0.05f; 

            GameObject splatter = Instantiate(paintSplatterPrefab, spawnPosition, Quaternion.identity);

            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            splatter.transform.rotation = surfaceRotation * Quaternion.Euler(90f, 0f, 0f);

            Color assignColor = materials[randomIndex].color;
            splatter.GetComponent<SpriteRenderer>().color = assignColor;

            float randomScale = Random.Range(0.02f, 0.08f);
            splatter.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            ContactPoint contact = other.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 spawnPosition = hitPoint + Vector3.up * 0.05f;

            GameObject splatter = Instantiate(paintSplatterPrefab, spawnPosition, Quaternion.identity);

            float randomRotation = Random.Range(0f, 360f);
            splatter.transform.rotation = Quaternion.Euler(90f, 0f, randomRotation);

            Color assignColor = materials[randomIndex].color;
            splatter.GetComponent<SpriteRenderer>().color = assignColor;

            float randomScale = Random.Range(0.1f, 0.4f);
            splatter.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        }
        Destroy(instance, 1f);
        Destroy(gameObject);
    }
}
