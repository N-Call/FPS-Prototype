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
    [SerializeField] ElementType elem;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    [Header("Homing Settings")]
    [SerializeField] private float FOV;
    [SerializeField] private float homingRadius;
    [SerializeField] float chaseDist;
    [SerializeField] float smoothSpeed;
    [SerializeField] bool isTriggerHoming;


    [Header("Damage Over Time Settings")]
    [SerializeField] private int dotDamage;
    [SerializeField] private int dotDamageRate;

    private Vector3 startPos;
    private Vector3 targetDir;
    private float angleToPlayer;
    private float waitTime;
    private bool isDamaging;
    private bool stopChasing;
    private bool isLocked;
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
                waitTime += Time.deltaTime;
                if (!isTriggerHoming)
                {
                    return;
                }
                if (Vector3.Distance(target.position, transform.position) > chaseDist && !stopChasing && CanSeeTarget())
                {
                    isLocked = true;

                    Vector3 direction = target.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
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
        }

    }

    bool CanSeeTarget()
    {
        targetDir = (target.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(targetDir.x, 0, targetDir.z), transform.forward);
        Debug.DrawRay(transform.position, new Vector3(targetDir.x, 0, targetDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.gameObject == target.gameObject)
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
        elem = (ElementType)type;
    }

    public void AddSpeedAmount(int range)
    {
        speed += range;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger){return;}
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
        ITarget targ = other.GetComponent<ITarget>();

        if (dmg != null || targ != null && (damageType == DamageType.moving || damageType == DamageType.homing || damageType == DamageType.stationary))
        {
            dmg?.TakeDamage(damageAmount);
            targ?.ActivateElem((int)elem);
        }

        if (damageType == DamageType.moving || damageType == DamageType.homing)
        {
            GameObject.Destroy(gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if(damageType == DamageType.homing && (Physics.Raycast(transform.position, transform.forward, homingRadius) ||
            Physics.Raycast(transform.position, transform.right, homingRadius) || Physics.Raycast(transform.position, -transform.right, homingRadius)))
        {
            other.GetComponent<IDamage>()?.TakeDamage(damageAmount);
            other.GetComponent<ITarget>()?.ActivateElem((int)elem);
            GameObject.Destroy(gameObject);
        }
        IDamage damage = other.GetComponent<IDamage>();
        if (isDamaging || damage == null || damageType != DamageType.DOT)
        {
            return;
        }
        else
        {
            StartCoroutine(DamageOther(damage));
        }
    }

    IEnumerator DamageOther(IDamage other)
    {
        other?.TakeDamage(dotDamage);
        isDamaging = true;
        
        yield return new WaitForSeconds(dotDamageRate);
        isDamaging = false;
    }

    public void SetDestroyTime(float time)
    {
        destroyTime = time;
    }

}
