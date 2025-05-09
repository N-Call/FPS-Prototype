using UnityEngine;
using System.Collections;
using System;

public class Damage : MonoBehaviour
{

    enum DamageType {DOT}

    [SerializeField] private DamageType damageType;

    [Header("Damage Over Time Settings")]
    [SerializeField] private int dotDamage;
    [SerializeField] private int dotDamageRate;

    private bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
