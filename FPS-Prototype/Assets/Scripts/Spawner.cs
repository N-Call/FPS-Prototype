using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [Header("Spawner Settings")]
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Vector3 offset;
    [SerializeField] Quaternion rotation;
    [SerializeField] float startDelay;
    [SerializeField] float rate;
    [SerializeField] int stopSpawningAfter;
    [SerializeField] bool deleteWhenFinished;

    float spawnTime;
    float elapsedTime;
    int objectsSpawned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnTime = rate;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (startDelay > 0.0f && elapsedTime < startDelay)
        {
            return;
        }

        if (stopSpawningAfter > 0 && objectsSpawned >= stopSpawningAfter)
        {
            if (deleteWhenFinished)
            {
                Destroy(gameObject);
            }

            return;
        }

        spawnTime += Time.deltaTime;
        if (rate > spawnTime)
        {
            return;
        }

        spawnTime = 0.0f;
        Instantiate(objectToSpawn, transform.position + offset, rotation);
        objectsSpawned++;
    }

}
