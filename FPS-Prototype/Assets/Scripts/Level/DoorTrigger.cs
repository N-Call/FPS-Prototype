using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private List<OpenDoors> doorScript;
    [SerializeField] private string colliderObject = "Player";
    [SerializeField] AudioSource clip;
    [SerializeField] AudioClip doorOpen;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(colliderObject))
        {
            if (clip != null)
            {
                clip.PlayOneShot(doorOpen);
            }
            foreach (OpenDoors door in doorScript)
            {
                door.Open(other.transform.position);
                
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(colliderObject))
        {
            foreach (OpenDoors door in doorScript)
            {
                door.Close(other.transform.position);
            }
        }
    }

}
