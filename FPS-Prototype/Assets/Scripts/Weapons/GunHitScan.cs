using UnityEngine;

public class GunHitScan : MonoBehaviour, IWeapon, IReloadable
{
    [SerializeField] int ammoMaxCopasity;
    [SerializeField] int reloadCopasity;
    [SerializeField] int ammoCount;
    [SerializeField] float distance;
    [SerializeField] int damage;

    private int ammoCopasity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = reloadCopasity;
        ammoCopasity = ammoMaxCopasity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(LayerMask playerMask, Camera camera)
    {
        //See if they have bullets
        if (ammoCount > 0)
        {
            //see if you hit an object
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, distance, ~playerMask))
            {
                Debug.Log(hit.collider.name);
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                //damage enemy
                if (dmg != null)
                {
                    dmg.takeDamage(damage);
                }
            }
            ammoCount--;
        }
    }

    public void Reload()
    {
        ammoCopasity -= reloadCopasity - ammoCount;
        ammoCount = reloadCopasity;

        if(ammoCopasity < 0)
        {
            ammoCount += ammoCopasity;
            ammoCopasity = 0;
        }
    }
}
