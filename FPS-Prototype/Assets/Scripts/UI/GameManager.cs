using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.Rendering.PostProcessing;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [Header("Reticles")]
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject hitMakerReticle;
    [Header("UI Counts")]
    [SerializeField] TMP_Text gradeLetter;
    [SerializeField] GameObject ammoCount;
    [SerializeField] GameObject weaponIcon;
    [SerializeField] TMP_Text enemyCountUI;
    [SerializeField] GameObject timerWinCount;
    [SerializeField] GameObject elapsedTime;
    [SerializeField] TMP_Text enemyWinCount;
    [Header("Buff Icons")]
    [SerializeField] GameObject buffSprint;
    [SerializeField] GameObject debuffSprint;
    [SerializeField] GameObject buffJump;
    [SerializeField] GameObject debuffJump;

    List<EnemyController> enemiesToRespawn;
       
    public Vector3 respawnPosition;

    public GameObject playerDamageScreen;
    public GameObject player;

    [Header("Dialogue")]
    public GameObject textPopUp;
    public TextMeshProUGUI speakerUI;
    public TextMeshProUGUI textComponent;


    public Image playerHPbar;
    public Image playerShieldbar;
    public PlayerScript playerScript;
    public SceneData sceneData;
    public SceneLoader sceneLoader;

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
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerScript>();
            startPos = player.transform.position;
        }

        timeScaleOrig = Time.timeScale;
        enemiesToRespawn = new List<EnemyController>();
        
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
        TogglePPVolume();
        // to turn off the reticle
        reticle.SetActive(false);
        SoundManager.instance.musicSource.Pause();
        SoundManager.instance.sfxSource.Pause();
        // stop the player from shooting
        playerScript.enabled = false;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TogglePPVolume();
        menuActive.SetActive(false);
        menuActive = null;
        // to turn on the reticle
        reticle.SetActive(true);
        SoundManager.instance.musicSource.Play();
        SoundManager.instance.sfxSource.Play ();
        playerScript.enabled = true;

    }
    public void ToggleReticle()
    {// this is for the Hit Marker 
        StartCoroutine(ReticleWaitTime());
    }
    // Showing Buffs/DeBuffs top Right of player UI 
    public void BuffSprintIcon(float time)
    {
        StartCoroutine(BuffSprintIconsTime(time));
    }
    public void DeBuffSprintIcon(float time)
    {
        StartCoroutine(DeBuffSprintIconsTime(time));
    }
    public void BuffJumpIcon(float time)
    {
        StartCoroutine(BuffJumpIconsTime(time));
    }
    public void DeBuffJumpIcon(float time)
    {
        StartCoroutine(DeBuffJumpIconsTime(time));
    }

    public void TogglePPVolume()
    {// toggle the blurr for menus 
        PostProcessVolume ppVolume = Camera.main.GetComponent<PostProcessVolume>();
        ppVolume.enabled = !ppVolume.enabled; 
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
            // show off win menu Time with enemy time added 
            timerWinCount.GetComponent<Timer>().DisplayTimeAdded(elapsedTime.GetComponent<Timer>().elapsedTime);
            gradeLetter.GetComponent<GradeSystem>().GradeSystemWin(timerWinCount.GetComponent<Timer>().elapsedTime);

            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void UpdateEnemyCounter(int amount)
    {
        enemyCount += amount;
        enemyCountUI.text = enemyCount.ToString("F0");
        enemyWinCount.text = enemyCount + " * 10s";
    }

    public float EnemyTimePenalty(float totalTime)
    { 
        return totalTime + enemyCount * 10; 
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
   
    public void AddEnemyToRespawn(EnemyController enemy)
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

        //foreach (EnemyController enemy in enemiesToRespawn)
        //{
        //    enemy.ResetEnemies();
        //}

    }
    IEnumerator ReticleWaitTime()
    {
        hitMakerReticle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitMakerReticle.SetActive(false);
    }
    IEnumerator BuffSprintIconsTime(float time)
    {
        buffSprint.SetActive(true);
        yield return new WaitForSeconds(time);
        buffSprint.SetActive(false);
    }
    IEnumerator DeBuffSprintIconsTime(float time)
    {
        debuffSprint.SetActive(true);
        yield return new WaitForSeconds(time);
        debuffSprint.SetActive(false);
    }
    IEnumerator BuffJumpIconsTime(float time)
    {
        buffJump.SetActive(true);
        yield return new WaitForSeconds(time);
        buffJump.SetActive(false);
    }
    IEnumerator DeBuffJumpIconsTime(float time)
    {
        debuffJump.SetActive(true);
        yield return new WaitForSeconds(time);
        debuffJump.SetActive(false);
    }

} 
