using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{

    private PlayerRespawn playerRespawn;
    private int playerCount = 0;    

    private void Start()
    {
        playerRespawn = GameObject.Find("Player").GetComponent<PlayerRespawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerCount == 0)
            {
                SoundManager.instance.PlaySFX("checkPoint");
                playerCount++;
            }
            playerRespawn.whereToSpawn = transform.position;
        }
    }

}
