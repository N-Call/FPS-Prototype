using UnityEngine;

public class Model2Mine : EnemyController, IElemental
{
    [Header("Mine Settings")]
    [SerializeField] float explosionDistance;
    [SerializeField] int damageAmount;

    [Header("Element Effects")]
    [SerializeField] protected float elementEffectTime;
    float elementSpeedMod;
    bool elemBuffed;
    bool elemDebuffed;
    float effectTimer;

    protected override void Start()
    {
        base.Start();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        originalColors = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {

            originalColors[i] = meshRenderers[i].material.color;
        }
    }

    protected override void Update()
    {
        base.Update();
        blinkTimer -= Time.deltaTime;
        EnemyFlash();


        //HandleElements();
        if (elemBuffed || elemDebuffed)
        {
            effectTimer += Time.deltaTime;
            if (effectTimer >= elementEffectTime)
            {
                EndElement();
            }
        }
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
        playerDir = (GameManager.instance.player.transform.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                if (hit.distance <= explosionDistance)
                {
                    Explode();
                }
                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }
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

    public void Explode()
    {
        SoundManager.instance.PlaySFX("mineExplosion");
        IDamage damage = GameManager.instance.player.GetComponent<IDamage>();
        damage?.TakeDamage(damageAmount);
        GameManager.instance.ToggleReticle();
        Destroy(gameObject);
        GameManager.instance.UpdateEnemyCounter(-1);
    }
    public void ApplyElement(int elem, bool buffStatus, float speedMod, float jumpMod)
    {
        elementSpeedMod = speedMod;
        if (buffStatus)
        {
            elemBuffed = true;
            agent.speed *= elementSpeedMod;
        }
        else if (!buffStatus)
        {
            elemDebuffed = true;
            agent.speed /= elementSpeedMod;
        }
        effectTimer = 0f;
    }

    void EndElement()
    {
        if (elemBuffed)
        {
            agent.speed -= elementSpeedMod;
            elemBuffed = false;
        }
        else if (elemDebuffed)
        {
            agent.speed += elementSpeedMod;
            elemDebuffed = false;
        }
    }
    public void ElementInverse()
    {
        throw new System.NotImplementedException();
    }
}


    

