
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
        LvlSelectManager.instance.StartGameBtn();
    }

    public void BackToOverWorld()
    {
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
        if (SceneManager.GetActiveScene().buildIndex + 1 <= 8)
        {
            SaveSystem.Save();
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
    public void LvlSelectScene()
    {
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

    public void Settings()
    {
        GameManager.instance.ToggleSettings();
    }
 
    public void Rules()
    {
        GameManager.instance.ToggleRules();
    }

    public void Credits()
    {
        GameManager.instance.ToggleCredits();
    }

    public void Save()
    {
        SaveSystem.Save();
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
