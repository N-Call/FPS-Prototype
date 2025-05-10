using UnityEngine;

public class GunHitScan : Range
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public  override void Attack(LayerMask playerMask, Camera camera)
    {
        Debug.Log(ammoCount);
        //See if they have bullets
        if (ammoCount > 0)
        {
            //see if you hit an object
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, distance, ~playerMask))
            {
                Debug.Log(hit.collider.name);
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                dmg?.takeDamage(damage);

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.activateElem(element, GameManager.instance.transform);
            }
            ammoCount--;
            GameManager.instance.globalAmmoCount(ammoCount, ammoCap);
        }
    }
}
