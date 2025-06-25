using UnityEngine;
using UnityEngine.SceneManagement;

public class OverWorldSceneChange : MonoBehaviour
{
    [SerializeField] GameObject button;


    bool playerInTrigger;

    // Update is called once per frame
    void Update()
    {
        // need to check which trigger zone the player is in to then send to correct scene
        if(playerInTrigger)
        {
            if (Input.GetButtonDown("Interact") && button.CompareTag("ButtonLvl"))
            {
                //level select
                
                SceneManager.LoadScene(1);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                

            }
            else if(Input.GetButtonDown("Interact"))
            {
                //Overworld shop
                
                SceneManager.LoadScene(3);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
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
