
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
        GameManager.instance.ResetRules();
        SceneManager.LoadScene(4);
    }

    public void LvlStartGame()
    {
        
        LvlSelectManager.instance.StartGameBtn();
    }

    public void BackToOverWorld()
    {
        
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
        
    }
    public void BackToOverWorldShopOnly()
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
        
        SoundManager.instance.sfxSource.Stop();
        GameManager.instance.StateUnpause();
    }

    public void NextLevel()
    {
        // this is to load the next level but does a check first on making sure your in scene count 
        if (SceneManager.GetActiveScene().buildIndex + 1 <= 14)
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            SoundManager.instance.sfxSource.Stop();
            GameManager.instance.StateUnpause();
        }
    }
    public void LvlOverWorldSave()
    {
        
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
    }
    public void LvlOverWorldLoad()
    {
        
        Time.timeScale = GameManager.instance.timeScaleOrig;
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(2);
    }

    public void LvlSelectStartScene()
    {
        
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
