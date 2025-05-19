using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{

    public bool isFinalCheckPoint;
    

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            Enemy enemy = GetComponent<Enemy>();
            
            SoundManager.instance.PlaySFX("checkPoint", 1f);
            Debug.Log("checkpoint reached");
            GameManager.instance.SetSpawnPosition(transform.position);
            Destroy(gameObject);
            if (enemy != null && enemy.isDead == true)
            {
                enemy.isRespawned = false;
                Destroy(enemy); 
            }

            if (isFinalCheckPoint)
            {
                Debug.Log("final checkpoint");
                GameManager.instance.WinCondition(-1);
                SoundManager.instance.sfxSource.Stop();
                SoundManager.instance.PlaySFX("victory", 0.5f);
            }

            
        }
    }

}
