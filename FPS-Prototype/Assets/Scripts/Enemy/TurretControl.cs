using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform head;

    Color colorOrig;

    Vector3 playerDir;

    float shootTimer;

    bool playerInRange;

    private Coroutine LookCoroutine;

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
                       
            if (shootTimer >= shootRate)
            {
                shoot();
            }                               
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
    //detect when player is in Range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    //detect when player is out of Range
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int amount)
    {

        HP -= amount;

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

    IEnumerator faceTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(playerDir);

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(head.transform.rotation, lookRotation, time);

            time += Time.deltaTime * faceTargetSpeed;

            yield return null;
        }

        
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

}
