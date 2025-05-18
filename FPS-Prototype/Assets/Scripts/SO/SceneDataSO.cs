using UnityEngine;

[CreateAssetMenu(menuName = "Scene Data", fileName = "New Scene Data")]
public class SceneDataSO : ScriptableObject
{
    public string UniqueName;
    public int sceneIndex;
}
