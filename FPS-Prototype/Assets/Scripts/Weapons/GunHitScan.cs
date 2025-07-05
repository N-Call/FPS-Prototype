using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHitScan : Range
{

    [Header("Hit Scan Settings")]
    public GameObject hitEffect;
    [SerializeField] Transform shootPosition;
    [SerializeField] LayerMask playerLayerMask;

    [Header("Major Upgrade Settings")]
    [SerializeField] float zipDistance;
    [SerializeField] float zipSpeed;

    RaycastHit cameraHit;
    Collider hitCollider;

    private bool isTargeting;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, zipDistance, ~playerLayerMask))
        {
            if (GameManager.instance.playerScript.speedBuffed && InputActionManager.instance.playerChange)
            {
                StartCoroutine(MoveOverTime(hit.point, zipSpeed));
                return;
            }
        }
    }
   
    bool ShootAt(Vector3 location)
    {

        GameObject effectHit = Instantiate(hitEffect, location, Quaternion.identity);
        Destroy(effectHit, 2f);

        if (hitCollider == null)
        {
            return false;
        }

        IDamage dmg = hitCollider.GetComponent<IDamage>();
        IOrb targ = hitCollider.GetComponent<IOrb>();
        Break breakable = hitCollider.GetComponent<Break>();

        if (dmg != null || targ != null || breakable != null)
        {
            dmg?.TakeDamage( (GameManager.instance.playerAbilities != null)?  damage + GameManager.instance.playerAbilities.w1DmgMod : damage);
            targ?.ActivateEffect(GameManager.instance.playerScript, EAbility.speedBoost);
            breakable?.Shatter(location);
            return true;
        }
        return false;
    }

    public override void AttackBegin(LayerMask playerMask)
    {
        float shootRateMod = (GameManager.instance.playerAbilities == null)? shootRate : shootRate - GameManager.instance.playerAbilities.w1RateMod;

        if (shootRate > shootTimer)
        {
            return;
        }

        shootTimer = 0;

        if (ammoCount <= 0 && currTotalBullets <= 0)
        {
            SoundManager.instance.PlaySFX("gunEmpty");
            return;
        }

        if (ammoCount <= 0 && currTotalBullets > 0)
        {
            Reload();
            
            return;
        }

        PlayShootAnim();
        SoundManager.instance.PlaySFX("pistol");

        RaycastHit hit;
        bool hitEnemyOrTarget = false;

        // Check if they hit an enemy
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, ~playerMask))
        {
            hitCollider = hit.collider;
            hitEnemyOrTarget = ShootAt(hit.point);
        }

        ammoCount--;
        currTotalBullets--;
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }



    IEnumerator MoveOverTime(Vector3 target, float duration)
    {
        isTargeting = true;
        GameManager.instance.playerScript.stopActions = true;
        GameObject player = GameManager.instance.player;
        Vector3 start = player.transform.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Vector3 nextPos = Vector3.Lerp(start, target, elapsed / duration);
            player.GetComponent<CharacterController>().Move(nextPos - transform.position);

            yield return null;
        }
        GameManager.instance.playerScript.stopActions = false;
        GameManager.instance.playerScript.ActivateProvideExtraJump();
        GameManager.instance.playerScript.EndAbility(EAbility.speedBoost);
        isTargeting = false;
    }

}
