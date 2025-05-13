using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TurretControl : MonoBehaviour, IDamage
{
    private float rotationAmount = 1.0f;
    private int ticksPerSecond = 60;

    [Header("Targeting Settings")]
    [SerializeField] Transform player;
    [SerializeField] Transform head;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] bool pause;

    [Header("Shooting and Damage")]
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform barrel;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;

    Color colorOrig;

    private Coroutine LookCoroutine;
    Vector3 originalPosition;

    float shootTimer;

    bool playerInRange;
    private void Start()
    {
        colorOrig = model.material.color;
        StartCoroutine(Rotate());
        originalPosition = transform.position;
    }

    private void Update()
    {
       
        
        //head.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);

        shootTimer += Time.deltaTime;

        //Check if Player is in Range before moving
      
    }

    private IEnumerator Rotate()
    {
        
        WaitForSeconds wait = new WaitForSeconds(1f / ticksPerSecond);

        while (true)
        {
            if (!pause)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.rotation.x, 0, transform.rotation.z));
                head.Rotate(Vector3.up * rotationAmount);
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

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3 (playerDir.x, playerDir.y, playerDir.z));

        float time = 0;

        while (time < 1)
        {
            head.rotation = Quaternion.Lerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * faceTargetSpeed;

            yield return null;
        }
    }
     public void OnTriggerStay(Collider other)
    {
        playerInRange = true;
        IDamage dmg = other.GetComponent<IDamage>();

        head.LookAt(player);

        if (dmg != null && shootTimer >= shootRate)
        {
            shoot();
        }
    }
    //detect when player is out of Range
    //public void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        playerInRange = false;
    //    }
    //}

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("turretHit");

        HP -= amount;

            if (HP <= 0)
        {
            SoundManager.instance.PlaySFX("turretDestroy");
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
        Instantiate(bullet, shootPos.position, barrel.rotation);
        SoundManager.instance.PlaySFX("turretShot");
        playerInRange = false;
    }


}
