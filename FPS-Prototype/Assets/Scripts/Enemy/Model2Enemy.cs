using UnityEngine;

public class Model2Enemy : EnemyController
{

    [Header("Element Effects")]
    [SerializeField] protected float elementEffectTime;
    float elementJumpMod;
    bool elemBuffed;
    bool elemDebuffed;
    float effectTimer;

    [Header("Model 2 Settings")]
    [SerializeField] Transform shootPosL;
    [SerializeField] Transform shootPosR;

    bool canShoot;
    bool leftShot;
    protected override void Start()
    {
        base.Start();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        //grab mesh material of all the components of the enemy
        originalColors = new Color[meshRenderers.Length];
        Debug.Log("Found" + meshRenderers.Length + " renderers on" + gameObject.name);
        for (int i = 0; i < meshRenderers.Length; i++)
        {

            originalColors[i] = meshRenderers[i].material.color;
        }
    }
    protected override void Update()
    {
        canShoot = false;
        base.Update();

        shootTimer += Time.deltaTime;
        blinkTimer -= Time.deltaTime;
        EnemyFlash();

        if (canShoot && shootTimer >= shootRate)
        {
            if (leftShot)
            {
                LShoot();
            }

            else
            {
                RShoot();
            }

            leftShot = !leftShot;
            shootTimer = 0f;
            SoundManager.instance.PlaySFX("enemyShot");
        }

        //HandleElements();
        //if (elemBuffed || elemDebuffed)
        //{
        //    effectTimer += Time.deltaTime;
        //    if (effectTimer >= elementEffectTime)
        //    {
        //        EndElement();
        //    }
        //}
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
                Debug.Log("Enemy flashed");
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
        playerDir = (GameManager.instance.player.transform.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        //Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit) && angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
        {
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
            canShoot = true;
            agent.stoppingDistance = stoppingDistanceOrig;
            return true;
        }

        canShoot = false;
        agent.stoppingDistance = 0;
        return false;    
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (currentHealth > 0.0f)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }

    void LShoot()
    {
        if (shootPosL != null)
        {
            Instantiate(bullet, shootPosL.position, shootPosL.rotation);
        }
    }

    void RShoot()
    {
        if (shootPosR != null)
        {
            Instantiate(bullet, shootPosR.position, shootPosR.rotation);
        }
    }

    //void HandleElements()
    //{
    //    if (elemBuffed)
    //    {
    //        agent.speed += elementJumpMod;
    //    }
    //    if (elemDebuffed)
    //    {
    //        agent.speed -= elementJumpMod;
    //    }
    //}

    //void EndElement()
    //{
    //    if (elemBuffed)
    //    {
    //        agent.baseOffset -= elementJumpMod;
    //        elemBuffed = false;
    //    }
    //    else if (elemDebuffed)
    //    {
    //        agent.baseOffset += elementJumpMod;
    //        elemDebuffed = false;
    //    }
    //}

    //public void ApplyElement(int elem, bool buffStatus, float speedMod, float jumpMod)
    //{
    //    elementJumpMod = jumpMod;
    //    if (buffStatus)
    //    {
    //        elemBuffed = true;
    //        agent.baseOffset *= elementJumpMod;
    //    }
    //    else if (!buffStatus)
    //    {
    //        elemDebuffed = true;
    //        agent.baseOffset /= elementJumpMod;
    //    }
    //    effectTimer = 0f;
    //}

    //public void ElementInverse()
    //{
    //    // Not yet implemented
    //}
}
