using UnityEngine;

public class GunHitScan : Range
{
    public override void Attack(LayerMask playerMask, Camera camera)
    {
        Debug.Log(ammoCount);
        //See if they have bullets
        if (ammoCount > 0)
        {
            //see if you hit an object
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, distance, ~playerMask))
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                dmg?.TakeDamage(damage);
                GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
                //damage enemy
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.ActivateElem(element);
            }
        }
    }

}
