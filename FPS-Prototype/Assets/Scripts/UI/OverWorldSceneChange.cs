using UnityEngine;
using UnityEngine.SceneManagement;

public class OverWorldSceneChange : MonoBehaviour
{
    [SerializeField] GameObject button;

    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInTrigger)
        {
            if(Input.GetButtonDown("Interact"))
            {
                SceneManager.LoadScene(1);
                
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        playerInTrigger = true;
        button.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerInTrigger = false;
        button.SetActive(false);
    }

    







}
