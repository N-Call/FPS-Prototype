using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    [SerializeField] int scrapAmount;
    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if (pickup != null )
        {
            Debug.Log("scrap picked up");
            pickup.CollectScrap(scrapAmount);
            SoundManager.instance.PlaySFX("Scrap");
            Destroy(gameObject);

        }
    }
}
