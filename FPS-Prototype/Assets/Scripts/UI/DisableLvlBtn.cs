using UnityEngine;
using UnityEngine.UI;

public class DisableLvlBtn : MonoBehaviour
{
    [SerializeField] Button[] lvlsBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < lvlsBtn.Length; i++)
        {
            if (!GameManager.instance.gradeSystem.LoadData(i + 2))
            {
                lvlsBtn[i].interactable = false;
            }

        }
    }
}
