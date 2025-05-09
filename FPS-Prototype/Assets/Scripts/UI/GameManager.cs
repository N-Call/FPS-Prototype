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
    public PMovement playerScript;

    public bool isPaused;

    float timeScaleOrig;


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
                statePause();
                menuActive = menuPause;
                menuPause.SetActive(isPaused);

            }
            else if (menuActive == menuPause)
            { 
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // to turn off the reticle
        reticle.SetActive(false);
        playerScript.enabled = false;
        SoundManager.instance.musicSource.Stop();
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        // to turn on the reticle
        reticle.SetActive(true);
        playerScript.enabled = true;
        SoundManager.instance.musicSource.Play();
    }

    public void globalAmmoCount(int amount)
    {
        // display ammo count for the UI 
        ammoCount.GetComponent<TMPro.TMP_Text>().text = "" + amount;
    }
   
}
