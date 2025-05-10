using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretTargeting : MonoBehaviour, IDamage
{
    public float rotationAmount = 2.0f;
    public int ticksPerSecond = 60;
    public bool pause = false;

    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;

    Color colorOrig;

    private Coroutine LookCoroutine;

    public Transform player;

    float shootTimer;

    bool playerInRange;
    private void Start()
    {
        colorOrig = model.material.color;
        StartCoroutine(Rotate());
    }

    private void Update()
    {
        transform.LookAt(player);

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

        Quaternion lookRotation = Quaternion.LookRotation(playerDir);

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * faceTargetSpeed;

            yield return null;
        }
    }
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
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
}
