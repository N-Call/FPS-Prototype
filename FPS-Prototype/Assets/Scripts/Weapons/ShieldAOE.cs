using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldAOE : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float maxSize;
    [SerializeField] private float scaleIncreaseRate;
    [SerializeField] private float destroyTime;

    [SerializeField] private int healthAmount;
    [SerializeField] private float healthRate;

    Coroutine DOTRoutin;

    float destroyRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DOTRoutin = StartCoroutine(DOT());
    }

    // Update is called once per frame
    void Update()
    {
        destroyRate += Time.deltaTime;
        if (destroyRate > destroyTime)
        {
            Destroy(gameObject);
            StopCoroutine(DOT());
        }
        if(transform.localScale.magnitude < maxSize)
        {
            transform.localScale *= 1 + scaleIncreaseRate * Time.deltaTime;
        }
    }

    IEnumerator DOT()
    {
        
        while (true)
        {
            if(Vector3.Distance(transform.position, GameManager.instance.player.transform.position) < sphereCollider.radius)
            {
                GameManager.instance.playerScript.TakeDamage(-healthAmount);
            }
            yield return new WaitForSecondsRealtime(healthRate);
        }
    }
}
