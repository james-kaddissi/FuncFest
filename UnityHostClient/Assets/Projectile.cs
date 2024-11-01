using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject tankExplosionEffect;
    [SerializeField] private GameObject bulletExplosionEffect;
    private Transform canvasTransform;
    private GameObject ownerTank;
    private TeamTanksConnection ttc;

    public void Initialize(GameObject tank) {
        ownerTank = tank;
    }

    void Start()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
        ttc = GameObject.Find("TeamTanksConnection").GetComponent<TeamTanksConnection>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Tank" && other.gameObject != ownerTank) {
            string tankName = other.gameObject.name;
            int playerNumber = ExtractPlayerNumber(tankName); 
            ttc.PlayerDeath(playerNumber);
            Instantiate(tankExplosionEffect, other.transform.position, other.transform.rotation, canvasTransform);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Environment") {
            Instantiate(bulletExplosionEffect, transform.position, transform.rotation, canvasTransform);
            Destroy(gameObject);
        }
    }
    private int ExtractPlayerNumber(string tankName) {
        string[] parts = tankName.Split(new string[] { "TankPlayer" }, StringSplitOptions.None);
        if (parts.Length > 1 && int.TryParse(parts[1], out int playerNumber)) {
            return playerNumber;
        }
        return -1;
    }
}
