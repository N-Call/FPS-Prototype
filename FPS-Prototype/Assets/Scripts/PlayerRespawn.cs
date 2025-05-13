using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Processors;
public class PlayerRespawn : MonoBehaviour
{
    public Vector3 whereToSpawn;
    public Enemy[] respawn;
    private bool isDead;
    public void RespawnPlayer()
    {
        
        GetComponent<CharacterController>().enabled = false;
        transform.position = whereToSpawn;
        GetComponent<CharacterController>().enabled = true;
        foreach (Enemy enemy in respawn)
        {
            enemy.ResetEnemies();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FallZone")
        {
            RespawnPlayer();
        }
    }
}
