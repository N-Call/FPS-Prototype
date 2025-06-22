using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class BossOrbSpawner : MonoBehaviour
{
    [SerializeField] private BossOrb[] orbs;
    [SerializeField] private SplineContainer[] splines;
    [SerializeField] private float waitTime;
    public UnityEvent onReachedPos;
   
    public void SpawnOrb()
    {
        
        int store = UnityEngine.Random.Range(0, orbs.Length);
        Instantiate(orbs[store], transform.position, transform.rotation).spline = splines[store];
        StartCoroutine(WaitForDestination());
    }

    IEnumerator WaitForDestination()
    {
        yield return new WaitForSeconds(waitTime);
        onReachedPos?.Invoke();
    }
}
