using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{

    private PlayerRespawn playerRespawn;
    public bool isFinalCheckPoint;

    private void Start()
    {
        playerRespawn = GameObject.Find("Player").GetComponent<PlayerRespawn>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            
            SoundManager.instance.PlaySFX("checkPoint");
                              
            playerRespawn.whereToSpawn = transform.position;
            
            if(isFinalCheckPoint)
            {
                Debug.Log("final checkpoint");
                GameManager.instance.WinCondition(-1);
            }
        }

    }

    

}
