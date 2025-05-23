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

    Ray ray;
    RaycastHit hit;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    // Update is called once per frame
    void Update()
    {
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

            if (hit.collider.tag != "Reflector")
            {
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

}
