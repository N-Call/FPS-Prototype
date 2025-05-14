using UnityEngine;

public class GunHitScan : Range
{
   
    public override void AttackBegin(LayerMask playerMask)
    {

        //See if they have bullets
        if (ammoCount > 0 && shootRate <= shootTimer)
        {
            PlayShootAnim();
            SoundManager.instance.PlaySFX("pistol");

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
            GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
        }
    }

}
