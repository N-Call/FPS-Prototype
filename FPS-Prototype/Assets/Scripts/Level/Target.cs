using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
{
    public enum ElementType { speed = 1, jump = 2, shield = 3 }

    [SerializeField] GameObject model;
    [SerializeField] Collider explosionRadius;
    [SerializeField] GameObject explosionVisual;
    [SerializeField] float explosionSize;

    [Header("Element Type")]
    [SerializeField] public ElementType elem;

    [Header("Elements")]
    [SerializeField] float speedElemMod;
    [SerializeField] float speedElemFOVMod;
    [SerializeField] float speedElemTime;
    [SerializeField] float jumpElemMod;
    [SerializeField] float jumpElemTime;
    [SerializeField] int shieldElemMod;

    [Header("Health")]
    [SerializeField] int HP;

    float baseFOV;

    bool buff;
    Vector3 explosionScale;
    public bool enemyBuff; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explosionScale = new Vector3(explosionSize, explosionSize, explosionSize);
        explosionVisual.transform.localScale = explosionScale;
        SphereCollider explode = explosionRadius.GetComponent<SphereCollider>();
        explode.radius = explosionSize/2;
        baseFOV = Camera.main.fieldOfView;
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

    public void ActivateElem(int element)
    {
        // Check area for applicable targets. Need IElemental interface
        //Toggle explosion radius on and off to achieve ^^
        Debug.Log("Activating Element");

        if ((int)elem == element)
        {
            buff = true;
            GameManager.instance.playerScript.ApplyElement((int)elem, buff, speedElemMod, jumpElemMod);
        }
        else
        {
            buff = false;
            GameManager.instance.playerScript.ApplyElement((int)elem, buff, speedElemMod, jumpElemMod);
            
        }
        switch ((int)elem)
        {
            case 1:
                ApplySpeedElem();
                break;
            case 2:
                ApplyJumpElem();
                break;
            case 3:
                ApplyShieldElem();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IElemental affected = other.GetComponent<IElemental>();
        if (buff)
        {
            affected?.ApplyElement((int)elem, buff, speedElemMod, jumpElemMod);
        }
        else
        {
            affected?.ApplyElement((int)elem, buff, speedElemMod, jumpElemMod);
        }
    }

    IEnumerator InitiateExplosion()
    {
        explosionRadius.enabled = true;
        explosionVisual.SetActive(true);
        model.SetActive(false);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        //explosionRadius.enabled = false;
        //explosionVisual.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ApplySpeedElem()
    {
        if (buff && (GameManager.instance.speedBuffTimer > speedElemTime || GameManager.instance.speedBuffTimer == 0))
        {
            Debug.Log("Speed Buff");
            SoundManager.instance.PlaySFX("powerUp", 0.3f);
            GameManager.instance.BuffSprintIcon(speedElemTime);
            GameManager.instance.playerScript.AddModifier(speedElemMod);
            GameManager.instance.playerScript.SetBaseFOV(baseFOV + speedElemFOVMod);
            GameManager.instance.playerScript.particleSpMod.gameObject.SetActive(true);
            
        }
        else if (!buff)
        {
            Debug.Log("Speed Debuff");
            SoundManager.instance.PlaySFX("debuff", 0.4f);
            GameManager.instance.DeBuffSprintIcon(speedElemTime);
            GameManager.instance.playerScript.AddModifier(-1 / speedElemMod);
            GameManager.instance.playerScript.SetBaseFOV(baseFOV - speedElemFOVMod);
        }
        GameManager.instance.SetElemParam((int)elem, buff, speedElemTime);
    }
    private void ApplyJumpElem()
    {
        if (buff && (GameManager.instance.jumpBuffTimer > jumpElemTime || GameManager.instance.jumpBuffTimer == 0))
        {
            Debug.Log("Jump Buff");
            SoundManager.instance.PlaySFX("powerUp", 0.3f);
            GameManager.instance.BuffJumpIcon(jumpElemTime);
            GameManager.instance.playerScript.AddModifier(0.0f, jumpElemMod);
            GameManager.instance.playerScript.particleJpMod.gameObject.SetActive(true);
        }
        else if (!buff)
        {
            Debug.Log("Jump Debuff");
            SoundManager.instance.PlaySFX("debuff", 0.4f);
            GameManager.instance.DeBuffJumpIcon(jumpElemTime);
            GameManager.instance.playerScript.AddModifier(0.0f, -1 / jumpElemMod);
        }
        GameManager.instance.SetElemParam((int)elem, buff, jumpElemTime);
    }
    private void ApplyShieldElem()
    {
        if (buff)
        {
            Debug.Log("Shield Given");

            SoundManager.instance.PlaySFX("powerUp", 0.3f);

            GameManager.instance.playerScript.SetShield(shieldElemMod);
        }
        else if (!buff)
        {
            Debug.Log("Shield Taken");
            SoundManager.instance.PlaySFX("debuff", 0.4f);

            GameManager.instance.playerScript.SetShield(-shieldElemMod);
        }
        GameManager.instance.playerScript.UpdatePlayerUI();
    }
}
