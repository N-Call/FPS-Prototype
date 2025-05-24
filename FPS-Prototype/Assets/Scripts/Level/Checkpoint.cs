using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    public List<Spawner> spawnersToDisable;
    public bool isFinalCheckPoint;
    

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            EnemyController enemy = GetComponent<EnemyController>();
            
            SoundManager.instance.PlaySFX("checkPoint", 1f);
            Debug.Log("checkpoint reached");
            GameManager.instance.SetSpawnPosition(transform.position);
            GameManager.instance.playerScript.UpdateCheckpointHealth();
            Destroy(gameObject);

            foreach (Spawner spawner in spawnersToDisable)
            {
                spawner.DisableSpawner();
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
