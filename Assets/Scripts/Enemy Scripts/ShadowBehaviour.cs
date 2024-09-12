using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBehaviour : MonoBehaviour
{
    public float health;

    private ParticleSystem burnParticles;
    private float previousHealth;

    public void Awake()
    {
        burnParticles = GetComponentInChildren<ParticleSystem>();
        burnParticles.Stop();
        previousHealth = health;
    }

    private void Update()
    {
        if (previousHealth > health)
            burnParticles.Play();
        else
            burnParticles.Stop();

        previousHealth = health;
    }

    public void TakeDamage()
    {
        if (health > 0)
        {
           health -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
