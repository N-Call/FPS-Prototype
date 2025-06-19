using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowScript : MonoBehaviour
{
    [SerializeField] float castDistance;
    DecalProjector projector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        projector = GetComponent<DecalProjector>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + (Vector3.up * 0.2f), Vector3.down * castDistance, Color.red);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out RaycastHit hit, castDistance))
        {
            float distance = hit.distance;

            //Debug.Log($"Raycast Hit! Distance: {distance}, Hit Object: {hit.collider.name}"); // Log hit information

            // This sets the size to keep the shadow from showing on multiple surfaces.
            projector.size = new Vector3(projector.size.x, projector.size.y, distance);

            // This makes the shadow fade out as you get closer in general.
            projector.fadeFactor = 1 - (distance / castDistance);

            // This moves the projector to help account for the new size overall.
            projector.pivot = (Vector3.forward * (distance / 2 + -0.1f));

            //Debug.Log($"Projector Size Z: {projector.size.z}, Fade Factor: {projector.fadeFactor}, Pivot Z: {projector.pivot.z}");
        }
    }
}
