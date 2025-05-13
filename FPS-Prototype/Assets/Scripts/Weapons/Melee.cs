using UnityEngine;
using UnityEngine.UI;

public class Melee : MonoBehaviour, IWeapon
{
    [Header("Referencess")]
    [SerializeField] private Sprite weaponImage;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider weaponCollider;

    [Header("Weapon Settings")]
    [SerializeField] [Range(1,3)]private int element;
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackBegin(LayerMask playerMask)
    {
        SoundManager.instance.PlaySFX("swordSwing");
        //start attack animation
        animator.CrossFade("Attack", 0.1f);
        animator.speed = attackSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) { return; }

        //check to see if the trigger hit an enemy
        other.GetComponent<IDamage>()?.TakeDamage(damage);
        other.GetComponent<ITarget>()?.ActivateElem(element);
    }
    public void AttackEnd(LayerMask playerMask)
    {

    }

    private void OnEnable()
    {
        GameManager.instance?.SetWeaponIcon(weaponImage);
        GameManager.instance?.GlobalAmmoCount(0, 0);
    }

    public void ToggleHitBox(int answer)
    {
        //In animation toggle the collider on/off
        weaponCollider.enabled = (answer != 0);
    }
}
