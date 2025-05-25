using UnityEngine;

public class InverseZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
