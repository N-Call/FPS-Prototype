using Unity.VisualScripting;
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
    protected int currTotalBullets;

    protected float shootTimer;

    private Animator animator;
    protected bool reloadInProgress = false;

    protected virtual void Awake()
    {
        //Grab the animator from object
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reloadCap += GameManager.instance.playerAbilities.w1AmmoMag;
        
        shootTimer = shootRate;
        ammoCount = reloadCap;
        currTotalBullets = ammoOrigCap + reloadCap;
        ammoCap = reloadCap + ammoOrigCap;
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }

    void OnEnable()
    {
        PlayIdle();
        GameManager.instance?.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
        GameManager.instance?.SetWeaponIcon(ammoIcon);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        shootTimer += Time.deltaTime;

        if (reloadInProgress && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Reload") && stateInfo.normalizedTime >= 0.9f)
            {
                if (!animator.IsInTransition(0))
                {
                    OnReloadAnimationEnd();
                    reloadInProgress = false;
                }
            }
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public virtual void AttackBegin(LayerMask playerMask)
    {

    }

    public virtual void AttackEnd(LayerMask playerMask)
    {

    }

    public virtual void Reload()
    {
        if (ammoCount == reloadCap)
        {
            return;
        }
        // Step 2 - Checks if there are no bullets left in total.
        if (currTotalBullets <= 0)
        {
            SoundManager.instance.PlaySFX("gunEmpty");
            return;
        }
        // Step 3 - Checks if there are no bullets left in both the mag and reserves.
        // This looks redundant, but it's there to doubnle check to know that there's
        //  no ammo in both the mag and reserve.
        if (ammoCount == 0 && currTotalBullets - ammoCount <= 0)
        {
            SoundManager.instance.PlaySFX("gunEmpty");
            return;
        }

        // Now we calculate how much space is empty in the current mag.
        int spaceInMag = reloadCap - ammoCount;

        // This calculates how much actual reserve ammo you have.
        int currReserveAmmo = currTotalBullets - ammoCount;

        // This determines the amount of ammo transfering.
        int ammoToTransfer = Mathf.Min(spaceInMag, currReserveAmmo);


        // Then perform the transfer of the bullets into the mag.
        if (ammoToTransfer > 0)
        {
            ammoCount += ammoToTransfer;
        }

        // Reload animation plays and sound, after checking the above conditions.
        PlayReloadAnim();
        SoundManager.instance.PlaySFX(soundFxName);

        reloadInProgress = true;
    }

    public void OnReloadAnimationEnd()
    {
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
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
        //Debug.Log($"<color=purple>--- AMMO CRATE PICKUP START ---</color>");
        //Debug.Log($"<color=purple>Before Add: currTotalBullets={currTotalBullets}, ammoCount={ammoCount}, ammoCap={ammoCap}, AmountToAdd={amount}</color>");

        currTotalBullets = Mathf.Min(ammoCap, currTotalBullets + amount);

        //Debug.Log($"<color=purple>After Add: currTotalBullets={currTotalBullets}, ammoCount={ammoCount}</color>");
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);

        //Debug.Log($"<color=purple>--- AMMO CRATE PICKUP END ---</color>");
    }
}

