using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    //public List<Spawner> spawnersToDisable;
    public bool isFinalCheckPoint;
    
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            
            // play audio feedback
            SoundManager.instance.PlaySFX("checkPoint");
            
            //set spawn position of checkpoint and make sure player spawns looking forward
            Quaternion lookDirection = Quaternion.LookRotation(transform.forward);
            GameManager.instance.SetSpawnPosition(GameManager.instance.player.transform.position, lookDirection);

            if (isFinalCheckPoint)
            {
                SaveSystem.SaveStats();
                GameManager.instance.WinCondition(-1);
                SoundManager.instance.sfxSource.Stop();
                SoundManager.instance.PlaySFX("victory");
            }

            Destroy(gameObject);
        }

    }

}
