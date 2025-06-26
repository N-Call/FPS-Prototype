using UnityEngine;

public class SettingsInitializer : MonoBehaviour
{
    
    void Awake()
    {
        SaveSettingsSystem.Load();
    }

}
