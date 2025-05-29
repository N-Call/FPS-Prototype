using UnityEngine;

public class StartScreenScript : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.SetOnStartScreen(true);
    }

}
