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
        ammoCap = ammoOrigCap;
        GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
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
        if (reloadCap == ammoCount || ammoCap == 0 && ammoCount == 0) {return;}

        PlayReloadAnim();
        SoundManager.instance.PlaySFX(soundFxName, 0.3f);

        ammoCap -= reloadCap - ammoCount;
        ammoCount = reloadCap;

        if (ammoCap < 0)
        {
            ammoCount += ammoCap;
            ammoCap = 0;
        }
        GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
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


        GameManager.instance?.GlobalAmmoCount(ammoCount, ammoCap);
        GameManager.instance?.SetWeaponIcon(ammoIcon);
    }

    public void SetAmmo(float percent)
    {
        float temp = percent / 100;
        int newAmmo = (int)(ammoOrigCap * temp);
        ammoCap += newAmmo;

        if (gameObject.activeSelf)
        {
            GameManager.instance?.GlobalAmmoCount(ammoCount, ammoCap);
        }
    }
}

