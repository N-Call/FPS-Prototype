using UnityEngine;

public class InverseZone : MonoBehaviour
{
  

    private void OnTriggerEnter(Collider other)
    {
        IElemental affected = other.gameObject.GetComponent<IElemental>();
        if (affected != null)
            affected.ElementInverse();
    }

    private void OnTriggerExit(Collider other)
    {
        IElemental affected = other.gameObject.GetComponent<IElemental>();
        if (affected != null)
            affected.ElementInverse();
    }
}
