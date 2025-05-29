using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalGradeSystem : MonoBehaviour
{
    public int enemyCount;
    public string finalTime;
    public string finalGrade;

    private int levelIndex;

    Dictionary<int, List<string>> scoreDataList = new Dictionary<int, List<string>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }
    private void OnLevelWasLoaded(int level)
    {
        GameManager.instance.gradeSystem = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveFinal(int count, string time, string grade)
    {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        List<string> scores = new List<string>
        {
            "" + count,
            time,
            grade
        };

        if (scoreDataList.TryGetValue(levelIndex, out List<string> data))
        {

            if (ConvertToInt(data[1]) > ConvertToInt(scores[1]))
            {
                data = scores;
            }
        }
        scoreDataList.TryAdd(levelIndex, scores);
    }

    private int ConvertToInt(string time)
    {
        string[] parts = time.Split(':');

        if (parts.Length == 2 &&
            int.TryParse(parts[0], out int minutes) &&
            int.TryParse(parts[1], out int seconds))
        {
            return (minutes * 60 + seconds);
        }
        return 0;
    }

    public bool LoadData(int index)
    {
        if(scoreDataList.TryGetValue(index, out List<string> data))
        {
            enemyCount = int.Parse(data[0]);
            finalTime = data[1];
            finalGrade = data[2];

            return true;
        }
        return false;
    }


    #region Save and Load
    public void Save(ref FinalGradeData data)
    {
        data.scoreDataList = scoreDataList;
    }

    public void Load(FinalGradeData data)
    {
        scoreDataList = data.scoreDataList;
    }

    #endregion
}

[System.Serializable]
public struct FinalGradeData
{
    public Dictionary<int, List<string>> scoreDataList;
}
