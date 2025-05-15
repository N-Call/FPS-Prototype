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

    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
        SoundManager.instance.sfxSource.Stop();
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
