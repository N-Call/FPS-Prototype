using UnityEngine;

public class Spawner : MonoBehaviour
{

    [Header("Base Settings")]
    [SerializeField][Tooltip("The object to spawn")]
    GameObject objectToSpawn;

    [SerializeField][Tooltip("The positions the object can spawn at")]
    protected Transform[] spawnPositions;

    [Header("Spawn Settings")]
    [SerializeField][Tooltip("The amount of objects to spawn")]
    int spawnAmount;
    
    [SerializeField][Tooltip("The delay before the first object spawns when the player is in the trigger")]
    float spawnDelay;

    [SerializeField][Tooltip("The rate at which objects spawn")]
    float spawnRate;

    [Header("Enemy Spawn Settings")]
    [SerializeField]
    [Tooltip("Add enemies to enemy count when they spawn? False adds the enemies that will spawn, at start")]
    bool addToCountWhenSpawned;

    int currentIndex;
    int amountSpawned;

    float spawnDelayTimer;
    float spawnTimer;

    bool isEnemy;
    bool firstSpawned;
    bool hasPlayer;
    bool isActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isEnemy = objectToSpawn.CompareTag("Enemy");
        if (isEnemy && !addToCountWhenSpawned)
        {
            GameManager.instance.UpdateEnemyCounter(spawnAmount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (!hasPlayer || objectToSpawn == null || spawnPositions.Length == 0 || amountSpawned >= spawnAmount)
        {
            return;
        }

        if (!firstSpawned)
        {
            spawnTimer = spawnRate;
        }

        if (spawnDelayTimer < spawnDelay)
        {
            spawnDelayTimer += Time.deltaTime;
            return;
        }

        if (spawnTimer < spawnRate)
        {
            spawnTimer += Time.deltaTime;
            return;
        }

        Spawn();
    }

    void Spawn()
    {
        firstSpawned = true;
        
        int index = GetPositionIndex();
        Instantiate(objectToSpawn, spawnPositions[index].position, spawnPositions[index].rotation);

        if (isEnemy && addToCountWhenSpawned)
        {
            GameManager.instance.UpdateEnemyCounter(1);
        }

        amountSpawned++;
        spawnTimer = 0.0f;
    }

    protected virtual int GetPositionIndex()
    {
        int index = currentIndex;

        if (index == spawnPositions.Length)
        {
            currentIndex = 1;
            return 0;
        }

        currentIndex++;
        return index;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayer = false;
        }
    }

    public void DisableSpawner()
    {
        isActive = false;
    }

}
