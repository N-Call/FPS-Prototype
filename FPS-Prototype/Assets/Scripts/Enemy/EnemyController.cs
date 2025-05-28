using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour, IDamage
{
    [SerializeField] protected Renderer model;
    [SerializeField] protected int currentHealth;

    [SerializeField] GameObject parent;
    
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
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
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

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SoundManager.instance.PlaySFX("turretHit", 0.2f);
        if (currentHealth <= 0)

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
                GameManager.instance.UpdateEnemyCounter(-1);
                SoundManager.instance.PlaySFX("turretDestroy", 0.2f);
            }
            else
            {
                StartCoroutine(flashRed());
            }
            
    }
    protected IEnumerator flashRed()
    {    
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    protected void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    protected virtual void Shoot()
    {
        
    }
}
