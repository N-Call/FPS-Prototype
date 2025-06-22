using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] bool isOnStartScreen = false;

    [Header("Menus")]
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject firstSelectedButton;
    

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuRules;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuShowBoard;
    [SerializeField] GameObject nextLvlBtn;
    [SerializeField] GameObject globalVol;

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
    [SerializeField] TMP_Text scrapUI;
    [SerializeField] TMP_Text totalScrapUI;

    [Header("Buff Icons")]
    [SerializeField] GameObject buffSprint;
    [SerializeField] GameObject debuffSprint;
    [SerializeField] GameObject buffJump;
    [SerializeField] GameObject debuffJump;


    

    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    public GameObject playerDamageScreen;
    public GameObject playerInInverseScreen;
    public GameObject player;

    [Header("Dialogue")]
    public GameObject textPopUp;
    public TextMeshProUGUI speakerUI;
    public TextMeshProUGUI textComponent;

    public Image bossHPbar;

    public Image playerHPbar;
    public Image playerShieldbar;
    public PlayerScript playerScript;
    public PlayerAbilities playerAbilities;
    public SceneData sceneData;
    public SceneLoader sceneLoader;
    public FinalGradeSystem gradeSystem;
    public ScrapManager scrapManager;
    public VolumeSystemData volumeSystemData;
    public BossSM boss; 
    

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
    int scrapCounter = 100000;
    
    public List<UpgradeData> allUpgrades;
    private Spawner[] allSpawners;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        DebugManager.instance.enableRuntimeUI = false;

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerScript>();
            playerAbilities = player.GetComponent<PlayerAbilities>();
            startPos = player.transform.position;
        }

        allSpawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);

        timeScaleOrig = Time.timeScale;
       
        SaveSettingsSystem.Load();
        

        //if (playerAbilities == null)
        //{
        //    playerAbilities = new PlayerAbilities();
        //}
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            StartCoroutine(ShowCursorDelayed());

            isPaused = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //EnablePPVolume();
            globalVol.SetActive(true);
            // to turn off the reticle
            reticle.SetActive(false);
            SoundManager.instance.musicSource.Pause();
            SoundManager.instance.sfxSource.Stop();
            // stop the player from shooting
            menuActive = menuRules;
            menuRules.SetActive(true);

            playerScript.enabled = false;
            InputActionManager.instance.EnableMenuInput();
        }

        if (scrapUI != null)
        {
            scrapUI.text = scrapCounter.ToString("F0");
            
        }
        if (totalScrapUI != null)
        {
            totalScrapUI.text = scrapManager.totalScrap.ToString("F0");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnStartScreen)
        {
            return;
        }

        if (InputActionManager.instance.playerPause)
        {
            MenuManager.instance.ShowPauseMenu();
        }
        else if (InputActionManager.instance.menuUnpause)
        {
            MenuManager.instance.CloseMenu();
        }

        //if (isPaused && menuActive == menuPause && InputActionManager.instance.menuNavigate.magnitude > 0 && eventSystem.currentSelectedGameObject == null)
        //{
        //    eventSystem.SetSelectedGameObject(firstSelectedButton);
        //}

        if (!isPaused && playerScript != null)
        {
            if (playerScript.speedBuffed || playerScript.jumpBuffed || playerScript.speedDebuffed || playerScript.jumpDebuffed)
            {
                HandleElemTimers();
            }
        }
    }

    public void StatePause(bool showPauseMenu)
    {
        if (isOnStartScreen || menuActive != null)
        {
            return;
        }
        
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //EnablePPVolume();
        globalVol.SetActive(true);
        // to turn off the reticle
        reticle.SetActive(false);
        SoundManager.instance.musicSource.Pause();
        SoundManager.instance.sfxSource.Stop();
        // stop the player from shooting

        playerScript.enabled = false;
        InputActionManager.instance.EnableMenuInput();

       
        if (showPauseMenu)
        {
            menuActive = menuPause;
            menuPause.SetActive(isPaused);
        }
        
    }

    public void StatePause()
    {
        StatePause(false);
    }

    public void StateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //DisablePPVolume();
        globalVol.SetActive(false);

        // Disable menus
        if (menuSettings != null)
        {
            menuSettings.SetActive(false);
        }

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }

        // Turn on the reticle
        if (reticle != null)
        {
            reticle.SetActive(true);
        }

        SoundManager.instance.musicSource.Play();
        volumeSystemData.SetVolumes();

        // Handle controls
        if (playerScript != null)
        {
            InputActionManager.instance.DisableMenuInput();
            playerScript.enabled = true;
        }
        if (menuRules != null)
        {
            // for level 1 rules
            menuRules.SetActive(false);
            menuActive = null;
            
        }
    }
    public void AddScrap(int amount)
    {
        Debug.Log(amount + "added");
        scrapCounter += amount;

        scrapUI.text = scrapCounter.ToString("F0");
    }

    public void AddToTotalScrap()
    {
        scrapManager.totalScrap += scrapCounter;
    }

    public bool SpendScrap(int amount)
    {
        if (scrapManager.totalScrap >= amount)
        {
            scrapManager.totalScrap -= amount;
            Debug.Log(scrapManager.totalScrap + "My Money");
            totalScrapUI.text = scrapManager.totalScrap.ToString("F0");

            return true;
        }
        return false;
    }

    public void ShowScrap()
    {
        totalScrapUI.text = scrapManager.totalScrap.ToString("F0");
    }

    public bool CanBuy(UpgradeData upgrade)
    {
        if (upgrade.isMajor)
        {
            Debug.Log("Can I buy major");
            return (upgrade);
        }
        else
        {
            Debug.Log("Maxed out lvl need to prompt player");
            return upgrade.currentLevel < upgrade.maxLevel &&
                   scrapManager.totalScrap >= upgrade.costPerLevel[upgrade.currentLevel];
        }
    }

    public void BuyUpgrade(UpgradeData upgrade)
    {
        if (!CanBuy(upgrade)) return;

        

        if (upgrade.isMajor && upgrade.currentLevel < upgrade.maxLevel)
        {
            Debug.Log("I bought a major");
            SpendScrap(upgrade.majorCost);
            upgrade.currentLevel++;
            
            ApplyUpgrade(upgrade);
        }
        else
        {
            Debug.Log("I bought minors");
            int cost = upgrade.costPerLevel[upgrade.currentLevel];
            SpendScrap(cost);
            upgrade.currentLevel++;
           
            ApplyUpgrade(upgrade);
        }
    }


    private void ApplyUpgrade(UpgradeData upgrade)
    {
        Debug.Log("Minor upgrade applied: " + upgrade.name + " to level " + upgrade.currentLevel);
        // Apply minor upgrade effect here

        UpgradeCategory name = upgrade.category;
           
        UpgradeType upgradeType = upgrade.type;

        switch (name)
        {
            case UpgradeCategory.Weapon1:
                Debug.Log("we upgraded the pistol");
                if((upgradeType == UpgradeType.Damage))
                {
                    playerAbilities.w1DmgMod++;
                }
                else if ((upgradeType == UpgradeType.Speed))
                {
                    //change to ammo mag size
                    playerAbilities.w1AmmoMag ++;
                }
                else if ((upgradeType == UpgradeType.Rate))
                {
                    playerAbilities.w1RateMod --;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.w1Major = true;
                    playerAbilities.ricochet = true;
                    
                    // need full auto for the pistol  
                }
                break;
            case UpgradeCategory.Weapon2:
                Debug.Log("we upgraded the bow");
                if ((upgradeType == UpgradeType.Damage))
                {
                    playerAbilities.w2DmgMod++;
                }
                else if ((upgradeType == UpgradeType.Speed))
                {
                    //spped of the arrow
                    playerAbilities.w2SpeedMod++;
                }
                else if ((upgradeType == UpgradeType.Rate))
                {
                    //can shoot faster
                    playerAbilities.w2RateMod++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.w2Major = true;
                }
                break;
            case UpgradeCategory.Weapon3:
                Debug.Log("we upgraded the sword");
                if ((upgradeType == UpgradeType.Damage))
                {
                    playerAbilities.w3DmgMod++;
                }
                else if ((upgradeType == UpgradeType.Speed))
                {
                    //speed of the swing 
                    playerAbilities.w3SpeedMod++;
                }
                else if ((upgradeType == UpgradeType.Rate))
                {
                    //speed of how many time you can swing
                    playerAbilities.w3RateMod++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.w3Major = true;
                }
                break;
            case UpgradeCategory.OrbSpeed:
                Debug.Log("we upgraded the orbSpeed");
                if ((upgradeType == UpgradeType.OrbStrength))
                {
                    playerAbilities.o1Srt++;
                }
                else if ((upgradeType == UpgradeType.OrbDuration))
                {
                    playerAbilities.o1Dur++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.o1Major = true;
                }
                break;
            case UpgradeCategory.OrbJump:
                if ((upgradeType == UpgradeType.OrbStrength))
                {
                    playerAbilities.o2Srt++;
                }
                else if ((upgradeType == UpgradeType.OrbDuration))
                {
                    playerAbilities.o2Dur++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.o2Major = true;
                }
                break;
            case UpgradeCategory.OrbShield:
                if ((upgradeType == UpgradeType.OrbStrength))
                {
                    playerAbilities.o3Srt++;
                }
                else if ((upgradeType == UpgradeType.OrbDuration))
                {
                    playerAbilities.o3Dur++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.o3Major = true;
                }
                break;
            case UpgradeCategory.OrbTime:
                if ((upgradeType == UpgradeType.OrbStrength))
                {
                    playerAbilities.o4Srt++;
                }
                else if ((upgradeType == UpgradeType.OrbDuration))
                {
                    playerAbilities.o4Dur++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.o4Major = true;
                }
                break;
            case UpgradeCategory.Slide:
                if ((upgradeType == UpgradeType.SlideSpeed))
                {
                    playerAbilities.moveSlideSpeed ++;
                }
            
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.slideMajor = true;
                }
                break;
            case UpgradeCategory.WallRun:
                if ((upgradeType == UpgradeType.WallRunSpeed))
                {
                    playerAbilities.moveWallRunSpeed++;
                }
                else if ((upgradeType == UpgradeType.WallRunJump))
                {
                    playerAbilities.moveWallRunJump++;
                }
                else if ((upgradeType == UpgradeType.Major))
                {
                    playerAbilities.wallRunMajor = true;
                }
                break;
                 
        }
    }


    public void NextLvlBtnOff()
    {
        nextLvlBtn.SetActive(false);
    }

    void DisableCurrentToggledMenu()
    {
        if (menuActive == null)
        {
            return;
        }

        if (menuActive == menuSettings || menuActive == menuRules || menuActive == menuCredits)
        {
            menuActive.SetActive(false);
            SoundManager.instance.musicSource.Pause();
            menuActive = null;
        }
    }

    public void ToggleSettings()
    {
        if (menuSettings != null)
        {
            if (!isOnStartScreen && menuActive == menuPause)
            {
                menuSettings.SetActive(!menuSettings.activeSelf);
                return;
            }

            if (menuActive == menuSettings)
            {
                menuActive.SetActive(false);
                menuActive = null;
                return;
            }

            DisableCurrentToggledMenu();
            menuActive = menuSettings;
            menuActive.SetActive(true);
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
            if (menuActive == menuRules)
            {
                menuActive.SetActive(false);
                menuActive = null;
                return;
            }

            DisableCurrentToggledMenu();
            menuActive = menuRules;
            menuActive.SetActive(true);
        }
    }

    public void ToggleCredits()
    {
        if (menuCredits != null)
        {
            if (menuActive == menuCredits)
            {
                menuActive.SetActive(false);
                menuActive = null;
                return;
            }

            DisableCurrentToggledMenu();
            menuActive = menuCredits;
            menuActive.SetActive(true);
        }
    }

    public void ToggleReticle()
    {
        // this is for the Hit Marker 
        StartCoroutine(ReticleWaitTime());
    }

    // Showing Buffs/DeBuffs top Right of player UI 
    public void BuffSprintIcon(bool active)
    {
        buffSprint.SetActive(active);
    }

    public void DeBuffSprintIcon(bool active)
    {
        debuffSprint.SetActive(active);
    }

    public void BuffJumpIcon(bool active)
    {
        buffJump.SetActive(active);
    }

    public void DeBuffJumpIcon(bool active)
    {
        debuffJump.SetActive(active);
    }

    public void DisablePPVolume()
    {
        PostProcessVolume ppVolume = Camera.main.GetComponent<PostProcessVolume>();
        if (ppVolume != null)
        {
            ppVolume.enabled = false;
        }
    }

    public void EnablePPVolume()
    {
        PostProcessVolume ppVolume = Camera.main.GetComponent<PostProcessVolume>();
        if (ppVolume != null)
        {
            ppVolume.enabled = true;

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
            MenuManager.instance.ShowWinMenu();
            speakerUI.text = string.Empty;
            textComponent.text = string.Empty;

            //Add scraps to total scraps
            AddToTotalScrap();

            // show off win menu Time with enemy time added 
            SoundManager.instance.PlaySFX("victory");
            timerWinCount.GetComponent<Timer>().DisplayTimeAdded(elapsedTime.GetComponent<Timer>().elapsedTime);
            gradeLetter.GetComponent<GradeSystem>().GradeSystemWin(timerWinCount.GetComponent<Timer>().elapsedTime);

            textPopUp.SetActive(true);

            float elapsedTempTime = EnemyTimePenalty(elapsedTime.GetComponent<Timer>().elapsedTime);
            int minutes = Mathf.FloorToInt(elapsedTempTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTempTime % 60);

            gradeSystem.SaveFinal(enemyCount, string.Format("{0:00}:{1:00}", minutes, seconds), gradeLetter.text);
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
            //Debug.Log($"<color=green>UI Update Call: Magazine={amount}, Reserve={ammoCap}</color>");
        }
    }

    public void SetWeaponIcon(Sprite icon)
    {
        if (weaponIcon != null)
        {
            weaponIcon.GetComponent<Image>().sprite = icon;
        }
    }

    //public void AddEnemyToRespawn(EnemyController enemy)
    //{
    //    enemiesToRespawn.Add(enemy);
    //}

    public void SetSpawnPosition(Vector3 newSpawnPosition, Quaternion newSpawnRotation)
    {
        respawnPosition = newSpawnPosition;
        respawnRotation = newSpawnRotation;
    }

    public void Respawn()
    {
        playerScript.GetComponent<CharacterController>().enabled = false;

        if (player.transform.parent != null)
        {
            player.transform.parent = null;
        }

        player.transform.SetPositionAndRotation(respawnPosition, respawnRotation);
        playerScript.ResetPlayerStats();
        ResetElemTimers();
        ResetBossHealth();
        
        foreach (var spawner in allSpawners)
        {
            if (spawner != null)
            {
                Debug.Log("Enemy Health Restored");
                spawner.ResetAllEnemyHealth();
            }
           
        }
        playerScript.GetComponent<CharacterController>().enabled = true;
        
    }

    private void ResetElemTimers()
    {
        speedBuffTimer = 0.0f;
        jumpBuffTimer = 0.0f;
        speedDebuffTimer = 0.0f;
        jumpDebuffTimer = 0.0f;

        speedBuffLimit = 0.0f;
        jumpBuffLimit = 0.0f;
        speedDebuffLimit = 0.0f;
        jumpDebuffLimit = 0.0f;

        buffSprint.SetActive(false);
        buffJump.SetActive(false);
        debuffSprint.SetActive(false);
        debuffJump.SetActive(false);
    }

    public void SetElemParam(int elem, bool buffStatus, float totalTime)
    {

        if (buffStatus)
        {
            switch (elem)
            {
                case 1:
                    Debug.Log("Timer Started");
                    if (playerAbilities != null)
                    {
                        speedBuffLimit = totalTime + playerAbilities.o1Dur;
                    }
                    else
                    {
                        speedBuffLimit = totalTime;
                    }
       
                        speedBuffTimer = 0;
                    
                    break;
                case 2:
                    if (playerAbilities != null)
                    {
                        jumpBuffLimit = totalTime + playerAbilities.o2Dur;
                    }
                    else
                    {
                        jumpBuffLimit = totalTime;
                    }
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

    public void SetOnStartScreen(bool onStartScreen)
    {
        isOnStartScreen = onStartScreen;
    }

    IEnumerator ReticleWaitTime()
    {
        hitMakerReticle.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hitMakerReticle.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void ResetBossHealth()
    {
        if (boss == null)
        {
            GameObject bossObj = GameObject.Find("BossLift/Phase3_Animated");

            if (bossObj != null)
            {
                boss = bossObj.GetComponent<BossSM>();
            }
        }
        if (boss == null) return;
        
        IEnemyReset bossReset = boss.GetComponent<IEnemyReset>();
        if (bossReset != null)
        {
            bossReset.ResetHealth();
        }        
    }

    

    IEnumerator ShowCursorDelayed()
    {
        yield return null; // wait one frame
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
