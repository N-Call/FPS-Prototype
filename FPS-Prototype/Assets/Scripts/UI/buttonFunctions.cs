using Unity.VisualScripting;
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
        // this is for the start game menu after button pushed then moves to first level
        SceneManager.LoadScene(1);
    }

    public void LvlStartGame()
    {
        LvlSelectManager.instance.StartGameBtn();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetLevelBtn(int level)
    {
        LvlSelectManager.instance.Setlevel(level);
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
        if (SceneManager.GetActiveScene().buildIndex + 1 <= 5)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            SoundManager.instance.sfxSource.Stop();
            GameManager.instance.StateUnpause();
        }//else if at end sends you to level selection menu 
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

    public void Load()
    {
        SaveSystem.Load();
        GameManager.instance.StateUnpause();
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
