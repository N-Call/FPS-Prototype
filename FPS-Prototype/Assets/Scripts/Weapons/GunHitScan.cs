using UnityEngine;

public class GunHitScan : MonoBehaviour, IWeapon, IReloadable
{
    [SerializeField] int ammoOrigCap;
    [SerializeField] int reloadCap;
    [SerializeField] int ammoCount;
    [SerializeField] float distance;
    [SerializeField] int damage;

    [SerializeField][Range(1, 3)] int element;

    private int ammoCopasity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = reloadCap;
        ammoCopasity = ammoOrigCap;
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
                dmg?.takeDamage(damage);

                ITarget targ = hit.collider.GetComponent<ITarget>();
                targ?.activateElem(element, GameManager.instance.transform);
            }
            ammoCount--;
        }
    }

    public void Reload()
    {
        ammoCopasity -= reloadCap - ammoCount;
        ammoCount = reloadCap;

        if(ammoCopasity < 0)
        {
            ammoCount += ammoCopasity;
            ammoCopasity = 0;
        }
    }
}
