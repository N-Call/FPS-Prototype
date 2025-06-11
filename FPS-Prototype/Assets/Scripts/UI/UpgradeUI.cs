using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{

    public Image[] tabImages;
    public GameObject[] pages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActiveTab(0);
    }

    public void ActiveTab(int tabNum)
    {
        for (int i = 0; i < pages.Length;i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNum].SetActive(true);
        tabImages[tabNum].color = Color.white;
    }

    
}
