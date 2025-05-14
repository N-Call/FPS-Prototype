using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject ammoCount;
    [SerializeField] GameObject enemyCountUI;
    [SerializeField] GameObject weaponIcon;

    List<Enemy> enemiesToRespawn;
    Vector3 respawnPosition;

    public GameObject playerDamageScreen;
    public Image playerHPbar;
    public GameObject player;
    public PMovement playerScript;

    public bool isPaused;
    public float timeScaleOrig;

    int gameGoalCount;
    int enemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PMovement>();
        timeScaleOrig = Time.timeScale;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enemiesToRespawn = new List<Enemy>();
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

    public void YouLose() 
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void WinCondition(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void UpdateEnemyCounter(int amount)
    {
        enemyCount += amount;

        if (enemyCountUI != null)
        {
            // display ammo count for the UI 
            enemyCountUI.GetComponent<TMPro.TMP_Text>().text = "" + amount;
        }
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
        weaponIcon.GetComponent<Image>().sprite = icon;
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
        playerScript.HP = playerScript.origHealth;

        playerScript.GetComponent<CharacterController>().enabled = true;

        foreach (Enemy enemy in enemiesToRespawn)
        {
            enemy.ResetEnemies();
        }

    }

} 
