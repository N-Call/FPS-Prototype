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
    [SerializeField] float respawnTime;

    float baseFOV;
    float respawnTimer;

    bool buff;
    bool respawn;

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
        if (respawn)
        {
            //Debug.Log("Respawning!");
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                //Debug.Log("Toggled!");
                respawn = false;
                respawnTimer = 0.0f;
                ToggleVisuals();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("targetHit");
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
        //Debug.Log("Activating Element");

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

    void ToggleVisuals()
    {
        explosionRadius.enabled = !explosionRadius.enabled;
        //explosionVisual.SetActive(!explosionVisual.activeSelf);

        CapsuleCollider collider = gameObject.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            collider.enabled = !collider.enabled;
        }

        model.SetActive(!model.activeSelf);
    }

    IEnumerator InitiateExplosion()
    {
        ToggleVisuals();
        yield return new WaitForSeconds(0.3f);

        if (respawnTime > 0.0f)
        {
            respawn = true;
        }
        else
        {
            //Debug.Log("Set inactive!");
            gameObject.SetActive(false);
        }
    }

    public void ApplySpeedElem()
    {
        if (buff)
        {
            SoundManager.instance.PlaySFX("powerUp");

            if (GameManager.instance.speedBuffTimer > speedElemTime || GameManager.instance.speedBuffTimer == 0)
            {
                GameManager.instance.playerScript.AddModifier(speedElemMod);
                GameManager.instance.playerScript.SetBaseFOV(baseFOV + speedElemFOVMod);
                GameManager.instance.BuffSprintIcon(true);
                GameManager.instance.playerScript.particleSpMod.gameObject.SetActive(true);
            }
        }

        else
        {
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.AddModifier(-1 / speedElemMod);
            GameManager.instance.DeBuffSprintIcon(true);
            GameManager.instance.playerScript.SetBaseFOV(baseFOV - speedElemFOVMod);
        }

        GameManager.instance.SetElemParam((int)elem, buff, speedElemTime);
    }

    private void ApplyJumpElem()
    {
        if (buff)
        {
            SoundManager.instance.PlaySFX("powerUp");

            if (GameManager.instance.jumpBuffTimer > jumpElemTime || GameManager.instance.jumpBuffTimer == 0) {
                GameManager.instance.playerScript.AddModifier(0.0f, jumpElemMod);
                GameManager.instance.BuffJumpIcon(true);
                GameManager.instance.playerScript.particleJpMod.gameObject.SetActive(true);
            }
        }

        else
        {
            //Debug.Log("Jump Debuff");
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.AddModifier(0.0f, -1 / jumpElemMod);
            GameManager.instance.DeBuffJumpIcon(true);
        }
        
        GameManager.instance.SetElemParam((int)elem, buff, jumpElemTime);
    }

    private void ApplyShieldElem()
    {
        if (buff)
        {
            //Debug.Log("Shield Given");
            SoundManager.instance.PlaySFX("powerUp");
            GameManager.instance.playerScript.SetShield(shieldElemMod);
        }
        else if (!buff)
        {
            //Debug.Log("Shield Taken");
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.SetShield(-shieldElemMod);
        }

        GameManager.instance.playerScript.UpdatePlayerUI();
    }

}
