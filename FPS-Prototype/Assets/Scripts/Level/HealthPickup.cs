using UnityEngine;

public class HealthPickup : MonoBehaviour
{

    [SerializeField] int HP = 1;

    private void OnValidate()
    {
        HP = Mathf.Clamp(HP, 1, int.MaxValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.AddHP(HP);
            SoundManager.instance.PlaySFX("Health", 0.5f);
            Destroy(gameObject);
        }
    }

}
