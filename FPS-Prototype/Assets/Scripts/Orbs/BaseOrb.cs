using System;
using System.Collections;
using UnityEngine;

public class BaseOrb : MonoBehaviour, IOrb, IDamage
{
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected float duration = 5f;
    [SerializeField] protected float modifier = 1f;
    [SerializeField] protected float respawnRate = 5f;
    [SerializeField] protected bool isDestroyable;

    protected bool major;
    protected EAbility curAbility;
    int currentHealth = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnValidate()
    {

    }
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateEffect(IEActivator activator, EAbility ability)
    {

        bool isPositive = (curAbility > EAbility.invensBoost)? (curAbility - EAbility.invensBoost) == (int)ability : (ability == curAbility);
        if (isPositive)
        {
            //call activator for buff
            SoundManager.instance.PlaySFX("powerUp");
            activator.ActivateBuffAbility(curAbility, duration, modifier);
        }
        else
        {
            //call Activator for deBuff
            SoundManager.instance.PlaySFX("debuff");
            activator.ActivateDebuffAbility(curAbility, duration, -modifier);
        }
    }

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("targetHit");
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (isDestroyable) { Destroy(gameObject); return; }
            StartCoroutine(Death());
        }
    }

    void Dead()
    {
        if(TryGetComponent<CapsuleCollider>(out var item))
        {
            item.enabled = false;
        }
        transform.GetChild(0).gameObject.SetActive(false);
    }


    void Alive()
    {
        if (TryGetComponent<CapsuleCollider>(out var item))
        {
            item.enabled = true;
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator Death()
    {
        Dead();
        yield return new WaitForSeconds(respawnRate);
        Alive();
        currentHealth = maxHealth;
    }
}
