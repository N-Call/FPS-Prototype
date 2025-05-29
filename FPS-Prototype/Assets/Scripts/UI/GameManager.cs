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
    [SerializeField] GameObject menuRules;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuShowBoard;
    [SerializeField] GameObject nextLvlBtn;
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
    public GameObject playerInInverseScreen;
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
    public FinalGradeSystem gradeSystem;

    public bool isPaused;
    public float timeScaleOrig;
    public Vector3 startPos;

    public float speedBuffTimer;
    public float jumpBuffTimer;
    public float speedDebuffTimer;
    public float jumpDebuffTimer;

    public float speedBuffLimit;
    public float jumpBuffLimit;
    public float speedDebuffLimit;
    public float jumpDebuffLimit;

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
        if (Input.GetButtonDown("Cancel"))
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
        if (playerScript != null)
        {
            if (playerScript.speedBuffed || playerScript.jumpBuffed || playerScript.speedDebuffed || playerScript.jumpDebuffed)
            {
                HandleElemTimers();
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
        SoundManager.instance.sfxSource.Stop();
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
        playerScript.enabled = true;

    }
    public void NextLvlBtnOff()
    {
        nextLvlBtn.SetActive(false);
    }
    public void ToggleSettings()
    {
        if (menuSettings != null)
        {
            if (menuActive == null)
            {
                menuActive = menuSettings;
                menuSettings.SetActive(!menuSettings.activeSelf);
            }
            else if (menuActive == menuPause)
            {
                menuActive = menuPause;
                menuSettings.SetActive(!menuSettings.activeSelf);
            }
            else if (menuActive == menuPause || menuActive == menuSettings)
            {
                menuActive = null;
                menuSettings.SetActive(!menuSettings.activeSelf);
            }
        }
    }

    public void LoadScoreBoard()
    {
        enemyCountUI.text = "" + gradeSystem.enemyCount;
        gradeLetter.text = gradeSystem.finalGrade;
        timerWinCount.GetComponent<TMP_Text>().text = gradeSystem.finalTime;
    }

    public void ToggleShowBoard()
    {
        if (menuShowBoard != null)
        {
            if (menuActive == null)
            {
                menuActive = menuShowBoard;
                menuShowBoard.SetActive(!menuShowBoard.activeSelf);
            }
            else if (menuActive == menuShowBoard)
            {
                menuActive = null;
                menuShowBoard.SetActive(!menuShowBoard.activeSelf);
            }
        }
    }

    public void ToggleRules()
    {
        if (menuRules != null)
        {
            if (menuActive == null)
            {
                menuActive = menuRules;
                menuRules.SetActive(!menuRules.activeSelf);
            }
            else if (menuActive == menuRules)
            {
                menuActive = null;
                menuRules.SetActive(!menuRules.activeSelf);
            }
        }
    }

    public void ToggleCredits()
    {
        if (menuCredits != null)
        {
            if (menuActive == null)
            {
                menuActive = menuCredits;
                menuCredits.SetActive(!menuCredits.activeSelf);
            }
            else if (menuActive == menuCredits)
            {
                menuActive = null;
                menuCredits.SetActive(!menuCredits.activeSelf);
            }
        }
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
        if (ppVolume != null)
        {
            ppVolume.enabled = !ppVolume.enabled;
        }
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
            SoundManager.instance.PlaySFX("victory", 0.1f);
            timerWinCount.GetComponent<Timer>().DisplayTimeAdded(elapsedTime.GetComponent<Timer>().elapsedTime);
            gradeLetter.GetComponent<GradeSystem>().GradeSystemWin(timerWinCount.GetComponent<Timer>().elapsedTime);

            menuActive = menuWin;
            menuActive.SetActive(true);
            gradeSystem.SaveFinal(enemyCount, timerWinCount.GetComponent<TMP_Text>().text, gradeLetter.text);
            
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
            Debug.Log($"<color=green>UI Update Call: Magazine={amount}, Reserve={ammoCap}</color>");
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

    public void SetElemParam(int elem, bool buffStatus, float totalTime)
    {

        if (buffStatus)
        {
            switch (elem)
            {
                case 1:
                    Debug.Log("Timer Started");
                    speedBuffLimit = totalTime;
                    speedBuffTimer = 0;
                    break;
                case 2:
                    jumpBuffLimit = totalTime;
                    jumpBuffTimer = 0;
                    break;
            }
        }
        else
        {
            switch (elem)
            {
                case 1:
                    speedDebuffLimit = totalTime;
                    speedDebuffTimer = 0;
                    break;
                case 2:
                    jumpDebuffLimit = totalTime;
                    jumpDebuffTimer = 0;
                    break;
            }
        }
        
    }
    void HandleElemTimers()
    {
        if (playerScript.speedBuffed)
        {
            speedBuffTimer += Time.deltaTime;
            if (speedBuffTimer >= speedBuffLimit)
            {
                playerScript.ElementReverse();
            }
        }
        if (playerScript.jumpBuffed)
        {
            jumpBuffTimer += Time.deltaTime;
            if (jumpBuffTimer >= jumpBuffLimit)
            {
                playerScript.ElementReverse();
            }
        }
        if (playerScript.speedDebuffed)
        {
            speedDebuffTimer += Time.deltaTime;
            if (speedDebuffTimer >= speedDebuffLimit)
            {
                playerScript.ElementReverse();
            }
        }
        if (playerScript.jumpDebuffed)
        {
            jumpDebuffTimer += Time.deltaTime;
            if (jumpDebuffTimer >= jumpDebuffLimit)
            {
                playerScript.ElementReverse();
            }
        }
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
