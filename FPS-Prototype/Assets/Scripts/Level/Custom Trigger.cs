using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class CustomTrigger : MonoBehaviour
{
    [SerializeField]public UnityEvent onTriggerEnter;

    [SerializeField]public UnityEvent onTriggerExit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke();
    }

    // Update is called once per frame
    public void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }
}
