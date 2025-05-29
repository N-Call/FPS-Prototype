using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();
    [System.Serializable]
    struct SaveData
    {
        public SceneSaveData SceneData;
        //public FinalGradeData finalGradeData;
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
        //GameManager.instance.gradeSystem.Save(ref saveData.finalGradeData);
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

    private static void HandleLoadData()
    {
        GameManager.instance.sceneData.Load(saveData.SceneData);
        //GameManager.instance.gradeSystem.Load(ref saveData.finalGradeData);

    }
}
