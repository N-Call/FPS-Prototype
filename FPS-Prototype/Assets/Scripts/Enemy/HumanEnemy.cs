using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class HumanEnemy : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] Transform player;

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

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3 (playerDir.x, transform.position.y, transform.position.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
}
