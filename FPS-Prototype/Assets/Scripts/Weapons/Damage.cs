using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType {DOT, moving, homing, stationary}
    enum ElementType {speed = 1, jump = 2, shield = 3}

    [HideInInspector] public Transform target;

    [Header("Resources")]
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider homingCollider;

    [Header("Damage Settings")]
    [SerializeField] DamageType damageType;
    [SerializeField] EAbility elem;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    [Header("Homing Settings")]
    [SerializeField] private float FOV;
    [SerializeField] private float homingRadius;
    [SerializeField] float chaseDist;
    [SerializeField] float smoothSpeed;
    [SerializeField] bool isTriggerHoming;

    [Header("Wall Bounce Settings")]
    [SerializeField] bool isWallBouncable;
    [SerializeField] int maxReflections;


    [Header("Damage Over Time Settings")]
    [SerializeField] bool canHeal;
    [SerializeField] private int dotHeal;
    [SerializeField] private float dotHealRate;
    [SerializeField] private int dotDamage;
    [SerializeField] private float dotDamageRate;

    private int reflectionCount;
    private Vector3 startPos;
    private Vector3 targetDir;
    private float angleToPlayer;
    private bool isDamaging;
    private bool stopChasing;
    private bool isLocked;
    private bool alreadyDestroyed;
    float DOTTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damageType == DamageType.moving || damageType == DamageType.homing)
        {
            Destroy(gameObject, destroyTime);
            rb.linearVelocity = transform.forward * speed;
            startPos = transform.position;
        }
    }

    private void Update()
    {
        if (damageType == DamageType.homing)
        {
            if (target != null)
            {
                if (!isTriggerHoming)
                {
                    return;
                }
                if (Vector3.Distance(target.position, transform.position) > chaseDist && !stopChasing && CanSeeTarget())
                {
                    isLocked = true;

                    Vector3 direction = target.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime );
                    rb.linearVelocity = transform.forward * speed;

                }
                else if (!CanSeeTarget() && !isLocked)
                {
                    rb.linearVelocity = transform.forward * Vector3.Distance(target.position, transform.position) * speed;
                    stopChasing = true;
                }
                else if (!stopChasing)
                {
                    stopChasing = true;
                    rb.linearVelocity = (target.position - transform.position) * speed;
                }
            }
            if (isWallBouncable)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1f) && reflectionCount <= maxReflections)
                {
                    reflectionCount++;
                    Vector3 reflectDir = Vector3.Reflect(transform.forward, hit.normal);
                    transform.forward = reflectDir;
                    rb.linearVelocity = transform.forward * speed;
                }
            }
        }

    }

    bool CanSeeTarget()
    {
        targetDir = (target.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(targetDir.x, 0, targetDir.z), transform.forward);
        

        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDir, out hit))
        {
            if (angleToPlayer <= FOV + (GameManager.instance.playerAbilities != null? GameManager.instance.playerAbilities.w2RateMod : 0) && hit.collider.gameObject == target.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void AddDamageAmount(int damage)
    {
        damageAmount += damage;
    }
    public void SetElement(int type)
    {
        elem = (EAbility)type;
    }

    public void AddSpeedAmount(int range)
    {
        speed += range;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || alreadyDestroyed) {return;}
        if (target == null && damageType == DamageType.homing) 
        {
            if (Vector3.Distance(startPos, transform.position) <= 1f)
            {
                return;
            }
            if (other.GetComponent<IDamage>() != null)
            {
                target = other.transform;
                homingCollider.radius = homingRadius;
                isTriggerHoming = true;
            }
            return; 
        }
        IDamage dmg = other.GetComponent<IDamage>();
        IOrb targ = other.GetComponent<IOrb>();

        if ((dmg != null || targ != null) && (damageType == DamageType.moving || damageType == DamageType.homing || damageType == DamageType.stationary))
        {
            dmg?.TakeDamage(damageAmount);
            targ?.ActivateEffect(GameManager.instance.playerScript, elem);
        }

        Break breakable = other.GetComponent<Break>();
        if (breakable != null)
        {
            Vector3 explosionOrigin = other.ClosestPoint(transform.position);
            breakable.Shatter(explosionOrigin);
        }

        if (damageType == DamageType.moving || isWallBouncable && reflectionCount > maxReflections || isWallBouncable && (dmg != null || targ != null))
        {
            GameObject.Destroy(gameObject);
            alreadyDestroyed = true;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger || alreadyDestroyed)
        {
            return;
        }

        Break breakable = other.GetComponent<Break>();
        if (breakable != null)
        {
            Vector3 explosionOrigin = other.ClosestPoint(transform.position);
            breakable.Shatter(explosionOrigin);
        }

        if (damageType == DamageType.homing && (Physics.Raycast(transform.position, transform.forward, homingRadius) ||
            Physics.Raycast(transform.position, transform.right, homingRadius) || Physics.Raycast(transform.position, -transform.right, homingRadius)))
        {
            IDamage dmg = other.GetComponent<IDamage>();
            IOrb targ = other.GetComponent<IOrb>();

            if (isWallBouncable && (dmg != null || targ != null))
            {
                dmg?.TakeDamage(damageAmount);
                targ?.ActivateEffect(GameManager.instance.playerScript, elem);
                GameObject.Destroy(gameObject);
                alreadyDestroyed = true;
            }else if (!isWallBouncable || reflectionCount > maxReflections)
            {
                dmg?.TakeDamage(damageAmount);
                targ?.ActivateEffect(GameManager.instance.playerScript, elem);
                GameObject.Destroy(gameObject);
                alreadyDestroyed = true;
            }
        }
        IDamage damage = other.GetComponent<IDamage>();
        if (isDamaging || damage == null || damageType != DamageType.DOT)
        {
            return;
        }
        else
        {
            if(canHeal && other.CompareTag("Player"))
            {
                StartCoroutine(HealOther(damage));
            }
            else
            {
                StartCoroutine(DamageOther(damage));
            }
        }
    }

    IEnumerator DamageOther(IDamage other)
    {
        other?.TakeDamage(dotDamage);
        isDamaging = true;
        
        yield return new WaitForSeconds(dotDamageRate);
        isDamaging = false;
    }

    IEnumerator HealOther(IDamage other)
    {
        other?.TakeDamage(-dotHeal);
        isDamaging = true;

        yield return new WaitForSeconds(dotHealRate);
        isDamaging = false;
    }

    public void SetDestroyTime(float time)
    {
        destroyTime = time;
    }

}
