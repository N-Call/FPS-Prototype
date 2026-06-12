using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{

    [Header("Damage Settings")]
    [SerializeField] int damage;
    [SerializeField] float damageRate;

    [Header("Laser Settings")]
    [SerializeField] Material material;
    [SerializeField] float maxLength;
    [SerializeField] float startWidth;
    [SerializeField] float endWidth;
    [SerializeField] int maxReflections;

    LineRenderer lineRenderer;

    public GameObject hitEffect;

    Ray ray;
    RaycastHit hit;

    bool isDamaging;
    public bool laserCanToggle;
    public float onTime;
    public float offTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        if (laserCanToggle)
        {
            StartCoroutine(ToggleLaser());
        }

        if (DifficultyManager.Instance != null && DifficultyManager.Instance.currentSettings != null)
        {
            damage = (int)(damage * DifficultyManager.Instance.currentSettings.laserDmgMod);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!lineRenderer.enabled)
            return;

        ray = new Ray(transform.position, transform.forward);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;

        for (int i = 0; i <= maxReflections; i++)
        {
            lineRenderer.positionCount++;

            if (!Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
                continue;
            }

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
            ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
            remainingLength -= Vector3.Distance(ray.origin, hit.point);
            
            if (!hit.collider.CompareTag("Reflector"))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    break;
                }

                IDamage damageable = hit.collider.GetComponent<IDamage>();
                if (damageable != null && !isDamaging)
                {
                    StartCoroutine(DealDamage(damageable));
                }

                break;
            }
        }
    }

    IEnumerator DealDamage(IDamage other)
    {
        isDamaging = true;
        other?.TakeDamage(damage);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    private IEnumerator ToggleLaser()
    {
        while (true)
        {
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(onTime);

            lineRenderer.enabled = false;
            yield return new WaitForSeconds(offTime);
        }
    }
}
