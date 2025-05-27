using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IDamage
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

    //Transform turretBase;
    Transform turretHead;
    Transform turretBarrel;
    //Transform turretLeftEye;
    //Transform turretLeftEyebrow;
    //Transform turretRightEye;
    //Transform turretRightEyebrow;
    //Color turretBaseColor;
    Color turretHeadColor;
    //Color turretBarrelColor;
    //Color turretLeftEyeColor;
    //Color turretLeftEyebrowColor;
    //Color turretRightEyeColor;
    //Color turretRightEyebrowColor;

    Transform mineTop;
    Color mineTopColor;

    public Transform robotHead;
    Transform robotMouth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
        originalPosition = transform.position;
        colorOrig = model.material.color;

        if (isTurret)
        {
            turretHead = transform.Find("Head");
            turretHeadColor = turretHead.GetComponent<MeshRenderer>().material.color;

            StartCoroutine(Rotate());

            //turretBase = transform.Find("Base");
            //turretBaseColor = turretBase.GetComponent<MeshRenderer>().material.color;

            //turretBarrel = transform.Find("Head/CannonBase/Cannon");
            //turretBarrelColor = turretBarrel.GetComponent<MeshRenderer>().material.color;

            //turretLeftEye = transform.Find("Head/Left Eye");
            //turretLeftEyeColor = turretLeftEye.GetComponent<MeshRenderer>().material.color;

            //turretLeftEyebrow = transform.Find("Head/Left Eye/Left Eyebrow");
            //turretLeftEyebrowColor = turretLeftEyebrow.GetComponent<MeshRenderer>().material.color;

            //turretRightEye = transform.Find("Head/Right Eye");
            //turretRightEyeColor = turretRightEye.GetComponent<MeshRenderer>().material.color;

            //turretRightEyebrow = transform.Find("Head/Right Eye/Right Eyebrow");
            //turretRightEyebrowColor = turretRightEyebrow.GetComponent<MeshRenderer>().material.color;
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
            Debug.Log("looking");

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
        

        SoundManager.instance.PlaySFX("turretHit", 0.1f);

        if (currentHealth <= 0)
        {
            GameManager.instance.UpdateEnemyCounter(-1);
            SoundManager.instance.PlaySFX("turretDestroy", 0.2f);
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
            SoundManager.instance.PlaySFX("mineExplosion", 0.3f);
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
            Debug.Log("tracking");
            turretHead.LookAt(GameManager.instance.player.transform);
            turretHead.eulerAngles = new Vector3(0, turretHead.eulerAngles.y, 0);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        //if (isTurret)
        //{
        //    FlashTurretRed();
        //}


        yield return new WaitForSeconds(0.05f);

        model.material.color = colorOrig;
        //if (isTurret)
        //{
        //    ReturnTurretColor();
        //}

    }



    //void FlashTurretRed()
    //{
    //    turretBase.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretHead.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretBarrel.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretLeftEye.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretLeftEyebrow.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretRightEye.GetComponent<MeshRenderer>().material.color = Color.red;
    //    turretRightEyebrow.GetComponent<MeshRenderer>().material.color = Color.red;
    //}

    //void ReturnTurretColor()
    //{
    //    turretBase.GetComponent<MeshRenderer>().material.color = turretBaseColor;
    //    turretHead.GetComponent<MeshRenderer>().material.color = turretHeadColor;
    //    turretBarrel.GetComponent<MeshRenderer>().material.color = turretBarrelColor;
    //    turretLeftEye.GetComponent<MeshRenderer>().material.color = turretLeftEyeColor;
    //    turretLeftEyebrow.GetComponent<MeshRenderer>().material.color = turretLeftEyebrowColor;
    //    turretRightEye.GetComponent<MeshRenderer>().material.color = turretRightEyeColor;
    //    turretRightEyebrow.GetComponent<MeshRenderer>().material.color = turretRightEyebrowColor;
    //}

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
                SoundManager.instance.PlaySFX("enemyShot", 0.2f);
            }
        }

        if (isTurret)
        {
            shootTimer = 0.0f;
            Instantiate(bullet, shootPos.position, turretBarrel.rotation);
            SoundManager.instance.PlaySFX("turretShot", 0.2f);
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
