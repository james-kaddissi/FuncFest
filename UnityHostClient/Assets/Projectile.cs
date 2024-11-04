using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SmoothShakeFree;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject tankExplosionEffect;
    [SerializeField] private GameObject bulletExplosionEffect;
    private Transform canvasTransform;
    private GameObject ownerTank;
    private TeamTanksConnection ttc;
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    private Image projectileImage;
    private Collider2D projectileCollider;

    public void Initialize(GameObject tank) {
        ownerTank = tank;
    }

    void Start()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
        ttc = GameObject.Find("TeamTanksConnection").GetComponent<TeamTanksConnection>();
        audioSource = GetComponent<AudioSource>();
        projectileImage = GetComponent<Image>(); 
        projectileCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Tank" && other.gameObject != ownerTank) {
            string tankName = other.gameObject.name;
            int playerNumber = ExtractPlayerNumber(tankName); 
            ttc.PlayerDeath(playerNumber);
            Instantiate(tankExplosionEffect, other.transform.position, other.transform.rotation, canvasTransform);
            Destroy(other.gameObject);
            HideAndPlayAudio(audioClips[1]);     
        }
        else if(other.gameObject.tag == "Environment") {
            Instantiate(bulletExplosionEffect, transform.position, transform.rotation, canvasTransform);
            HideAndPlayAudio(audioClips[0]);
            
        }

        GameObject.Find("ShakeHolder").GetComponent<SmoothShake>().StartShake();
    }

    private void HideAndPlayAudio(AudioClip clip) {
        if (projectileImage != null) {
            projectileImage.enabled = false;
        }
        if (projectileCollider != null) {
            projectileCollider.enabled = false;
        }
        
        audioSource.clip = clip;
        audioSource.Play();
        
        StartCoroutine(DestroyAfterAudio(clip.length));
    }

    private IEnumerator DestroyAfterAudio(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private int ExtractPlayerNumber(string tankName) {
        string[] parts = tankName.Split(new string[] { "TankPlayer" }, StringSplitOptions.None);
        if (parts.Length > 1 && int.TryParse(parts[1], out int playerNumber)) {
            return playerNumber;
        }
        return -1;
    }
}
