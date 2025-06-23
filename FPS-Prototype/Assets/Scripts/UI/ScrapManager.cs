using UnityEngine;

public class ScrapManager : MonoBehaviour
{
    public int totalScrap;

    private void OnLevelWasLoaded(int level)
    {
        GameManager.instance.scrapManager = this;
    }


    #region Save and Load
    public void Save(ref ScrapSaveData data)
    {
        data.scraps = totalScrap;
    }

    public void Load(ScrapSaveData data)
    {
        totalScrap = data.scraps;
    }

    #endregion
}

[System.Serializable]

public struct ScrapSaveData
{
    public int scraps;
}