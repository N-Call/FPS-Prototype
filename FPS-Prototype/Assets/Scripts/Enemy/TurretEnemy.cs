using System.Collections;
using UnityEngine;

public class TurretEnemy : EnemyController
{

    [SerializeField] bool shouldRotate = true;
    [SerializeField][Range(0, 90)] float maxPitch;
    [SerializeField][Range(0, 90)] float minPitch;

    bool canSeePlayer;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToRespawn(this);
        maxHealth = currentHealth;
        colorOrig = model.material.color;
        turretHead = transform.Find("Head");
        turretBarrel = transform.Find("Head/CannonBase/Cannon");

        if (shouldRotate)
        {
            StartCoroutine(Rotate());
        }

        GameManager.instance.UpdateEnemyCounter(1);
    }

    protected override void Update()
    {
        shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            canSeePlayer = CanSeePlayer();
        }
    }

    protected override bool CanSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                Vector3 middlePlayerDir = playerDir;
                middlePlayerDir.y -= 0.5f;

                // Calculate the vertical angle from the direction
                float pitch = Vector3.SignedAngle(middlePlayerDir, new Vector3(playerDir.x, 0, playerDir.z), turretHead.right);
                pitch = Mathf.Clamp(-pitch, -maxPitch, minPitch);

                turretHead.LookAt(GameManager.instance.player.transform.position);
                
                Vector3 eulerAngles = turretHead.rotation.eulerAngles;
                eulerAngles.x = pitch;

                turretHead.rotation = Quaternion.Euler(eulerAngles);
                
                if (shootTimer >= shootRate)
                {
                    Shoot();
                    SoundManager.instance.PlaySFX("turretShot", 0.2f);
                }

                return true;
            }
        }

        return false;
    }

    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SoundManager.instance.PlaySFX("turretHit", 0.2f);

        if (currentHealth <= 0)
        {
            GameManager.instance.UpdateEnemyCounter(-1);
            //gameObject.SetActive(false);
            //isDead = true;
            Destroy(gameObject);
            SoundManager.instance.PlaySFX("turretDestroy", 0.2f);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            turretHead.eulerAngles = new Vector3(0, turretHead.eulerAngles.y, 0);
        }
    }

    protected override void Shoot()
    {
        if (shootPos != null)
        {
            shootTimer = 0;
            Debug.Log("shooting");
            Instantiate(bullet, shootPos.position, turretBarrel.rotation);
        }
    }

    private IEnumerator Rotate()
    {
        WaitForSeconds wait = new WaitForSeconds(1f / ticksPerSecond);
        while (true)
        {
            if (!pause && (!playerInRange || !canSeePlayer))
            {
                turretHead.Rotate(Vector3.up * rotationAmount);
            }
            yield return wait;
        }
    }

}
