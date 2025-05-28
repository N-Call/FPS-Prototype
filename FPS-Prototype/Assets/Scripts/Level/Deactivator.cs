using UnityEngine;

public class Deactivator : MonoBehaviour
{

    [SerializeField] GameObject[] objectsToDeactivate;
    [SerializeField] bool activateOnExit;

    int playerTriggerCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerTriggerCount++;

        foreach (GameObject obj in objectsToDeactivate) {
            obj.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerTriggerCount--;
        if (!activateOnExit || playerTriggerCount > 0)
        {
            return;
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(true);
        }
    }

}
