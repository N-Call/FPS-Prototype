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
            if (InputActionManager.instance.playerInteract && button.CompareTag("ButtonLvl"))
            {
                //level select
                GameManager.instance.playerPosition.SaveToFile();
                SceneManager.LoadScene(1);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if(InputActionManager.instance.playerInteract)
            {
                //Overworld shop
                GameManager.instance.playerPosition.SaveToFile();
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
