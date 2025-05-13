using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject ammoCount;

    public GameObject player;
    List<GameObject> activeEnemies = new List<GameObject>();
    public PMovement playerScript;
    

    public bool isPaused;

    public float timeScaleOrig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
            
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PMovement>();
        timeScaleOrig = Time.timeScale;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuPause.SetActive(isPaused);

            }
            else if (menuActive == menuPause)
            { 
                StateUnpause();
            }
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // to turn off the reticle
        reticle.SetActive(false);
        SoundManager.instance.musicSource.Stop();
        // stop the player from shooting 
        playerScript.enabled = false;
        
    }
    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        // to turn on the reticle
        reticle.SetActive(true);
        SoundManager.instance.musicSource.Play();
        playerScript.enabled = true;

    }

    public void GlobalAmmoCount(int amount, int ammoCap)
    {
        if (ammoCount != null)
        {
            // display ammo count for the UI 
            ammoCount.GetComponent<TMPro.TMP_Text>().text = "" + amount + "/" + ammoCap;
        }
    }
   
} 
