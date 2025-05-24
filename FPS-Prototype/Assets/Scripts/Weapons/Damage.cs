using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType {DOT, moving, homing, stationary}
    enum ElementType {speed = 1, jump = 2, time = 3}

    [Header("Resources")]
    [SerializeField] Rigidbody rb;

    [Header("Damage Settings")]
    [SerializeField] DamageType damageType;
    [SerializeField] ElementType elem;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float chaseDist;

    [Header("Damage Over Time Settings")]
    [SerializeField] private int dotDamage;
    [SerializeField] private int dotDamageRate;

    private bool isDamaging;
    private bool stopChasing;

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
            if(Vector3.Distance(GameManager.instance.player.transform.position, transform.position) > chaseDist && !stopChasing)
            {
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position) * speed;
            }
            else if(!stopChasing)
            {
                stopChasing = true;
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position) * speed;
            }
        }
    }

    public void AddDamageAmount(int damage)
    {
        damageAmount += damage;
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
            SoundManager.instance.PlaySFX("turretDestroy", 1f);
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

        StartCoroutine(DamageOther(damage));
    }

    IEnumerator DamageOther(IDamage other)
    {
        isDamaging = true;
        other?.TakeDamage(dotDamage);
        yield return new WaitForSeconds(dotDamageRate);
        isDamaging = false;
    }

}
