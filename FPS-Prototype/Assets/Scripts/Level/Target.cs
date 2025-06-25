using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
{
    public enum ElementType { speed = 1, jump = 2, shield = 3 }

    [SerializeField] GameObject model;
    [SerializeField] GameObject explosionRadius;
    [SerializeField] float explosionSize;

    [Header("Element Type")]
    [SerializeField] public ElementType elem;

    [Header("Elements")]
    [SerializeField] public float speedElemMod;
    [SerializeField] float speedElemFOVMod;
    [SerializeField] float speedElemTime;
    [SerializeField] public float jumpElemMod;
    [SerializeField] float jumpElemTime;
    [SerializeField] int shieldElemMod;

    [Header("Health")]
    [SerializeField] int HP;
    [SerializeField] float respawnTime;

    float baseFOV;
    float respawnTimer;

    public bool buff;
    bool respawn;
    //bool isActive;

    Vector3 explosionScale;
    public bool enemyBuff; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explosionScale = new Vector3(explosionSize, explosionSize, explosionSize);
        explosionRadius.transform.localScale = explosionScale;
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
            StartCoroutine(ToggleExplosionVisual());
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

    void ToggleVisuals()
    {
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
            //isActive = false;
            //Debug.Log("Set inactive!");
            gameObject.SetActive(false);
        }
    }

    IEnumerator ToggleExplosionVisual()
    {
        explosionRadius.SetActive(!explosionRadius.activeSelf);
        yield return new WaitForSeconds(0.1f);
        explosionRadius.SetActive(!explosionRadius.activeSelf);
    }

    public void ApplySpeedElem()
    {
        if (buff)
        {
            SoundManager.instance.PlaySFX("powerUp");

            
            if (GameManager.instance.speedBuffTimer <= 0 || !GameManager.instance.playerScript.speedBuffed)
            {
                GameManager.instance.playerScript.AddModifier(speedElemMod);
                GameManager.instance.playerScript.SetBaseFOV(baseFOV + speedElemFOVMod);
                GameManager.instance.BuffSprintIcon(true);
                GameManager.instance.playerScript.particleSpMod.gameObject.SetActive(true);
                GameManager.instance.playerScript.speedBuffed = true;
            }
            
            GameManager.instance.speedBuffTimer = 0f;
        }

        else
        {
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.AddModifier(-speedElemMod);
            GameManager.instance.DeBuffSprintIcon(true);
            GameManager.instance.playerScript.SetBaseFOV(baseFOV - speedElemFOVMod);
            GameManager.instance.playerScript.speedBuffed = false;
        }
        float tempElem = (GameManager.instance.playerAbilities != null)? speedElemTime + GameManager.instance.playerAbilities.o1Dur : speedElemTime;
        GameManager.instance.SetElemParam((int)elem, buff, tempElem);
    }

    private void ApplyJumpElem()
    {
        if (buff)
        {
            SoundManager.instance.PlaySFX("powerUp");

            if (GameManager.instance.jumpBuffTimer <= 0 || !GameManager.instance.playerScript.jumpBuffed)
            {
                GameManager.instance.playerScript.AddModifier(0.0f, jumpElemMod);
                GameManager.instance.BuffJumpIcon(true);
                GameManager.instance.playerScript.particleJpMod.gameObject.SetActive(true);
                GameManager.instance.playerScript.jumpBuffed = true;
            }

                GameManager.instance.jumpBuffTimer = 0f;
        }

        else
        {
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.AddModifier(0.0f, -jumpElemMod);
            GameManager.instance.DeBuffJumpIcon(true);
            GameManager.instance.playerScript.particleJpMod.gameObject.SetActive(false);
            GameManager.instance.playerScript.jumpBuffed = false;
        }


        GameManager.instance.SetElemParam((int)elem, buff, jumpElemTime);

    }

    private void ApplyShieldElem()
    {
        if (buff)
        {
            
            SoundManager.instance.PlaySFX("powerUp");
            GameManager.instance.playerScript.SetShield(shieldElemMod);
        }
        else if (!buff)
        {
            
            SoundManager.instance.PlaySFX("debuff");
            GameManager.instance.playerScript.SetShield(-shieldElemMod);
        }

        GameManager.instance.playerScript.UpdatePlayerUI();
    }

    //public void ResetState()
    //{
    //    isActive = true;
    //    gameObject.SetActive(true);
    //}
}
