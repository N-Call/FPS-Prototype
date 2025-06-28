using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
public class BossOrbSpawner : MonoBehaviour
{
    [SerializeField] private BossOrb[] orbs;
    [SerializeField] private SplineContainer[] splines;
    [SerializeField] private float waitTime;
    public UnityEvent onReachedPos;
    public List<GameObject> smallOrbs = new();
   
    public void SpawnOrb()
    {
        
        int store = UnityEngine.Random.Range(0, orbs.Length);
        orbs[store].orbSpawner = this;
        Instantiate(orbs[store], transform.position, transform.rotation).spline = splines[store];
        if (smallOrbs != null)
        {
            foreach (var orb in smallOrbs)
            {
                Destroy(orb);
            }
            smallOrbs.Clear();
        }
        StartCoroutine(WaitForDestination());
    }

    IEnumerator WaitForDestination()
    {
        yield return new WaitForSeconds(waitTime);
        onReachedPos?.Invoke();
    }
}
