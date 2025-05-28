using UnityEngine;

public class Activator : MonoBehaviour
{

    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] bool deactivateOnExit;

    int playerTriggerCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerTriggerCount++;

        foreach (GameObject obj in objectsToActivate) {
            obj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerTriggerCount--;
        if (!deactivateOnExit || playerTriggerCount > 0)
        {
            return;
        }

        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }

}
