using UnityEngine;

public class GunHitScan : Range
{
   
    public override void AttackBegin(LayerMask playerMask)
    {

        //See if they have bullets
        if (ammoCount > 0 && currTotalBullets > 0 && shootRate <= shootTimer)
        {
            PlayShootAnim();
            SoundManager.instance.PlaySFX("pistol", 0.2f);

            //see if you hit an object
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, ~playerMask))
            {
                //damage enemy
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                dmg?.TakeDamage(damage);

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.ActivateElem((int)elem);

            }

            shootTimer = 0;
            ammoCount--;
            currTotalBullets--;
            GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        }
        else if (ammoCount <= 0 && currTotalBullets <= 0)
            SoundManager.instance.PlaySFX("gunEmpty", 0.5f);
        else if (ammoCount <= 0 && currTotalBullets > 0)
            SoundManager.instance.PlaySFX("gunClick", 0.5f);
    }

}
