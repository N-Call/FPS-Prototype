using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MineController : MonoBehaviour, IDamage
{
   
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float sightRange;
    public LayerMask whatIsPlayer;

    private int playCount;

    Color colorOrig;
    Vector3 playerDir;

    bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.UpdateEnemyCounter(1);
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (playerInRange)
        {
            if (playCount == 0)
            {
                SoundManager.instance.PlaySFX("robotAngry");
                playCount++;
            }
            playerDir = (GameManager.instance.player.transform.position - transform.position);

            agent.SetDestination(GameManager.instance.player.transform.position);
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            faceTarget();
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
