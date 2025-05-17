using UnityEngine;

public class Laser : MonoBehaviour
{

    [Header("Damage Settings")]
    [SerializeField] int damage;

    [Header("Laser Settings")]
    [SerializeField] Material material;
    [SerializeField] float maxLength;
    [SerializeField] float startWidth;
    [SerializeField] float endWidth;
    [SerializeField] int maxReflections;

    LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = material;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    /*
     *      Laser from object to hit
     *      If hit is reflector
     *      -> reflect from hit
     */

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;
        
        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, remainingLength))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                if (hit.collider.tag != "Reflector")
                {
                    break;
                }


            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
            }
        }
    }

}
