using UnityEngine;
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
            
            SoundManager.instance.PlaySFX("checkPoint");
            Debug.Log("checkpoint reached");
            Vector3 forwardDir = transform.forward;
            Quaternion lookDirection = Quaternion.LookRotation(forwardDir);
            GameManager.instance.SetSpawnPosition(GameManager.instance.player.transform.position, lookDirection);

            foreach (Spawner spawner in spawnersToDisable)
            {
                spawner.DisableSpawner();
            }

            if (isFinalCheckPoint)
            {
                GameManager.instance.WinCondition(-1);
                SoundManager.instance.sfxSource.Stop();
                SoundManager.instance.PlaySFX("victory");
            }

            Destroy(gameObject);
        }

    }

}
