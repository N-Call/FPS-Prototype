
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{
   
    public void Respawn()
    {
        if (GameManager.instance.respawnPosition == GameManager.instance.startPos)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            MenuManager.instance.CloseMenu();
            GameManager.instance.Respawn();
        }

        GameManager.instance.StateUnpause();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(4);
    }

    public void LvlStartGame()
    {
        SaveSystem.LoadGrades();
        LvlSelectManager.instance.StartGameBtn();
    }

    public void BackToOverWorld()
    {
        SaveSystem.SaveStats();
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
        
    }

    public void MainMenu()
    {
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(0);
    }

    public void SetLevelBtn(int level)
    {
        SaveSystem.LoadGrades();
        LvlSelectManager.instance.Setlevel(level);
        GameManager.instance.LoadScoreBoard();
    }
  
    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SaveSystem.LoadGrades();
        SoundManager.instance.sfxSource.Stop();
        GameManager.instance.StateUnpause();
    }

    public void NextLevel()
    {
        // this is to load the next level but does a check first on making sure your in scene count 
        if (SceneManager.GetActiveScene().buildIndex + 1 <= 8)
        {
            Debug.Log("I have not made it to next scene");
            SaveSystem.LoadGrades();
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            
            SoundManager.instance.sfxSource.Stop();
            GameManager.instance.StateUnpause();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            // only works after the button is clicked if prefab is updated of UI then go to last lvl turn off next button and save scene
            GameManager.instance.NextLvlBtnOff();
        }
    }
    public void LvlOverWorldSave()
    {
        SaveSystem.SaveStats();
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
    }
    public void LvlOverWorldLoad()
    {
        SaveSystem.LoadGrades();
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
    }

    public void LvlSelectStartScene()
    {
        SaveSystem.LoadGrades();
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2); 
    }


    public void OnSelectDifficulty(int difficultyLevel)
    {
        DifficultyManager.Instance.SetDifficulty((EDifficultyLevel)difficultyLevel);
        StartGame();
    }

    public void StartMenu()
    {
        MenuManager.instance.ShowStartMenu();
    }

    public void Settings()
    {
        MenuManager.instance.ShowSettingsMenu();
    }

    public void SettingsAudio()
    {
        MenuManager.instance.ShowSettingsAudioMenu();
    }

    public void SettingsAudioBack()
    {
        MenuManager.instance.SettingsAudioBack();
    }

    public void SettingsPC()
    {
        MenuManager.instance.ShowSettingsPCMenu();
    }

    public void SettingsPCBack()
    {
        MenuManager.instance.SettingsPCBack();
    }

    public void SettingsController()
    {
        MenuManager.instance.ShowSettingsControllerMenu();
    }

    public void SettingsControllerBack()
    {
        MenuManager.instance.SettingsControllerBack();
    }

    public void Rules()
    {
        MenuManager.instance.ShowRulesMenu();
    }

    public void Credits()
    {
        MenuManager.instance.ShowCreditsMenu();
    }

    public void Save()
    {
        SaveSystem.Save();
        SaveSystem.SaveStats();
    }

    public void SaveSetting()
    {
        SaveSettingsSystem.Save();
    }
    public void Load()
    {
        SaveSystem.Load();
    }

    public void Quit()
    {// allows you to quit the app from Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
