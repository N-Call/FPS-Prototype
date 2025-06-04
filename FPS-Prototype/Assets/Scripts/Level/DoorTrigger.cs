using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private List<OpenDoors> doorScript;
    [SerializeField] private string colliderObject = "Player";
    [SerializeField] string sfxName;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(colliderObject))
        {
            SoundManager.instance.PlaySFX(sfxName, 1.0f);
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
