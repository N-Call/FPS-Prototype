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
    public void LevelStartGame()
    {
        // need the button to take what level is selected in the
        // level selecte scene and start that scene if only that level is unlocked 
    }
    // buttons needed for level scelection levels need to be made first 5 in total  

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
        if (SceneManager.GetActiveScene().buildIndex + 1 <= 3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            SoundManager.instance.sfxSource.Stop();
            GameManager.instance.StateUnpause();
        }
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
