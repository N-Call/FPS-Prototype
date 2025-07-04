using Unity.VisualScripting;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] GameObject child;
    [SerializeField] float respawnTime;

    float respawnRate;
    GameObject currChild;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currChild = Instantiate(child, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (currChild == null)
        {
            respawnRate += Time.deltaTime;

            if (respawnRate > respawnTime)
            {
                currChild = Instantiate(child, transform);
                respawnRate = 0;
            }
        }
    }
}
