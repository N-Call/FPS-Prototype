using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance { get; private set; }

    #region Serialized Fields
    [Header("Events")]
    [SerializeField] EventSystem eventSystem;

    [Header("Menus")]
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsAudioMenu;
    [SerializeField] GameObject settingsPCMenu;
    [SerializeField] GameObject settingsControllerMenu;
    [SerializeField] GameObject rulesMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;

    [Header("Start Menu Buttons")]
    [SerializeField] GameObject startMenuFirstSelected;
    [SerializeField] GameObject startMenuFirstSelectedNoSave;
    [SerializeField] GameObject startMenuOverworldButton;
    [SerializeField] GameObject startMenuSettingsButton;
    [SerializeField] GameObject startMenuRulesButton;
    [SerializeField] GameObject startMenuCreditsButton;
    [SerializeField] GameObject startMenuQuitButton;

    [Header("Settings Menu Buttons")]
    [SerializeField] GameObject settingsMenuFirstSelected;
    [SerializeField] GameObject settingsMenuAudioButton;
    [SerializeField] GameObject settingsMenuPCButton;
    [SerializeField] GameObject settingsMenuControllerButton;
    [SerializeField] GameObject settingsAudioMenuFirstSelected;
    [SerializeField] GameObject settingsPCMenuFirstSelected;
    [SerializeField] GameObject settingsControllerMenuFirstSelected;

    [Header("Misc. Menu Buttons")]
    [SerializeField] GameObject pauseMenuFirstSelected;
    [SerializeField] GameObject pauseMenuSettingsButton;
    [SerializeField] GameObject winMenuFirstSelected;
    [SerializeField] GameObject loseMenuFirstSelected;
    [SerializeField] GameObject rulesMenuFirstSelected;
    [SerializeField] GameObject creditsMenuFirstSelected;
    #endregion

    #region Private Fields
    GameObject activeMenu;
    GameObject defaultSelectedButton;
    #endregion

    #region Startup

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startMenu != null)
        {
            ShowStartMenu();
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }

        if (settingsAudioMenu != null)
        {
            settingsAudioMenu.SetActive(false);
        }

        if (settingsPCMenu != null)
        {
            settingsPCMenu.SetActive(false);
        }

        if (settingsControllerMenu != null)
        {
            settingsControllerMenu.SetActive(false);
        }

        if (rulesMenu != null)
        {
            rulesMenu.SetActive(false);
        }

        if (creditsMenu != null)
        {
            creditsMenu.SetActive(false);
        }

        if (winMenu != null)
        {
            winMenu.SetActive(false);
        }

        if (loseMenu != null)
        {
            loseMenu.SetActive(false);
        }
    }

    #endregion

    void Update()
    {
        if (activeMenu != null && eventSystem.currentSelectedGameObject == null && InputActionManager.instance.menuNavigate.magnitude != 0.0f)
        {
            eventSystem.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    void InitializeStartMenu()
    {
        if (SaveSystem.HasSave())
        {
            eventSystem.firstSelectedGameObject = startMenuFirstSelected;
        }
        else
        {
            Button continueButton = startMenuFirstSelected.GetComponent<Button>();
            Button newGameButton = startMenuFirstSelectedNoSave.GetComponent<Button>();
            Button overworldButton = startMenuOverworldButton.GetComponent<Button>();
            Button settingsButton = startMenuSettingsButton.GetComponent<Button>();
            Button quitButton = startMenuQuitButton.GetComponent<Button>();

            if (continueButton != null)
            {
                continueButton.interactable = false;
            }

            if (overworldButton != null)
            {
                overworldButton.interactable = false;
            }

            if (newGameButton != null)
            {
                if (settingsButton != null)
                {
                    // Change the new game button navigation
                    // Navigating down now moves to settings
                    Navigation nav = newGameButton.navigation;
                    nav.selectOnDown = settingsButton;
                    newGameButton.navigation = nav;

                    // Change the settings button navigation
                    // Navigating up now moves to new game
                    nav = settingsButton.navigation;
                    nav.selectOnUp = newGameButton;
                    settingsButton.navigation = nav;
                }

                if (quitButton != null)
                {
                    // Change the new game button navigation
                    // Navigating up now moves to quit
                    Navigation nav = newGameButton.navigation;
                    nav.selectOnUp = quitButton;
                    newGameButton.navigation = nav;

                    // Change the quit button navigation
                    // Navigating down now moves to new game
                    nav = quitButton.navigation;
                    nav.selectOnDown = newGameButton;
                    quitButton.navigation = nav;
                }
            }

            eventSystem.firstSelectedGameObject = startMenuFirstSelectedNoSave;
        }
    }

    #region Button Common Code
    void SelectButton(GameObject button)
    {
        if (button == null)
        {
            return;
        }

        eventSystem.SetSelectedGameObject(button);
    }

    void UpdateSelectedButton()
    {
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
    #endregion

    #region Menu Common Code
    void ShowMenu(GameObject menu)
    {
        if (menu == null)
        {
            return;
        }

        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }

        activeMenu = menu;
        activeMenu.SetActive(true);
    }

    void HideMenu(GameObject menu)
    {
        if (menu == null)
        {
            return;
        }

        if (activeMenu != menu)
        {
            return;
        }

        activeMenu.SetActive(false);
        activeMenu = null;
    }
    #endregion

    public void CloseMenu()
    {
        HideMenu(activeMenu);
        eventSystem.firstSelectedGameObject = null;
        eventSystem.SetSelectedGameObject(null);
        GameManager.instance.StateUnpause();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    #region Show Menu Methods

    public void ShowStartMenu()
    {
        ShowMenu(startMenu);
        InitializeStartMenu();
        defaultSelectedButton = startMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowPauseMenu()
    {
        ShowMenu(pauseMenu);
        eventSystem.firstSelectedGameObject = pauseMenuFirstSelected;
        defaultSelectedButton = pauseMenuFirstSelected;
        UpdateSelectedButton();
        GameManager.instance.StatePause();
    }

    public void ShowSettingsMenu()
    {
        ShowMenu(settingsMenu);
        eventSystem.firstSelectedGameObject = settingsMenuFirstSelected;
        defaultSelectedButton = settingsMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowSettingsAudioMenu()
    {
        ShowMenu(settingsAudioMenu);
        eventSystem.firstSelectedGameObject = settingsAudioMenuFirstSelected;
        defaultSelectedButton = settingsAudioMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowSettingsPCMenu()
    {
        ShowMenu(settingsPCMenu);
        eventSystem.firstSelectedGameObject = settingsPCMenuFirstSelected;
        defaultSelectedButton = settingsPCMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowSettingsControllerMenu()
    {
        ShowMenu(settingsControllerMenu);
        eventSystem.firstSelectedGameObject = settingsControllerMenuFirstSelected;
        defaultSelectedButton = settingsControllerMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowRulesMenu()
    {
        ShowMenu(rulesMenu);
        eventSystem.firstSelectedGameObject = rulesMenuFirstSelected;
        defaultSelectedButton = rulesMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowCreditsMenu()
    {
        ShowMenu(creditsMenu);
        eventSystem.firstSelectedGameObject = creditsMenuFirstSelected;
        defaultSelectedButton = creditsMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowWinMenu()
    {
        GameManager.instance.StatePause();
        InputActionManager.instance.DisablePauseInput();
        ShowMenu(winMenu);
        eventSystem.firstSelectedGameObject = winMenuFirstSelected;
        defaultSelectedButton = winMenuFirstSelected;
        UpdateSelectedButton();
    }

    public void ShowLoseMenu()
    {
        GameManager.instance.StatePause();
        InputActionManager.instance.DisablePauseInput();
        ShowMenu(loseMenu);
        eventSystem.firstSelectedGameObject = loseMenuFirstSelected;
        defaultSelectedButton = loseMenuFirstSelected;
        UpdateSelectedButton();
    }

    #endregion

    #region Back Buttons

    // Back to Start Menu from Settings
    public void SettingsToStartMenu()
    {
        ShowStartMenu();
        eventSystem.firstSelectedGameObject = startMenuSettingsButton;
        UpdateSelectedButton();
    }

    // Back to Pause from Settings
    public void SettingsToPauseMenu()
    {
        ShowPauseMenu();
        eventSystem.firstSelectedGameObject = pauseMenuSettingsButton;
        UpdateSelectedButton();
    }

    // Back to Settings from Audio Settings
    public void SettingsAudioBack()
    {
        ShowSettingsMenu();
        eventSystem.SetSelectedGameObject(settingsMenuAudioButton);
    }

    // Back to Settings from Keyboard & Mouse Settings
    public void SettingsPCBack()
    {
        ShowSettingsMenu();
        eventSystem.SetSelectedGameObject(settingsMenuPCButton);
    }

    // Back to Settings from Controller Settings
    public void SettingsControllerBack()
    {
        ShowSettingsMenu();
        eventSystem.SetSelectedGameObject(settingsMenuControllerButton);
    }

    // Back to Start from Rules
    public void RulesBack()
    {
        ShowStartMenu();
        eventSystem.firstSelectedGameObject = startMenuRulesButton;
        UpdateSelectedButton();
    }

    // Back to Start from Credits
    public void CreditsBack()
    {
        ShowStartMenu();
        eventSystem.firstSelectedGameObject = startMenuCreditsButton;
        UpdateSelectedButton();
    }

    #endregion

}
