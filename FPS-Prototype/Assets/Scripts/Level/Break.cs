using UnityEngine;

public class Break : MonoBehaviour, ILevelReset
{
    public GameObject brokenObject;
    public float explosionRadius, explosionForce;
    Vector3 explosionOffset = Vector3.zero;
    private bool isBroken = false;
    private GameObject brokenInstance;
    [SerializeField] string soundFxName;

    public void Shatter(Vector3 explosionOrigin)
    {
        if (isBroken) return;
        isBroken = true;
        gameObject.SetActive(false);

        if (brokenObject != null)
        {           
            brokenInstance = Instantiate(brokenObject, transform.position, transform.rotation);
           
            foreach (Rigidbody rb in brokenInstance.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, explosionOrigin, explosionRadius);
               // rb.angularVelocity = Random.insideUnitSphere * 5f;
            }
        }
        SoundManager.instance.PlaySFX(soundFxName);
           
    }
    public void ResetState()
    {
        if (brokenInstance != null)
        {
            Destroy(brokenInstance);
        }
        isBroken = false;
        gameObject.SetActive(true);
    }
}
