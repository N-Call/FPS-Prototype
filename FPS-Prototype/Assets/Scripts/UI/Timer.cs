using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerCount;
    [SerializeField] float timerDelay;

    float elapsedDelayTime;
    public float elapsedTime;

    // Update is called once per frame
    void Update()
    {
        if (timerDelay >= 0.0f)
        {
            elapsedDelayTime += Time.deltaTime;
            if (elapsedDelayTime < timerDelay)
            {
                return;
            }
        }
        
        // this is to count up one second at a time
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerCount.text = string.Format("{0:00}:{1:00}",minutes,seconds);

        
    }
    public void DisplayTimeAdded(float timeCount)
    {
        
        elapsedTime = GameManager.instance.EnemyTimePenalty(timeCount);
    }
    
}
