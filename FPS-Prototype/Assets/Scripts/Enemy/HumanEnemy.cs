using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using static Unity.IntegerTime.RationalTime;
using NUnit.Framework;
using System.ComponentModel.Design;

public class HumanEnemy : MonoBehaviour, IDamage
{
    [Header("Stats and Info")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;

    [Header("Targeting and Shooting")]
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    
    

    Color colorOrig;

    Vector3 playerDir;

    float shootTimer;

    bool playerInRange;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.UpdateEnemyCounter(+1);
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
        shootTimer += Time.deltaTime;

        if (playerInRange)
       {

            playerDir = (GameManager.instance.player.transform.position - transform.position);

            agent.SetDestination(GameManager.instance.player.transform.position);

            if (shootTimer >= shootRate)
            {
                shoot();
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
            
        }
    }

    

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
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
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
        SoundManager.instance.PlaySFX("enemyShot");
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        SoundManager.instance.PlaySFX("robotHit");

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            SoundManager.instance.PlaySFX("turretDestroy");
            GameManager.instance.UpdateEnemyCounter(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }
}
