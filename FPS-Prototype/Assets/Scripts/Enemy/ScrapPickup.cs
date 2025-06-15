using TMPro;
using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
   
    [HideInInspector]public int scrapAmount;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if (pickup != null )
        {
            pickup.CollectScrap(scrapAmount);
            SoundManager.instance.PlaySFX("Scrap");
            Destroy(gameObject);
        }
    }
}
