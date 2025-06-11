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
        // need to check which trigger zone the player is in to then send to correct scene
        if(playerInTrigger)
        {
            if (Input.GetButtonDown("Interact") && button.CompareTag("ButtonLvl"))
            {
                SceneManager.LoadScene(1);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //GameManager.instance.StatePause();

            }
            else if(Input.GetButtonDown("Interact"))
            {
                SceneManager.LoadScene(2);
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
