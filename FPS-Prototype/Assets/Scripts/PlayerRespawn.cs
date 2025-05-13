using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerRespawn : MonoBehaviour
{
    public Vector3 whereToSpawn;

    public void RespawnPlayer()
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = whereToSpawn;
        GameManager.instance.playerScript.HP = GameManager.instance.playerScript.origHealth;
        GetComponent<CharacterController>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FallZone")
        {
            RespawnPlayer();
        }
    }
}
