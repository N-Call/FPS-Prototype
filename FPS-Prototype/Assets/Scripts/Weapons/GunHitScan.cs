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

    LineRenderer ricochetLineRenderer;
    Collider hitCollider;

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

        RaycastHit cameraHit;
        if (!ShouldDrawLine(out cameraHit))
        {
            ricochetLineRenderer.enabled = false;
            return;
        }
        else if (!ricochetLineRenderer.enabled)
        {
            ricochetLineRenderer.enabled = true;
        }

        ricochetLineRenderer.positionCount = 2;
        ricochetLineRenderer.SetPosition(0, shootPosition.position);
        ricochetLineRenderer.SetPosition(1, cameraHit.point);

        float remainingLength = Mathf.Min(distance, lineRange);
        Ray ray = new Ray(cameraHit.point, Vector3.Reflect(shootPosition.forward, cameraHit.normal));
        RaycastHit hit;

        for (int i = 0; i < maxReflections; i++)
        {
            ricochetLineRenderer.positionCount++;

            if (!Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                ricochetLineRenderer.SetPosition(ricochetLineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
                continue;
            }

            ricochetLineRenderer.SetPosition(ricochetLineRenderer.positionCount - 1, hit.point);
            ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            remainingLength -= Vector3.Distance(ray.origin, hit.point);

            // Check if they hit an IDamage or ITarget (an enemy or target)
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            ITarget targ = hit.collider.GetComponent<ITarget>();

            if (dmg != null || targ != null)
            {
                hitCollider = hit.collider;
                break;
            }
        }
    }

    bool ShouldRicochet()
    {
        if (!GameManager.instance.playerAbilities.RicochetUnlocked())
        {
            return false;
        }

        if (reloadInProgress)
        {
            return false;
        }

        if (!Input.GetButton("Fire2"))
        {
            return false;
        }

        return true;
    }

    bool ShouldDrawLine(out RaycastHit cameraHit)
    {
        cameraHit = default;

        if (!ShouldRicochet())
        {
            return false;
        }

        return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit, distance, ~playerLayerMask);
    }

    void ShootAt(Vector3 location)
    {
        GameObject effectHit = Instantiate(hitEffect, location, Quaternion.identity);
        Destroy(effectHit, 2f);

        if (hitCollider == null)
        {
            return;
        }

        // Damage enemy or target
        IDamage dmg = hitCollider.GetComponent<IDamage>();
        dmg?.TakeDamage(damage);

        ITarget targ = hitCollider.GetComponent<ITarget>();
        targ?.ActivateElem((int) elem);
    }

    public override void AttackBegin(LayerMask playerMask)
    {
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
            SoundManager.instance.PlaySFX("gunClick");
            return;
        }

        //      shootRate <= shootTimer
        // &&   ammoCount > 0
        // &&   currTotalBullets > 0

        PlayShootAnim();
        SoundManager.instance.PlaySFX("pistol");

        // If they should ricochet
        int positions = ricochetLineRenderer.positionCount;
        if (positions > 1 && ShouldRicochet())
        {
            for (int i = 1; i < positions; i++)
            {
                ShootAt(ricochetLineRenderer.GetPosition(i));
            }
        }

        else
        {
            // See if you hit an object
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, ~playerMask))
            {
                hitCollider = hit.collider;
                ShootAt(hit.point);
            }
        }

        ammoCount--;
        currTotalBullets--;
        GameManager.instance.GlobalAmmoCount(ammoCount, currTotalBullets - ammoCount);
    }

}
