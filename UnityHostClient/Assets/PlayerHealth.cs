using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject healthBar;
    

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(1.45f, 0.3f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(((float)currentHealth / (float)maxHealth) * 1.45f, 0.3f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die() {
        Destroy(gameObject);
    }
}
