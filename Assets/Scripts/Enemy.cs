using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth;
    [SerializeField]
    private float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    public void takeDamage(float dmg) {
        currentHealth -= dmg;
    }
}
