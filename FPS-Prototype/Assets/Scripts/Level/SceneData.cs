using UnityEngine;

public class SceneData : MonoBehaviour
{
    public SceneDataSO sceneData;

    private void Awake()
    {
        GameManager.instance.sceneData = this;
    }

    #region Save and Load
    public void Save(ref SceneSaveData data)
    {
        data.sceneID = sceneData.UniqueName;
    }

    public void Load(SceneSaveData data)
    {
        GameManager.instance.sceneLoader.LoadSceneIndex(data.sceneID);
    }

    #endregion
}

[System.Serializable]

public struct SceneSaveData
{
    public string sceneID;
}