using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //GameManager.instance.YouLose();
            PlayerScript playerScript = GetComponent<PlayerScript>();
            IDamage dmg = playerScript.GetComponent<IDamage>();
            dmg?.TakeDamage(50);
        }
    }
}
