using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public Vector3 position;

    private void OnLevelWasLoaded(int level)
    {
        GameManager.instance.playerPosition = this;
    }

    //Save
    public void Save(ref PlayerPositionData data)
    {
        data.position = position;
    }
    //load
    public void Load(PlayerPositionData data)
    {
        position = data.position;
    }

    [System.Serializable]
    public struct PlayerPositionData
    {
        public Vector3 position;
    }

    private static string SavePath => Application.persistentDataPath + "/playerpos.json";

    public void SaveToFile()
    {
        PlayerPositionData data = new PlayerPositionData { position = GameManager.instance.player.transform.position };
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(SavePath, json);
    }

    public void LoadFromFile()
    {
        if (System.IO.File.Exists(SavePath))
        {
            string json = System.IO.File.ReadAllText(SavePath);
            PlayerPositionData data = JsonUtility.FromJson<PlayerPositionData>(json);
            GameManager.instance.player.transform.position = data.position;
        }
    }

}
