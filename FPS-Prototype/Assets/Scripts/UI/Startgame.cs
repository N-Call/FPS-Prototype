using System.Collections;
using UnityEngine;

public class Startgame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartGameTime());
    }

    IEnumerator StartGameTime()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.StartGame();
    }

}
