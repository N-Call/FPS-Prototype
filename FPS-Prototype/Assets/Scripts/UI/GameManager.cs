using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject hitMakerReticle;
    [SerializeField] GameObject ammoCount;
    [SerializeField] GameObject weaponIcon;
    [SerializeField] TMP_Text enemyCountUI;
    [SerializeField] GameObject timerWinCount;
    [SerializeField] GameObject elapsedTime;
    [SerializeField] TMP_Text enemyWinCount;


    List<Enemy> enemiesToRespawn;
       
    public Vector3 respawnPosition;

    public GameObject playerDamageScreen;
    public Image playerHPbar;
    public GameObject player;
    public PMovement playerScript;

    public bool isPaused;
    public float timeScaleOrig;
    public Vector3 startPos;


    
    int gameGoalCount;
    int enemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PMovement>();
        timeScaleOrig = Time.timeScale;
       
        enemiesToRespawn = new List<Enemy>();
        startPos = player.transform.position;
        
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
    public void ToggleReticle()
    {// this is for the Hit Marker 
        StartCoroutine(ReticleWaitTime());
    }

    public void YouLose() 
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void StartGame()
    {
        StatePause();
        menuActive = menuStart;
        menuActive.SetActive(true);
    }

    public void WinCondition(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            StatePause();
            
            timerWinCount.GetComponent<Timer>().DisplayTimeAdded(elapsedTime.GetComponent<Timer>().elapsedTime);

            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void UpdateEnemyCounter(int amount)
    {
        enemyCount += amount;
        enemyCountUI.text = enemyCount.ToString("F0");
        enemyWinCount.text = enemyCount.ToString(enemyCount + " * 5s ");
    }

    public float EnemyTimePenalty(float totalTime)
    { 
        return totalTime + enemyCount * 5; 
    }

    public void GlobalAmmoCount(int amount, int ammoCap)
    {
        if (ammoCount != null)
        {
            // display ammo count for the UI 
            ammoCount.GetComponent<TMPro.TMP_Text>().text = "" + amount + "/" + ammoCap;
        }
    }

    public void SetWeaponIcon(Sprite icon)
    {
        if (weaponIcon != null)
        {
            weaponIcon.GetComponent<Image>().sprite = icon;
        }
    }
   
    public void AddEnemyToRespawn(Enemy enemy)
    {
        enemiesToRespawn.Add(enemy);
        
    }

    public void SetSpawnPosition(Vector3 newSpawnPosition)
    {
        respawnPosition = newSpawnPosition;
    }

    public void Respawn()
    {
        playerScript.GetComponent<CharacterController>().enabled = false;

        player.transform.position = respawnPosition;
        playerScript.ResetPlayerStats();

        playerScript.GetComponent<CharacterController>().enabled = true;

        foreach (Enemy enemy in enemiesToRespawn)
        {
            enemy.ResetEnemies();
        }

    }
    IEnumerator ReticleWaitTime()
    {
        hitMakerReticle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitMakerReticle.SetActive(false);
    }

} 
