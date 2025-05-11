using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerRespawn : MonoBehaviour
{
    Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPosition = GameManager.instance.player.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FallZone")
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = spawnPosition;
            GetComponent<CharacterController>().enabled = true;
        }
    }

    public void PlayerDeath()
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = spawnPosition;
        GetComponent<CharacterController>().enabled = true;
    }

    public void UpdateSpawnPosition(Vector3 newPosition)
    {
        spawnPosition = newPosition;    
    }
}
