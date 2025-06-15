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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnOrb()
    {
        Debug.Log("is spawning");
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
