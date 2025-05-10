using UnityEngine;

public class GunHitScan : Range
{
    public override void Attack(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0)
        {
            //see if you hit an object
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, ~playerMask))
            {
                //damage enemy
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                dmg?.TakeDamage(damage);

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.ActivateElem(element);
            }
            ammoCount--;
            GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
        }
    }

}
