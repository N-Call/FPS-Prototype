using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO[] sceneDataSOArray;
    private Dictionary<string, int> sceneIDtoIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        GameManager.instance.sceneLoader = this;
        PopulateDictionary();
 
    }
    private void PopulateDictionary()
    {
        foreach (var scene in sceneDataSOArray)
        {
            sceneIDtoIndexMap[scene.UniqueName] = scene.sceneIndex;
        }
    }

    public void LoadSceneIndex(string savedSceneID)
    {
        if(sceneIDtoIndexMap.TryGetValue(savedSceneID, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("no scene found for Index: "+ savedSceneID);
        }
    }
}
