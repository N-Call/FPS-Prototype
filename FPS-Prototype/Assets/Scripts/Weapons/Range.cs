using UnityEngine;

public class Range : MonoBehaviour, IReloadable, IWeapon
{
    [Header("Ammo Settings")]
    [SerializeField] protected int ammoOrigCap;
    [SerializeField] protected int reloadCap;
    [SerializeField] protected int ammoCount;
    [Header("Weapon Settings")]
    [SerializeField] protected float distance;
    [SerializeField] protected int damage;

    [SerializeField][Range(1, 3)] protected int element;

    [SerializeField] protected string soundFxName;
    [Range(0, 1f)]
    [SerializeField] protected float soundFxVolume;

    protected int ammoCap;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoCount = reloadCap;
        ammoCap = ammoOrigCap;
        GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
    }

    public virtual void AttackBegin(LayerMask playerMask)
    {

    }

    public virtual void AttackEnd(LayerMask playerMask)
    {

    }

    public void Reload()
    {
        SoundManager.instance.PlaySFX(soundFxName);

        ammoCap -= reloadCap - ammoCount;
        ammoCount = reloadCap;

        if (ammoCap < 0)
        {
            ammoCount += ammoCap;
            ammoCap = 0;
        }
        GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
    }
    private void OnEnable()
    {

        GameManager.instance?.GlobalAmmoCount(ammoCount, ammoCap);
    }
}

