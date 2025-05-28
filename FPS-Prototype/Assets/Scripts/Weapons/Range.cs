using UnityEngine;
using UnityEngine.UI;

public class Range : MonoBehaviour, IReloadable, IWeapon
{
    [Header("Ammo Icon Reference")]
    [SerializeField] protected Sprite ammoIcon;
    [Header("Ammo Settings")]
    [SerializeField] protected int ammoOrigCap;
    [SerializeField] protected int reloadCap;
    [SerializeField] protected int ammoCount;
    [SerializeField] protected int currTotalBullets;
    [Header("Weapon Settings")]
    [SerializeField] protected float distance;
    [SerializeField] protected int damage;
    [SerializeField] protected float shootRate;

    public enum ElementType { speed = 1, jump = 2, shield = 3 }

    [SerializeField] public ElementType elem;

    [SerializeField] protected string soundFxName;
    [Range(0, 1f)]
    [SerializeField] protected float soundFxVolume;

    protected int ammoCap;

    protected float shootTimer;

    private Animator animator;

    private void Awake()
    {
        //Grab the animator from object
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootTimer = shootRate;
        ammoCount = reloadCap;
        currTotalBullets = ammoOrigCap + reloadCap;
        ammoCap = ammoOrigCap;
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
    }

    public virtual void AttackBegin(LayerMask playerMask)
    {

    }

    public virtual void AttackEnd(LayerMask playerMask)
    {

    }

    public void Reload()
    {
        //if (reloadCap == ammoCount || ammoCap == 0 && ammoCount == 0) {return;}


        //ammoCap -= reloadCap - ammoCount;
        //ammoCount = reloadCap;

        //if (ammoCap < 0)
        //{
        //    ammoCount += ammoCap;
        //    ammoCap = 0;
        //}
        //GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);

        //Debug.Log($"--- RELOAD START ---");
        //Debug.Log($"Initial: ammoCount={ammoCount}, currTotalBulletr={currTotalBullets}, reloadCap={reloadCap}");

        // Here's how it's set up, since it's taking both the processing and the animations into account.

        // Step 1 - This checks if the mag is already full.
        if (ammoCount == reloadCap)
        {
            //Debug.Log("Mag already full. Can't reload.");
            return;
        }
        // Step 2 - Checks if there are no bullets left in total.
        if (currTotalBullets <= 0)
        {
            //Debug.Log("No ammo in mag and reserve. Can't reload.");
            SoundManager.instance.PlaySFX("gunEmpty", 0.3f);
            return;
        }
        // Step 3 - Checks if there are no bullets left in both the mag and reserves.
        // This looks redundant, but it's there to doubnle check to know that there's
        //  no ammo in both the mag and reserve.
        if (ammoCount == 0 && currTotalBullets - ammoCount <= 0)
        {
            //Debug.Log("No ammo in mag and reserve. Can't reload.");
            SoundManager.instance.PlaySFX("gunEmpty", 0.3f);
            return;
        }

        // Reload animation plays and sound, after checking the above conditions.
        PlayReloadAnim();
        SoundManager.instance.PlaySFX(soundFxName, 0.3f);

        // Now we calculate how much space is empty in the current mag.
        int spaceInMag = reloadCap - ammoCount;

        // This calculates how much actual reserve ammo you have.
        int currReserveAmmo = currTotalBullets - ammoCount;

        // This determines the amount of ammo transfering.
        int ammoToTransfer = Mathf.Min(spaceInMag, currReserveAmmo);

        //Debug.Log($"Calculated: spaceInMag={spaceInMag}, currReserveAmmo={currReserveAmmo}");
        //Debug.Log($"Ammo to Transfer: {ammoToTransfer}");

        // Then perform the transfer of the bullets into the mag.
        if (ammoToTransfer > 0)
        {
            ammoCount += ammoToTransfer;
            //Debug.Log($"After Transfer: ammoCount={ammoCount}, currTotalBullets={currTotalBullets}");
        }
        else
        {
            //Debug.Log("No ammo to transfer (magazine already full or no reserve ammo).");
        }
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        //Debug.Log($"--- RELOAD END ---");
    }

    private void PlayReloadAnim()
    {
        animator?.CrossFade("Reload", 0.1f);
    }

    public void PlayShootAnim()
    {
        animator?.CrossFade("Shoot", 0.1f);
    }

    protected void PlayChargeAnim()
    {
        animator?.CrossFade("Charge", 0.1f);
    }

    protected void PlayIdle()
    {
        if(animator != null) { animator.CrossFade("Idle", 0f); }
    }

    protected void PlaySeconedIdle(bool answer)
    {
        animator?.SetBool("isIdle2", answer);
    }

    private void OnEnable()
    {
        PlayIdle();


        GameManager.instance?.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        GameManager.instance?.SetWeaponIcon(ammoIcon);
    }

    public void SetAmmo(float percent)
    {
        //float temp = percent / 100;
        //int newAmmo = (int)(ammoOrigCap * temp);
        //ammoCap += newAmmo;

        //if (gameObject.activeSelf)
        //{
        //    GameManager.instance?.GlobalAmmoCount(ammoCount, ammoCap);
        //}

        // I thought of using this method to set the total ammo capacity.
        // Normally used for initial setups of full refills.
        float amount = ammoOrigCap * (percent / 100f);
        currTotalBullets = Mathf.Min(ammoCap, (int)amount);
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }

    public void AddAmmoToReserve(int amount)
    {
        currTotalBullets = Mathf.Min(ammoCap, currTotalBullets + amount);
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }
}

