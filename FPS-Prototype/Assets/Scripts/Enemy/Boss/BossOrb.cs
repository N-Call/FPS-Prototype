using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class BossOrb : MonoBehaviour, IBossDamagable
{
    [HideInInspector] public List<Transform> spawnLocations = new();

    public SplineContainer spline;
    [SerializeField] EAbility ability;
    [SerializeField] int health;
    public BossOrbSpawner orbSpawner;
    [SerializeField] SplineMovement regularOrb;

    public static event Action<EAbility> OnDeath;

    private int minOrbsSpawn = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DifficultyManager.Instance == null) { return; }
        minOrbsSpawn = spline.Spline.Count - 3 + (int)DifficultyManager.Instance?.currentSettings.difficulty;
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

        int counter = UnityEngine.Random.Range(minOrbsSpawn, spline.Spline.Count);
        List<int> numbers = new List<int>();
        for (int i = 0; i <= counter; i++)
        {
            bool hasFailed = false;
            int index = UnityEngine.Random.Range(0, spline.Spline.Count);
            foreach(int n in numbers)
            {
                if(index == n)
                {
                    hasFailed = true; break;
                }
            }
            if (hasFailed) 
            {
                i--; continue;
            }
            numbers.Add(index);
            regularOrb.splineContainer = spline;
            regularOrb.splinIndex = index;
            GameObject currOrb = Instantiate(regularOrb, transform.position, transform.rotation).gameObject;
            orbSpawner.smallOrbs.Add(currOrb);
        }
        
        Destroy(gameObject);
    }
}
