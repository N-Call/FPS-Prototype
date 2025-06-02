using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [SerializeField] private DifficultySettingsSo[] difficultySettings;
    public DifficultySettingsSo currentSettings;

    void Awake()
    {
        if (Instance == null) 
        { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(EDifficultyLevel settings)
    {
        foreach(DifficultySettingsSo level in difficultySettings)
        {
            if(level.difficulty == settings)
            {
                currentSettings = level;
                return;
            }
        }
    }

    #region Save and Load
    public void Save(ref DifficultySaveData data)
    {
        data.difficulty = (int)currentSettings.difficulty;
    }

    public void Load(DifficultySaveData data)
    {
        SetDifficulty((EDifficultyLevel)data.difficulty);
    }

    #endregion
}

[System.Serializable]

public struct DifficultySaveData
{
    public int difficulty;
}
