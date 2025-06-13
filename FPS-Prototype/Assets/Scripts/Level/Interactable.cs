using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject objectToActivate;

    bool canInteract;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInteract)
        {
            return;
        }

        if (InputActionManager.instance.playerInteract)
        {
            canvas.SetActive(false);
            objectToActivate.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvas.SetActive(true);
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canvas.SetActive(false);
            canInteract = false;
            objectToActivate.SetActive(false);
        }
    }

}
