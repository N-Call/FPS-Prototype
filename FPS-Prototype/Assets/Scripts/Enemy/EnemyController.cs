using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamage
{
    [SerializeField] protected Renderer model;
    [SerializeField] protected UnityEngine.AI.NavMeshAgent agent;
    //[SerializeField] Transform headPos;
    //[SerializeField] Animator anim;

    [SerializeField] protected int currentHealth;
    [SerializeField] protected int faceTargetSpeed;
    [SerializeField] protected int FOV;
    [SerializeField] protected int roamDist;
    [SerializeField] protected int roamPauseTime;
    [SerializeField] protected bool pause;
    //[SerializeField] int animTransSpeed;

    [SerializeField] protected Transform shootPos;
    [SerializeField] protected GameObject bullet;
    [SerializeField] protected float shootRate;
    [SerializeField] protected int damageAmount;

    protected Transform turretHead;
    protected Transform turretBarrel;
    
    protected Color colorOrig;
    protected Vector3 playerDir;
    public Vector3 originalPosition;
    Vector3 startingPos;


    protected float shootTimer;
    protected float angleToPlayer;
    protected float roamTimer;
    protected float stoppingDistanceOrig;
    protected float rotationAmount = 1.0f;
    protected int ticksPerSecond = 60;
    

    bool playerAttackRange;

    protected bool playerInRange;
    //public bool isDead;
    //public bool isRespawned;

    bool isPlayerDead;
    protected int maxHealth;
    public CustomTrigger RangeTrigger;
    public CustomTrigger ExplosionTrigger;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        GameManager.instance.UpdateEnemyCounter(1);
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
        playerDir = (GameManager.instance.player.transform.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }
    public void OnTriggerEnter(Collider other)
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

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (currentHealth <= 0)
        {
            GameManager.instance.UpdateEnemyCounter(-1);
            //gameObject.SetActive(false);
            //isDead = true;
            Destroy(gameObject);
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
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    protected virtual void Shoot()
    {
        if (shootPos != null)
        {
            shootTimer = 0;
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }

    //public void ResetEnemies()
    //{
    //    transform.position = originalPosition;
    //    currentHealth = maxHealth;

    //    if(isDead)
    //    {
    //        gameObject.SetActive(true);
    //        isDead = false;
    //        GameManager.instance.UpdateEnemyCounter(1);
    //        if (isRespawned == false)
    //        {
    //            GameManager.instance.UpdateEnemyCounter(-1);
    //            gameObject.SetActive(false);

    //        }
    //    }
    //}


}
