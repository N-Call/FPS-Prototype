using UnityEngine;
using System.IO;

public class SaveSystem
{

    private static SaveData saveData = new SaveData();

    [System.Serializable]
    struct SaveData
    {
        public SceneSaveData SceneData;
        public ScrapSaveData ScrapData;
        public FinalGradeData finalGradeData;
        public DifficultySaveData difficultyData;
        public AbilitiesSaveData abilitiesData;
        public SettingsSaveData settingsData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static bool HasSave()
    {
        return File.Exists(SaveFileName());
    }

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        GameManager.instance.scrapManager.Save(ref saveData.ScrapData);
        GameManager.instance.sceneData.Save(ref saveData.SceneData);
        GameManager.instance.gradeSystem.Save(ref saveData.finalGradeData);
        GameManager.instance.playerAbilities.Save(ref saveData.abilitiesData);
        GameManager.instance.playerSettings.Save(ref saveData.settingsData);
        DifficultyManager.Instance.Save(ref saveData.difficultyData);
    }

    public static void SaveSettings()
    {
        GameManager.instance.playerSettings.Save(ref saveData.settingsData);
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    public static void SaveStats()
    {
        HandleSaveStatsData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveStatsData()
    {
        GameManager.instance.scrapManager.Save(ref saveData.ScrapData);
        GameManager.instance.playerAbilities.Save(ref saveData.abilitiesData);
        GameManager.instance.gradeSystem.Save(ref saveData.finalGradeData);
        DifficultyManager.Instance.Save(ref saveData.difficultyData);
    }


    public static void Load()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveContent = File.ReadAllText(SaveFileName());

            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            HandleLoadData();
        }
    }

    public static void LoadGrades()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveContent = File.ReadAllText(SaveFileName());

            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            HandleLoadGradeData();
        }
    }

    public static void LoadScraps()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveContent = File.ReadAllText(SaveFileName());

            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            HandleLoadScrapData();
        }
    }

    private static void HandleLoadData()
    {
        GameManager.instance.sceneData.Load(saveData.SceneData);
        GameManager.instance.gradeSystem.Load(ref saveData.finalGradeData);
        DifficultyManager.Instance.Load(saveData.difficultyData);
        GameManager.instance.scrapManager.Load(saveData.ScrapData);
        GameManager.instance.playerAbilities.Load(saveData.abilitiesData);
        GameManager.instance.playerSettings.Load(saveData.settingsData);
    }

    private static void HandleLoadGradeData()
    {
        DifficultyManager.Instance.Load(saveData.difficultyData);
        GameManager.instance.gradeSystem.Load(ref saveData.finalGradeData);
        GameManager.instance.scrapManager.Load(saveData.ScrapData);
        GameManager.instance.playerAbilities.Load(saveData.abilitiesData);
    }

    private static void HandleLoadScrapData()
    {
        Debug.Log(saveData.ScrapData.scraps);
        GameManager.instance.scrapManager.Load(saveData.ScrapData);
        GameManager.instance.playerAbilities.Load(saveData.abilitiesData);
    }

}
