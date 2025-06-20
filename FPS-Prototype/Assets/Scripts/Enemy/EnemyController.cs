using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour, IDamage, IEnemyReset
{
    public GameObject scrap;
    [SerializeField] int scrapAmount;

    [SerializeField] protected Renderer model;
    [SerializeField] protected int currentHealth;

    //[SerializeField] GameObject parent;

    [SerializeField] protected bool addToEnemyCount;

    //[SerializeField] Animator anim;
    //[SerializeField] int animTransSpeed;

    [Header("Agent Settings")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected int faceTargetSpeed;
    [SerializeField] protected int FOV;
    [SerializeField] protected int roamDist;
    [SerializeField] protected int roamPauseTime;
    [SerializeField] protected Transform headPos;

    [Header("Shooting Settings")]
    [SerializeField] protected Transform shootPos;
    [SerializeField] protected GameObject bullet;
    [SerializeField] protected float shootRate;

    [Header("Flash Settings")]
    [SerializeField] protected float blinkIntensity;
    [SerializeField] protected float blinkDuration;
    protected float blinkTimer;

    protected MeshRenderer[] meshRenderers;
    protected Color[] originalColors;

    protected Color colorOrig;
    protected Vector3 playerDir;
    public Vector3 originalPosition;
    Vector3 startingPos;

    protected float shootTimer;
    protected float angleToPlayer;
    protected float roamTimer;
    protected float stoppingDistanceOrig;
    
    protected float originalShootRate;
    protected int maxHealth;

    protected bool playerInRange;
    protected bool shootRateBuffed = false;
    protected bool canSeePlayer;
    protected bool isDead;
    bool canDropScrap = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
        //GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
       
        
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        isDead = false;

        if (addToEnemyCount)
        {
            GameManager.instance.UpdateEnemyCounter(1);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //SetAnimParameters();
        
        shootTimer += Time.deltaTime;
        


        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange && !CanSeePlayer())
        {
            CheckRoam();
        }
        else if (!playerInRange)
        {
            CheckRoam();
        }
    }

   
    //void SetAnimParameters()
    //{
    //    float agentSpeedCurr = agent.velocity.normalized.magnitude;
    //    float animSpeedCurr = anim.GetFloat("Speed");

    //    anim.SetFloat("Speed", Mathf.Lerp(animSpeedCurr, agentSpeedCurr, Time.deltaTime * animTransSpeed));
    //}

    void CheckRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            Roam();
        }
    }

    void Roam()
    {
        roamTimer = 0;

        agent.stoppingDistance = 0;

        Vector3 randPos = Random.insideUnitSphere * roamDist;
        randPos += startingPos;

        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

    }

    protected virtual bool CanSeePlayer()
    {
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    public void SetCanDropScrap(bool value)
    {
        canDropScrap = value;
    }
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SoundManager.instance.PlaySFX("turretHit");
        blinkTimer = blinkDuration;

        if (currentHealth <= 0)
        {
            isDead = true;
            GameManager.instance.UpdateEnemyCounter(-1);
            if (canDropScrap)
            {
                Instantiate(scrap, transform.position, Quaternion.identity);
            }
            ScrapPickup pickup =  scrap.GetComponent<ScrapPickup>();
            if (pickup != null)
            {
                pickup.scrapAmount = scrapAmount;
            }

            SoundManager.instance.PlaySFX("turretDestroy");

            Destroy(gameObject);
        }
    }

    //protected IEnumerator flashRed()
    //{
    //    // Set this object's color to red
    //    model.material.color = Color.red;

    //    List<Color> colors = new List<Color>();

    //    // Set children's colors to red
    //    foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
    //    {
    //        colors.Add(renderer.material.color);
    //        renderer.material.color = Color.red;
    //    }

    //    yield return new WaitForSeconds(0.05f);

    //    // Set this object's color back to its original
    //    model.material.color = colorOrig;

    //    int index = 0;
    //    foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
    //    {
    //        renderer.material.color = colors[index];
    //        index++;
    //    }
    //}

    protected void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    protected virtual void Shoot()
    {
        
    }

    public void ResetHealth()
    {
        if (!isDead)
        {
            currentHealth = maxHealth;
        }
    }
}
