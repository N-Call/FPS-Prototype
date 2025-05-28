using UnityEngine;

public class Replacer : MonoBehaviour
{
    [SerializeField] GameObject replaced;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(replaced, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
