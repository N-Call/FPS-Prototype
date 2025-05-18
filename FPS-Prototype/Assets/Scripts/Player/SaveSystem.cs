using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();
    [System.Serializable]
    struct SaveData
    {
        public PlayerSaveData playerData;
        public SceneSaveData SceneData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        GameManager.instance.playerScript.Save(ref saveData.playerData);
        GameManager.instance.sceneData.Save(ref saveData.SceneData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameManager.instance.playerScript.Load(saveData.playerData);
        GameManager.instance.sceneData.Load(saveData.SceneData);
    }
}
