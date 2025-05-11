using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using static Unity.IntegerTime.RationalTime;

public class HumanEnemy : MonoBehaviour, IDamage
{
    private float rotationAmount = 2.0f;
    private int ticksPerSecond = 60;

    [Header("Targeting Settings")]
    [SerializeField] Transform player;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] bool pause;

    [Header("Shooting and Damage Settings")]
    [SerializeField] int HP;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    private Coroutine LookCoroutine;

    Color colorOrig;

    Vector3 playerDir;

    float shootTimer;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        shootTimer += Time.deltaTime;

        //Check if Player is in Range before moving
        if (playerInRange)
        {
           
            playerDir = (GameManager.instance.player.transform.position - transform.position);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if(shootTimer >= shootRate)
            {
                shoot();
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    private IEnumerator Rotate()
    {
        WaitForSeconds wait = new WaitForSeconds(1f / ticksPerSecond);

        while (true)
        {
            if (!pause)
            {
                transform.Rotate(Vector3.up * rotationAmount);
            }
            yield return wait;
        }
    }
    public void TrackPlayer()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(faceTarget());
    }
    IEnumerator faceTarget()
    {
        playerDir = (GameManager.instance.transform.position - transform.position);

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * faceTargetSpeed;

            yield return null;
        }
    }
    //detect when player is in Range
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            {
                playerInRange = true;   
            }
    }
    //detect when player is out of Range
    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange=false;
        }
    }

    public void TakeDamage(int amount)
    {
        
        HP -= amount;
        SoundManager.instance.PlaySFX("playerHurt");

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {            
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
        SoundManager.instance.PlaySFX("enemyShot");
    }
}
