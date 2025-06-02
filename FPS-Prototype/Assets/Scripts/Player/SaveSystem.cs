using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();
    [System.Serializable]
    struct SaveData
    {
        public SceneSaveData SceneData;
        public FinalGradeData finalGradeData;
        public DifficultySaveData difficultyData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        Debug.Log("isSaving");
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        GameManager.instance.sceneData.Save(ref saveData.SceneData);
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

    private static void HandleLoadData()
    {
        GameManager.instance.sceneData.Load(saveData.SceneData);
        GameManager.instance.gradeSystem.Load(ref saveData.finalGradeData);
        DifficultyManager.Instance.Load(saveData.difficultyData);
    }

    private static void HandleLoadGradeData()
    {
        DifficultyManager.Instance.Load(saveData.difficultyData);
        GameManager.instance.gradeSystem.Load(ref saveData.finalGradeData);
    }
}
