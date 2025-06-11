using System;
using UnityEngine;

public class BossOrb : MonoBehaviour, IBossDamagable
{


    [SerializeField] EAbility ability;
    [SerializeField] int health;

    public static event Action<EAbility> OnSomethingHappened;
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
        BossOrb.OnSomethingHappened?.Invoke(ability);
        Destroy(gameObject);
    }
}
