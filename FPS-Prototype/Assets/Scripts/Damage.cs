using UnityEngine;
using System.Collections;
using System;


public class Damage : MonoBehaviour
{
    enum DamageType {DOT, moving, homing, stationary}

    [Header("Resources")]
    [SerializeField] Rigidbody rb;

    [Header("Damage Settings")]
    [SerializeField] DamageType damageType;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [Header("Damage Over Time Settings")]
    [SerializeField] private int dotDamage;
    [SerializeField] private int dotDamageRate;

    private bool isDamaging;
    bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damageType == DamageType.moving)
        {
            Destroy(gameObject, destroyTime);

            if (damageType == DamageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (damageType == DamageType.homing)
        {
        }
    }

    public void AddDamageAmount(int damage)
    {
        damageAmount += damage;
        Debug.Log("Damage Amount: " + damageAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.isTrigger))
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && (damageType == DamageType.moving || damageType == DamageType.homing || damageType == DamageType.stationary))
        {
            dmg.TakeDamage(damageAmount);
        }

        if (damageType == DamageType.moving || damageType == DamageType.homing)
        {
            gameObject.SetActive(false);
            isDead = true;
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

        StartCoroutine(DamageOther(damage));
    }

    IEnumerator DamageOther(IDamage other)
    {
        isDamaging = true;
        other.TakeDamage(dotDamage);
        yield return new WaitForSeconds(dotDamageRate);
        isDamaging = false;
    }

}
