using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;

    public event System.Action<Collider> ExitedTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered" + gameObject.name);
        EnteredTrigger?.Invoke(other);
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other)
    {
        ExitedTrigger?.Invoke(other);
    }
}
