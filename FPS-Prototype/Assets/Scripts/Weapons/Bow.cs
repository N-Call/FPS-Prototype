using System.Collections;
using UnityEngine;
public class Bow : Range
{
    [SerializeField] float chargeMaxRate;
    [SerializeField] float chargeRate;
    [Header("References")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject animArrow;
    [SerializeField] Damage[] projectils;

    Coroutine chargeCoroutine;

    private int projectileIndex;
    float currentCharge;

    public override void AttackBegin(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0 && currTotalBullets > 0 && shootTimer >= shootRate)
        {
            chargeCoroutine = StartCoroutine(Charge());
        }
        else if (ammoCount <= 0 && currTotalBullets <= 0)
            SoundManager.instance.PlaySFX("bowEmpty");
        else if (ammoCount <= 0 && currTotalBullets > 0)
            SoundManager.instance.PlaySFX("bowEmpty");
    }

    public override void AttackEnd(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0 && shootTimer >= shootRate && chargeCoroutine != null)
        {
            //Stop and clear the charge coroutine
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;

            SoundManager.instance.PlaySFX("bowRelease");
            PlayShootAnim();
            ammoCount--;
            currTotalBullets--;

            //see if out of ammo, if so change idle animation
            PlaySeconedIdle(ammoCount == 0 && ammoCap == 0);

            //Update Ammo Display
            GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        Damage dmg = Instantiate(projectils[projectileIndex], shootPos.position, transform.rotation);

        if (GameManager.instance.playerAbilities != null)
        {
            dmg.AddDamageAmount((int)(damage * currentCharge + GameManager.instance.playerAbilities.w2DmgMod));
        }
        else
        {
            dmg.AddDamageAmount((int)(damage * currentCharge));
        }

        dmg.AddSpeedAmount((int)(distance / chargeMaxRate * currentCharge));

        currentCharge = 0;
    }

    private void OnEnable()
    {
        PlaySeconedIdle(currTotalBullets == 0);
        PlayIdle();
        if(ammoCount == 0 && ammoCap != 0) { Reload(); }

        GameManager.instance?.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        GameManager.instance?.SetWeaponIcon(ammoIcon);
    }
    protected override void Update()
    {
        base.Update();
        SwapArrows();
    }

    public void SwapArrows()
    {
        if(GameManager.instance.playerAbilities == null)
        {
            return;
        }

        if (GameManager.instance.playerAbilities.w2Major == true && InputActionManager.instance.playerAim)
        {
            projectileIndex = (projectileIndex + 1 > projectils.Length - 1) ? 0 : projectileIndex + 1;
        }
    }

    IEnumerator Charge()
    {
        PlayChargeAnim();
        SoundManager.instance.PlaySFX("bowLoad");
        while (currentCharge < chargeMaxRate)
        {
            if (GameManager.instance.playerAbilities != null)
            {
                currentCharge += chargeRate + GameManager.instance.playerAbilities.w2SpeedMod;
            }
            else
            {
                currentCharge += chargeRate;
            }

            yield return new WaitForSeconds(chargeRate);
        }
    }
}
