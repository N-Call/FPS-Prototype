using UnityEngine;

public class GunHitScan : MonoBehaviour, IWeapon, IReloadable
{
    public Transform Player;
    [SerializeField] int ammoMaxCopasity;
    [SerializeField] int reloadCopasity;
    [SerializeField] int ammoCount;
    [SerializeField] float distance;
    [SerializeField] int damage;

    [SerializeField][Range(1, 3)] int element;

    private int ammoCopasity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = reloadCopasity;
        ammoCopasity = ammoMaxCopasity;
        GameManager.instance.globalAmmoCount(ammoCount, ammoCopasity);
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

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.activateElem(element, Player);
            }
            ammoCount--;
            GameManager.instance.globalAmmoCount(ammoCount, ammoCopasity);
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
        GameManager.instance.globalAmmoCount(ammoCount, ammoCopasity);
    }
    private void OnEnable()
    {
        // For safety call on globalAmmoCount for if its null don't use
        GameManager.instance?.globalAmmoCount(ammoCount, ammoCopasity);
    }
}
