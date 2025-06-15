using System.Collections.Generic;
using UnityEngine;

public class GunHitScan : Range
{

    [Header("Hit Scan Settings")]
    public GameObject hitEffect;
    [SerializeField] Transform shootPosition;
    [SerializeField] LayerMask playerLayerMask;
   
    

    [Header("Ricochet Upgrade Settings")]
    [SerializeField] float lineRange = 50.0f;
    
    [SerializeField][Tooltip("Only applied if there's no line renderer component already on the object")]
    float rendererStartWidth = 0.1f, rendererEndWidth = 0.1f;

    [SerializeField][Tooltip("Only applied if there's no line renderer component already on the object")]
    Gradient rendererGradient;

    [SerializeField][Tooltip("Only applied if there's no line renderer component already on the object")]
    Material rendererMaterial;

    [SerializeField] int maxReflections;

    RaycastHit cameraHit;
    LineRenderer ricochetLineRenderer;
    Collider hitCollider;

    [SerializeField] float bulletForce;



    protected override void Awake()
    {
        base.Awake();

        ricochetLineRenderer = gameObject.GetComponent<LineRenderer>();
        if (ricochetLineRenderer == null)
        {
            ricochetLineRenderer = gameObject.AddComponent<LineRenderer>();
            ricochetLineRenderer.startWidth = rendererStartWidth;
            ricochetLineRenderer.endWidth = rendererEndWidth;

            ricochetLineRenderer.colorGradient = rendererGradient;
            ricochetLineRenderer.material = rendererMaterial;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Draw line from shootPosition to camera

        if (!ShouldDrawLine())
        {
            // Disable line renderer and return if we should not draw a line
            ricochetLineRenderer.enabled = false;
            return;
        }
        else if (!ricochetLineRenderer.enabled)
        {
            ricochetLineRenderer.enabled = true;
        }

        // Draw the line from the shootPosition to the camera's hit point
        ricochetLineRenderer.positionCount = 2;
        ricochetLineRenderer.SetPosition(0, shootPosition.position);
        ricochetLineRenderer.SetPosition(1, cameraHit.point);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, ~playerLayerMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            ITarget targ = hit.collider.GetComponent<ITarget>();

            if (dmg != null || targ != null)
            {
                hitCollider = hit.collider;
                return;
            }
        }

        // Get hit positions from reflections
        foreach (Vector3 position in GetHitPositions(Mathf.Min(distance, lineRange))) {
            ricochetLineRenderer.positionCount++;
            ricochetLineRenderer.SetPosition(ricochetLineRenderer.positionCount - 1, position);
        }
    }

    List<Vector3> GetHitPositions(float maxDistance)
    {
        List<Vector3> positions = new();

        float remainingLength = maxDistance;
        Ray ray = new Ray(cameraHit.point, Vector3.Reflect(shootPosition.forward, cameraHit.normal));

        RaycastHit hit;
        for (int i = 0; i < maxReflections; i++)
        {
            if (!Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                positions.Add(ray.origin + ray.direction * remainingLength);
                continue;
            }

            positions.Add(hit.point);
            ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            remainingLength -= Vector3.Distance(ray.origin, hit.point);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            ITarget targ = hit.collider.GetComponent<ITarget>();
            if (dmg != null || targ != null)
            {
                hitCollider = hit.collider;
                return positions;
            }
        }

        return positions;
    }

    bool ShouldRicochet()
    {
        if (GameManager.instance.playerAbilities == null || !GameManager.instance.playerAbilities.RicochetUnlocked())
        {
            return false;
        }

        if (reloadInProgress)
        {
            return false;
        }

        if (!InputActionManager.instance.playerAim)
        {
            return false;
        }

        return true;
    }

    bool ShouldDrawLine()
    {
        cameraHit = default;

        if (!ShouldRicochet())
        {
            return false;
        }

        return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit, distance, ~playerLayerMask);
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
        ITarget targ = hitCollider.GetComponent<ITarget>();
        Break breakable = hitCollider.GetComponent<Break>();

        if (dmg != null || targ != null || breakable != null)
        {
            
            dmg?.TakeDamage(damage + GameManager.instance.playerAbilities.w1DmgMod);
            targ?.ActivateElem((int)elem);
            breakable?.Shatter(location);
            return true;
        }
        return false;
        // Check if they damaged an enemy or target..
        //IDamage dmg = hitCollider.GetComponent<IDamage>();
        //if (dmg != null)
        //{
        //    dmg.TakeDamage(damage);
        //    return true;
        //}

        //ITarget targ = hitCollider.GetComponent<ITarget>();
        //if (targ != null)
        //{

        //    targ.ActivateElem((int)elem);
        //    return true;
        //}
        //Break breakable = hitCollider.GetComponent<Break>();

        //if (breakable != null)
        //{
        //    breakable.Shatter(location);
        //    return true;
        //}

    }

    public override void AttackBegin(LayerMask playerMask)
    {
        float shootRateMod = shootRate + GameManager.instance.playerAbilities.w1RateMod;

        if (shootRateMod > shootTimer)
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
            SoundManager.instance.PlaySFX("gunClick");
            return;
        }

        //      shootRate <= shootTimer
        // &&   ammoCount > 0
        // &&   currTotalBullets > 0

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

        // If they should ricochet and did not hit an enemy or target..
        if (ShouldRicochet() && !hitEnemyOrTarget)
        {
            foreach (Vector3 position in GetHitPositions(distance))
            {
                ShootAt(position);
            }
        }

        ammoCount--;
        currTotalBullets--;
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }

}
