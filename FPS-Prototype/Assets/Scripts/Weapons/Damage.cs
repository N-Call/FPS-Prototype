using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType {DOT, moving, homing, stationary}
    enum ElementType {speed = 1, jump = 2, shield = 3}

    [Header("Resources")]
    [SerializeField] Rigidbody rb;

    [Header("Damage Settings")]
    [SerializeField] DamageType damageType;
    [SerializeField] ElementType elem;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] private float FOV;
    [SerializeField] float chaseDist;

    [Header("Damage Over Time Settings")]
    [SerializeField] private int dotDamage;
    [SerializeField] private int dotDamageRate;

    private Vector3 playerDir;
    private float angleToPlayer;
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

            if (damageType == DamageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    private void Update()
    {
        if (damageType == DamageType.homing)
        {
            if(Vector3.Distance(GameManager.instance.player.transform.position, transform.position) > chaseDist && !stopChasing && CanSeePlayer())
            {
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position) * speed;
                transform.LookAt(GameManager.instance.player.transform.position);
                isLocked = true;
            }else if (!CanSeePlayer() && !isLocked)
            {
                rb.linearVelocity = transform.forward * Vector3.Distance(GameManager.instance.player.transform.position, transform.position) * speed;
                stopChasing = true;
            }
            else if(!stopChasing)
            {
                stopChasing = true;
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position) * speed;
            }
        }

    }

    bool CanSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
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
        if (other.isTrigger)
        {
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

        if (damageType == DamageType.homing)
        {
            SoundManager.instance.PlaySFX("turretDestroy");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
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
