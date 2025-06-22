using System.Collections;
using UnityEngine;

public class TurretEnemy : EnemyController
{

    [Header("Turret Settings")]
    [SerializeField] float shootDistance = 1000.0f;
    [SerializeField] Transform aimPos;
    [SerializeField] LayerMask layerToIgnore;
    [SerializeField] protected bool idleRotate = true;
    [SerializeField][Range(0, 90)] protected float maxPitch;
    [SerializeField][Range(0, 90)] protected float minPitch;

    Transform turretHead;
    Transform turretBarrel;

    float resetPitchTimer;

    protected override void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        originalColors = new Color[meshRenderers.Length];
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {

            originalColors[i] = meshRenderers[i].material.color;
        }
        
        maxHealth = currentHealth;
        colorOrig = model.material.color;
        turretHead = transform.Find("Head");
        turretBarrel = transform.Find("Head/CannonBase/Cannon");

        if (idleRotate)
        {
            StartCoroutine(Rotate());
        }

        if (addToEnemyCount)
        {
            GameManager.instance.UpdateEnemyCounter(1);
        }
    }

    protected override void Update()
    {
        shootTimer += Time.deltaTime;
        blinkTimer -= Time.deltaTime;
        EnemyFlash();
        canSeePlayer = CanSeePlayer();
    }
    void EnemyFlash()
    {

        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration) * 1.0f;
        float intensity = lerp * blinkIntensity;
        Color flashColor = Color.red * intensity;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (blinkTimer > 0)
            {
                meshRenderers[i].material.color = flashColor;
            }
            else
            {
                meshRenderers[i].material.color = originalColors[i];
            }

        }

    }
    protected override bool CanSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position + (Vector3.up * 0.5f)) - aimPos.position;
        angleToPlayer = Vector3.Angle(playerDir, aimPos.forward);
        

        RaycastHit hit;
        if (Physics.Raycast(aimPos.position, playerDir, out hit, shootDistance, ~layerToIgnore) && angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
        {
            Vector3 middlePlayerDir = (GameManager.instance.player.transform.position - (Vector3.up * 0.5f)) - turretHead.position;

            // Calculate the vertical angle from the direction
            float pitch = Vector3.SignedAngle(middlePlayerDir, new Vector3(middlePlayerDir.x, 0, middlePlayerDir.z), turretHead.right);
            pitch = Mathf.Clamp(-pitch, -maxPitch, minPitch);

            turretHead.LookAt(GameManager.instance.player.transform.position);
                
            Vector3 eulerAngles = turretHead.rotation.eulerAngles;
            eulerAngles.x = pitch;

            turretHead.rotation = Quaternion.Euler(eulerAngles);

            if (shootTimer >= shootRate)
            {
                Shoot();
                SoundManager.instance.PlaySFX("turretShot");
            }

            return true;
        }

        if (resetPitchTimer < 1.0f)
        {
            resetPitchTimer += Time.deltaTime;
            return false;
        }

        turretHead.eulerAngles = new Vector3(0, turretHead.eulerAngles.y, 0);
        resetPitchTimer = 0.0f;
        return false;
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    protected override void Shoot()
    {
        if (shootPos != null)
        {
            shootTimer = 0;
            Instantiate(bullet, shootPos.position, turretBarrel.rotation);
        }
    }

    private IEnumerator Rotate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (true)
        {
            if (!playerInRange || !canSeePlayer)
            {
                turretHead.Rotate(Vector3.up);
            }
            yield return wait;
        }
    }

    public void SetShootDistance(float distance)
    {
        shootDistance = distance;
    }

    public void SetBulletDestroyTime(float time)
    {
        Damage dmg = bullet.GetComponent<Damage>();
        if (dmg != null)
        {
            dmg.SetDestroyTime(time);
        }
    }

}
