using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private List<OpenDoors> doorScript;
    [SerializeField] private string colliderObject = "Player";
    [SerializeField] string doorOpen;
    [SerializeField] private bool disableTrigger;
    [SerializeField] private bool isLocked;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(colliderObject) && !isLocked)
        {
            SoundManager.instance.PlaySFX(doorOpen);
            
            foreach (OpenDoors door in doorScript)
            {
                door.Open(other.transform.position);
                
            }
        }
        else if (isLocked) 
        {
           SoundManager.instance.PlaySFX("Door Lock");
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
            if (disableTrigger)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
