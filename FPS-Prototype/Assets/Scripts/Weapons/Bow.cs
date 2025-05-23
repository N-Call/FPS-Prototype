using System.Collections;
using UnityEngine;

public class Bow : Range
{
    [SerializeField] float chargeMaxRate;
    [SerializeField] float chargeRate;
    [Header("References")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject animArrow;
    [SerializeField] Damage projectil;

    Coroutine chargeCoroutine;

    float currentCharge;

    public override void AttackBegin(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0 && shootTimer >= shootRate)
        {
            chargeCoroutine = StartCoroutine(Charge());
        }
    }

    public override void AttackEnd(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0 && shootTimer >= shootRate && chargeCoroutine != null)
        {
            //Stop and clear the charge coroutine
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;

            SoundManager.instance.PlaySFX("bowRelease", 1f);
            Shoot();
            ammoCount--;
            currentCharge = 0;

            //see if out of ammo, if so change idle animation
            PlaySeconedIdle(ammoCount == 0 && ammoCap == 0);

            //Update Ammo Display
            GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
        }
    }

    void Shoot()
    {
        PlayShootAnim();
        shootTimer = 0;
        Damage dmg = Instantiate(projectil, shootPos.position, transform.rotation);
        dmg.AddDamageAmount((int)(damage * currentCharge));
        dmg.AddSpeedAmount((int)(distance / chargeMaxRate * currentCharge));
    }

    private void OnEnable()
    {
        PlaySeconedIdle(ammoCap == 0 && ammoCount == 0);
        PlayIdle();


        GameManager.instance?.GlobalAmmoCount(ammoCount, ammoCap);
        GameManager.instance?.SetWeaponIcon(ammoIcon);
    }

    IEnumerator Charge()
    {
        PlayChargeAnim();
        SoundManager.instance.PlaySFX("bowLoad", 1f);
        while (currentCharge < chargeMaxRate)
        {
            currentCharge += chargeRate;
            yield return new WaitForSeconds(chargeRate);
        }
    }
}
