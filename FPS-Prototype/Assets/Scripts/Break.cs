using UnityEngine;

public class Break : MonoBehaviour
{
    public Transform brokenObject;
    public float magnitudeCol, radius, power;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > magnitudeCol)
        {
            Destroy(gameObject);
            Instantiate(brokenObject, transform.position, transform.rotation);
            brokenObject.localScale = transform.localScale;
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

            foreach (Collider hit in colliders)
            {
                if (hit.attachedRigidbody)
                {
                    hit.attachedRigidbody.AddExplosionForce(power * collision.relativeVelocity.magnitude, explosionPos, radius);
                }
            }
        }

    }
}
