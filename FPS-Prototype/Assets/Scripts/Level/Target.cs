using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
{
    enum ElementType { speed = 1, jump = 2, ammo = 3 }

    [SerializeField] GameObject artToDisable = null;

    [Header("Health")]
    [SerializeField] int HP;

    [Header("Element Type")]
    [SerializeField] ElementType elem;

    [Header("Speed Element")]
    [SerializeField][Range(0.01f, 999999)] float speedMod;
    [SerializeField] float speedModTime;

    [Header("Jump Element")]
    [SerializeField][Range(0.01f, 999999)] float jumpMod;
    [SerializeField] float jumpModTime;

    [Header("Reload Element")]
    [SerializeField][Range(1, 100)] float reloadPercentBuff;
    [SerializeField][Range(1, 100)] float reloadPercentDebuff;

    Collider targCollider;

    bool isSpeedBuffed;
    bool isJumpBuffed;
    bool isSpeedDebuffed;
    bool isJumpDebuffed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("targetHit");
        GameManager.instance.ToggleReticle();
        HP -= amount;

        if(HP <= 0)
        {
            targCollider.enabled = false;
            artToDisable.SetActive(false);
        }   
    }

    public void ActivateElem(int modifier)
    {
        Debug.Log("Activating Element");
        int result;
        int element = (int) elem;
        if (element >= modifier)
        {
            result = element - modifier;
        }
        else { result = modifier - element; }

        if (result > 0)
        {
            Debuff();
        }
        else
        {
            Buff();
        }
    }

    void Buff()
    {
        
        int element = (int)elem;
        switch (element)
        {
            case 1:
                if (isSpeedBuffed == true)
                {
                    break;
                }
                else
                {
                    isSpeedBuffed = true;
                    StartCoroutine(SpeedBuff());
                    
                }
                break;
            case 2:
                if (isJumpBuffed == true)
                {
                    break;
                }
                else
                {
                    isJumpBuffed = true;
                    StartCoroutine(JumpBuff());
                }
                break;
            case 3:
                AmmoBuff();
                
                break;
        }
    }

    void Debuff()
    {
        
        int element = (int)elem;

        switch (element)
        {
            case 1:
                if (isSpeedDebuffed == true)
                {
                    break;
                }
                else
                {
                    
                    isSpeedDebuffed = true;
                    StartCoroutine(SpeedDebuff());
                    
                }
                break;
            case 2:
                if (isJumpDebuffed == true)
                {
                    break;
                }
                else
                {
                    isJumpDebuffed = true;
                    StartCoroutine(JumpDebuff());
                    
                }
                break;
            case 3:
                AmmoDebuff();
                break;
        }
    }

    public IEnumerator SpeedBuff()
    {
        SoundManager.instance.PlaySFX("powerUp");
        GameManager.instance.playerScript.baseSpeed *= speedMod;
       

        yield return new WaitForSeconds(speedModTime);
        
        isSpeedBuffed = false;
        GameManager.instance.playerScript.baseSpeed /= speedMod;
        

        Destroy(gameObject);
    }

    public IEnumerator SpeedDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        GameManager.instance.playerScript.baseSpeed /= speedMod;
        
        yield return new WaitForSeconds(speedModTime);

       
        isSpeedDebuffed = false;
        GameManager.instance.playerScript.baseSpeed *= speedMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpBuff()
    {
        SoundManager.instance.PlaySFX("powerUp");
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

        isJumpBuffed = false;
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

        
        isJumpDebuffed = false;
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        Destroy(gameObject);
    }

    private void AmmoBuff() 
    {
        SoundManager.instance.PlaySFX("powerUp");
        for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
        {
            IReloadable rld = GameManager.instance.playerScript.weaponList[i].GetComponent<IReloadable>();
            rld?.SetAmmo(reloadPercentBuff);
        }

    }

    private void AmmoDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
        {
            IReloadable rld = GameManager.instance.playerScript.weaponList[i].GetComponent<IReloadable>();
            rld?.SetAmmo(-reloadPercentDebuff);
        }
    }

}
