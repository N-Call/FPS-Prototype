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
            Destroy(gameObject);
            
            SoundManager.instance.PlaySFX("checkPoint");
            GameManager.instance.SetSpawnPosition(transform.position);
            
            if(isFinalCheckPoint)
            {
                GameManager.instance.WinCondition(-1);
            }
        }
    }

}
