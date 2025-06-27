using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("killzone entered");
        if (other.CompareTag("Player"))
        {
            MenuManager.instance.ShowLoseMenu();
        }
    }
}
