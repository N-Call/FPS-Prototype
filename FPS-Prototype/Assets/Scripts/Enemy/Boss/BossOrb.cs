using System;
using System.Collections.Generic;
using UnityEngine;

public class BossOrb : MonoBehaviour, IBossDamagable
{
    [HideInInspector] public List<Transform> spawnLocations = new();

    [SerializeField] EAbility ability;
    [SerializeField] int health;
    public BossOrbSpawner orbSpawner;
    [SerializeField] Target regularOrb;

    public static event Action<EAbility> OnDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage()
    {
        health--;
        if(health == 0)
        {
            Death();
        }
    }

    private void Death()
    {
        if(gameObject == null)
        {
            return;
        }
        BossOrb.OnDeath?.Invoke(ability);

        int counter = UnityEngine.Random.Range(0, spawnLocations.Count);
        Instantiate(regularOrb, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
