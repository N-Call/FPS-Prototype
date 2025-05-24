using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
{
    enum ElementType { speed = 1, jump = 2, shield = 3 }

    [SerializeField] Collider explosionRadius;
    [SerializeField] GameObject explosionVisual;

    [Header("Health")]
    [SerializeField] int HP;

    [Header("Element Type")]
    [SerializeField] ElementType elem;

    bool buff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("targetHit", 0.3f);
        GameManager.instance.ToggleReticle();
        HP -= amount;

        if(HP <= 0)
        {
            StartCoroutine(InitiateExplosion());
        }   
    }

    public void ActivateElem(int modifier)
    {
        // Check area for applicable targets. Need IElemental interface
        //Toggle explosion radius on and off to achieve ^^
        Debug.Log("Activating Element");

        if ((int)elem == modifier)
        {
            buff = true;
        }
        else
        {
            buff = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Worked");
        IElemental affected = other.GetComponent<IElemental>();
        if (buff)
        {
            affected.ElementBuff((int) elem);
        }
        else
        {
            affected.ElementDebuff((int) elem);
        }
    }
    IEnumerator InitiateExplosion()
    {
        explosionRadius.enabled = true;
        explosionVisual.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        explosionRadius.enabled = false;
        explosionVisual.SetActive(false);
        gameObject.SetActive(false);
    }
}
