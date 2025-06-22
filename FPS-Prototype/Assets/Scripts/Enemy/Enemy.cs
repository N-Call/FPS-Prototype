using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy: MonoBehaviour, IDamage
{
    [Header("Stats and Info")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int currentHealth;
    
    [Header("Targeting and Shooting")]
    [SerializeField] int faceTargetSpeed;
    [SerializeField] protected int sightRange;
    [SerializeField] bool rangeIsTrigger;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] bool pause;
    [SerializeField] int damageAmount;

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
    public bool isDead;
    public bool isRespawned;

    
    Transform turretHead;
    Transform turretBarrel;
   
    Color turretHeadColor;
 

    Transform mineTop;
    Color mineTopColor;

    public Transform robotHead;
    Transform robotMouth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
        maxHealth = currentHealth;
        originalPosition = transform.position;
        colorOrig = model.material.color;

        if (isTurret)
        {
            turretHead = transform.Find("Head");
            turretHeadColor = turretHead.GetComponent<MeshRenderer>().material.color;

            StartCoroutine(Rotate());
        }
        robotMouth = transform.Find("RobotMouth.001");

    }

    // Update is called once per frame
    void Update()
    {


        shootTimer += Time.deltaTime;

        if (!rangeIsTrigger)
        {
            if (!isTurret && agent.isStopped)
            {
                playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            }

            else if (isTurret)
            {
                playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            }
        }

        if (!isTurret)
        {
            agent.isStopped = true;
        }

        if (playerInRange && !isTurret)
        {
            playerDir = (GameManager.instance.player.transform.position - transform.position);

            agent.isStopped = false;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }

        }
        if (playerInRange && isTurret)
        {

            turretHead.LookAt(GameManager.instance.player.transform);
            turretHead.eulerAngles = new Vector3(0, turretHead.eulerAngles.y, 0);

        }

        if (shootTimer >= shootRate && playerAttackRange && isShooting)
        {
            Shoot();
        }
    }

    public void TakeDamage(int amount)
    {

        if (isDead) { return; }
        GameManager.instance.ToggleReticle();
        if (!isTurret)
        {
            agent.isStopped = false;
            playerInRange = true;
        }
        if (isShooting && !isTurret)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
        currentHealth -= amount;
        

        SoundManager.instance.PlaySFX("turretHit");

        if (currentHealth <= 0)
        {
            GameManager.instance.UpdateEnemyCounter(-1);
            SoundManager.instance.PlaySFX("turretDestroy");
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
        if (isDead) { return; }

        if (other.CompareTag("Player"))
        {
            playerAttackRange = true;

            if (rangeIsTrigger)
            {
                playerInRange = true;
            }
        }

        if (!isTurret && !isShooting)
        {
            SoundManager.instance.PlaySFX("mineExplosion");   
            IDamage damage = other.GetComponent<IDamage>();
            damage?.TakeDamage(damageAmount);
            GameManager.instance.ToggleReticle();
            gameObject.SetActive(false);
            isDead = true;
            GameManager.instance.UpdateEnemyCounter(-1);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerAttackRange = false;

            if (rangeIsTrigger)
            {
                playerInRange = false;
            }
        }
    }

    void OnTriggerStay()
    {
        if (playerInRange && isTurret && rangeIsTrigger)
        {
            
            turretHead.LookAt(GameManager.instance.player.transform);
            turretHead.eulerAngles = new Vector3(0, turretHead.eulerAngles.y, 0);
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
        while (true)
        {
            if (!pause)
            {
                turretHead.Rotate(Vector3.up * rotationAmount);
            }
            yield return wait;
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
            
            {
                shootTimer = 0;
                Instantiate(bullet, shootPos.position, robotMouth.rotation);
                SoundManager.instance.PlaySFX("enemyShot");
            }
        }

        if (isTurret)
        {
            shootTimer = 0.0f;
            Instantiate(bullet, shootPos.position, turretBarrel.rotation);
            SoundManager.instance.PlaySFX("turretShot");
        }
    }

    public void ResetEnemies()
    {
        transform.position = originalPosition;
        if (!isTurret && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }

        playerAttackRange = false;
        currentHealth = maxHealth;

        if (isDead)
        {
            gameObject.SetActive(true);
            isDead = false;
            GameManager.instance.UpdateEnemyCounter(1);
            if (isRespawned == false)
            {
                GameManager.instance.UpdateEnemyCounter(-1);
                gameObject.SetActive(false);

            }
        }
    }

}
