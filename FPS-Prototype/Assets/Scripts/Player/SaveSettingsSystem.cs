using UnityEngine;
using System.IO;
public class SaveSettingsSystem : MonoBehaviour
{
    private static SaveData saveData = new SaveData();
    [System.Serializable]
    struct SaveData
    {
        public VolumeSaveData volumeSaveData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/settings" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        GameManager.instance.volumeSystemData.Save(ref saveData.volumeSaveData);
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
        GameManager.instance.volumeSystemData.Load(saveData.volumeSaveData);
    }
}
