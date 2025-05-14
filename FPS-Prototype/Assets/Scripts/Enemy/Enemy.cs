using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

    Transform turretBase;
    Transform turretHead;
    Transform turretBarrel;
    Transform turretLeftEye;
    Transform turretLeftEyebrow;
    Transform turretRightEye;
    Transform turretRightEyebrow;
    Color turretBaseColor;
    Color turretHeadColor;
    Color turretBarrelColor;
    Color turretLeftEyeColor;
    Color turretLeftEyebrowColor;
    Color turretRightEyeColor;
    Color turretRightEyebrowColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
        originalPosition = transform.position;
        colorOrig = model.material.color;
        GameManager.instance.UpdateEnemyCounter(1);

        if (isTurret)
        {
            turretHead = transform.Find("Head");
            turretHeadColor = turretHead.GetComponent<MeshRenderer>().material.color;

            StartCoroutine(Rotate());

            turretBase = transform.Find("Base");
            turretBaseColor = turretBase.GetComponent<MeshRenderer>().material.color;

            turretBarrel = transform.Find("Head/Barrel");
            turretBarrelColor = turretBarrel.GetComponent<MeshRenderer>().material.color;

            turretLeftEye = transform.Find("Head/Left Eye");
            turretLeftEyeColor = turretLeftEye.GetComponent<MeshRenderer>().material.color;

            turretLeftEyebrow = transform.Find("Head/Left Eye/Left Eyebrow");
            turretLeftEyebrowColor = turretLeftEyebrow.GetComponent<MeshRenderer>().material.color;

            turretRightEye = transform.Find("Head/Right Eye");
            turretRightEyeColor = turretRightEye.GetComponent<MeshRenderer>().material.color;

            turretRightEyebrow = transform.Find("Head/Right Eye/Right Eyebrow");
            turretRightEyebrowColor = turretRightEyebrow.GetComponent<MeshRenderer>().material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (!rangeIsTrigger)
        {
            playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
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
        if (!rangeIsTrigger && playerInRange && isTurret)
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
        if (!isTurret)
        {
            agent.isStopped = false;
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
            gameObject.SetActive(false);
            isDead = true;
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
        if (isTurret)
        {
            FlashTurretRed();
        }

        yield return new WaitForSeconds(0.05f);

        model.material.color = colorOrig;
        if (isTurret)
        {
            ReturnTurretColor();
        }
    }

    void FlashTurretRed()
    {
        turretBase.GetComponent<MeshRenderer>().material.color = Color.red;
        turretHead.GetComponent<MeshRenderer>().material.color = Color.red;
        turretBarrel.GetComponent<MeshRenderer>().material.color = Color.red;
        turretLeftEye.GetComponent<MeshRenderer>().material.color = Color.red;
        turretLeftEyebrow.GetComponent<MeshRenderer>().material.color = Color.red;
        turretRightEye.GetComponent<MeshRenderer>().material.color = Color.red;
        turretRightEyebrow.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void ReturnTurretColor()
    {
        turretBase.GetComponent<MeshRenderer>().material.color = turretBaseColor;
        turretHead.GetComponent<MeshRenderer>().material.color = turretHeadColor;
        turretBarrel.GetComponent<MeshRenderer>().material.color = turretBarrelColor;
        turretLeftEye.GetComponent<MeshRenderer>().material.color = turretLeftEyeColor;
        turretLeftEyebrow.GetComponent<MeshRenderer>().material.color = turretLeftEyebrowColor;
        turretRightEye.GetComponent<MeshRenderer>().material.color = turretRightEyeColor;
        turretRightEyebrow.GetComponent<MeshRenderer>().material.color = turretRightEyebrowColor;
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

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(playerDir.x, turretHead.position.y, playerDir.z));

        float time = 0;

        while (time < 1)
        {
            turretHead.rotation = Quaternion.Lerp(transform.rotation, lookRotation, time);

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
            if (agent.isStopped == false)
            {
                shootTimer = 0;
                Instantiate(bullet, shootPos.position, transform.rotation);
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
        if (!isTurret)
        {
            agent.isStopped = true;
        }

        playerAttackRange = false;
        currentHealth = maxHealth;

        if (isDead)
        { 
            gameObject.SetActive(true);
            isDead = false;
        }
    }

}
