using UnityEngine;
using UnityEngine.UI;

public class Melee : MonoBehaviour, IWeapon
{
    public enum ElementType { speed = 1, jump = 2, shield = 3 }

    
    [Header("Referencess")]
    [SerializeField] private Sprite weaponImage;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider weaponCollider;

    [Header("Weapon Settings")]
    [SerializeField] public ElementType elem;
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRate;

    private float attackTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = attackRate;
        GetComponentInChildren<Damage>().AddDamageAmount(damage);
        GetComponentInChildren<Damage>().SetElement((int)elem);
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
    }

    public void AttackBegin(LayerMask playerMask)
    {
        float tRate = (GameManager.instance.playerAbilities != null) ? attackRate - GameManager.instance.playerAbilities.w3RateMod : attackRate;
        if (attackTimer < tRate) { return; }

        SoundManager.instance.PlaySFX("swordSwing");
        //start attack animation
        animator.CrossFade("Attack", 0.1f);
        animator.speed = (GameManager.instance.playerAbilities != null) ? attackSpeed + GameManager.instance.playerAbilities.w3SpeedMod : attackSpeed;
        attackTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) { return; }

        //check to see if the trigger hit an enemy
        other.GetComponent<IDamage>()?.TakeDamage((GameManager.instance.playerAbilities != null)? damage + GameManager.instance.playerAbilities.w3DmgMod: damage);
        other.GetComponent<ITarget>()?.ActivateElem((int)elem);
    }
    public void AttackEnd(LayerMask playerMask)
    {

    }

    private void OnEnable()
    {
        animator.CrossFade("Idle", 0f);

        GameManager.instance?.SetWeaponIcon(weaponImage);
        GameManager.instance?.GlobalAmmoCount(0, 0);
    }

    public void ToggleHitBox(int answer)
    {
        //In animation toggle the collider on/off
        weaponCollider.enabled = (answer != 0);
    }
}
