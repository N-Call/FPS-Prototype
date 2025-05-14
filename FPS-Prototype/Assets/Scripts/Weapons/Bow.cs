using System.Collections;
using UnityEngine;

public class Bow : Range
{
    [SerializeField] float chargeMaxRate;
    [SerializeField] float chargeRate;
    [Header("References")]
    [SerializeField] Transform shootPos;
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

            SoundManager.instance.PlaySFX("bowRelease");
            Shoot();
            ammoCount--;
            currentCharge = 0;

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

    IEnumerator Charge()
    {
        PlayChargeAnim();
        SoundManager.instance.PlaySFX("bowLoad");
        while (currentCharge < chargeMaxRate)
        {
            currentCharge += chargeRate;
            yield return new WaitForSeconds(chargeRate);
        }
    }
}
