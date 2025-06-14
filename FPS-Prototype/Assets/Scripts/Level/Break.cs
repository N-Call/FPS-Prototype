using UnityEngine;

public class Break : MonoBehaviour
{
    public GameObject brokenObject;
    public float explosionRadius, explosionForce;
    Vector3 explosionOffset = Vector3.zero; 
    bool _alreadyBroken;

    public void Shatter(Vector3 explosionOrigin)
    {
        if (brokenObject != null)
        {
            GameObject brokenObj = Instantiate(brokenObject);
            brokenObj.transform.position = transform.position;
            brokenObj.transform.rotation = transform.rotation;
           
            foreach (Rigidbody rb in brokenObj.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, explosionOrigin, explosionRadius);
               // rb.angularVelocity = Random.insideUnitSphere * 5f;
            }
        }
        SoundManager.instance.PlaySFX("Shatter");
        Destroy(gameObject);    
    }
}
