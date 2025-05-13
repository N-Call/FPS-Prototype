using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.IntegerTime.RationalTime;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("Stats and Info")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int currentHealth;
    
    [Header("Sound Settings")]
    [SerializeField] string hitSoundFx;
    [SerializeField] string deathSoundFx;
    [SerializeField] string sightSoundFx;
    [SerializeField] string shootSoundFx;
    [Range(0f, 1f)]
    [SerializeField] float soundFxVolume;

    [Header("Targeting and Shooting")]
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int sightRange;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] bool pause;

    [Header("Turret Settings")]
    [SerializeField] Transform player;
    [SerializeField] Transform head;
    [SerializeField] Transform barrel;

    public bool isShooting;
    public bool isTurret;
    private float rotationAmount = 1.0f;
    private int ticksPerSecond = 60;
    public LayerMask whatIsPlayer;
    float shootTimer;
    Color colorOrig;
    Vector3 playerDir;

    bool playerInRange;
    bool playerAttackRange;
    private Coroutine LookCoroutine;
    public Vector3 originalPosition;
    int maxHealth;
    bool isDead;

    private PlayerRespawn playerRespawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        maxHealth = currentHealth;
        originalPosition = transform.position;
    }
    void Start()
    {
        StartCoroutine(Rotate());
        colorOrig = model.material.color;
        
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (playerInRange && !isTurret)
        {
            playerDir = (GameManager.instance.player.transform.position - transform.position);

            agent.SetDestination(GameManager.instance.player.transform.position);

            SoundManager.instance.PlaySFX(sightSoundFx);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
            
        }
        if (playerInRange && isTurret)
        {
            head.LookAt(player);
            head.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        }

        if (shootTimer >= shootRate && playerAttackRange && isShooting)
        {
            Shoot();
        }
    }
   
    public void TakeDamage(int amount)
    {
        
        currentHealth -= amount;
        SoundManager.instance.PlaySFX(hitSoundFx);
        if (!isTurret)
        {
           // agent.SetDestination(GameManager.instance.player.transform.position);
        }
        
        if (currentHealth <= 0)
        {
            
            SoundManager.instance.PlaySFX(deathSoundFx);
            gameObject.SetActive(false);
            isDead = true;
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAttackRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAttackRange = false;
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }
    private IEnumerator Rotate()
    {
        WaitForSeconds wait = new WaitForSeconds(1f / ticksPerSecond);
        if (isTurret) 
        {
            while (true)
            {
                if (!pause)
                {
                    head.Rotate(Vector3.up * rotationAmount);
                }
                yield return wait;
            }
        }
    }
    public void TrackPlayer()
    {
        if (isTurret)
        {
            if (LookCoroutine != null)
            {
                StopCoroutine(LookCoroutine);
            }

            LookCoroutine = StartCoroutine(turretTarget());
        }
    }
    IEnumerator turretTarget()
    {
        playerDir = (GameManager.instance.transform.position - transform.position);

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(playerDir.x, head.position.y, playerDir.z));

        float time = 0;

        while (time < 1)
        {
            head.rotation = Quaternion.Lerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * faceTargetSpeed;

            yield return null;
        }
    }
    public void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void Shoot()
    {
       if (!isTurret) 
       {
            shootTimer = 0;
            Instantiate(bullet, shootPos.position, transform.rotation);
            SoundManager.instance.PlaySFX(shootSoundFx);
       }
       if (isTurret)
        {
            shootTimer = 0;
            Instantiate(bullet, shootPos.position, barrel.rotation);
            SoundManager.instance.PlaySFX(shootSoundFx);
        }
    }
    public void ResetEnemies()
    {
       Debug.Log("reset");
       transform.position = originalPosition;
        agent.SetDestination(originalPosition);
        if (isDead)
        {
            currentHealth = 2;
            gameObject.SetActive(true);
            isDead = false;
        }
        
    }
}
