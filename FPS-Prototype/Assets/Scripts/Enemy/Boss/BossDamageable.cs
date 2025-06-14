using UnityEngine;

public class BossDamageable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IBossDamagable>()?.TakeDamage();
    }
}
