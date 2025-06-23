
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LvlSelectManager : MonoBehaviour
{
    public static LvlSelectManager instance;

    [Header("Images")]
    // put is sprite array
    public GameObject[] lvlIamges;

    [Header("Buttons")]
    
    [SerializeField] GameObject StartGame;

    [SerializeField] Button[] lvlsBtn;
    [SerializeField] GameObject ActiveImage;
    [SerializeField] GameObject ActiveRecord;

    [SerializeField] GameObject BackButton;

    public int SelectedScene;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        bool nextlvl = false;
       
        for (int i = 0; i < lvlsBtn.Length; i++)
        {// plus 4 if for all of the menu scenes in front of level 1 and on
            if (!GameManager.instance.gradeSystem.LoadData(i + 4))
            {
                if (!nextlvl)
                {
                    nextlvl = true;
                }
                else
                {
                    lvlsBtn[i].interactable = false;
                }
            }

        }
    }

    public void StartGameBtn()
    {
        if (SelectedScene != 0)
            SceneManager.LoadScene(SelectedScene);

        Time.timeScale = GameManager.instance.timeScaleOrig;
    }
 
    public void Setlevel(int Index)
    {
        // set scene manager index to scene of the btn
        if (ActiveImage != null)
        {
            ActiveImage.SetActive(false);
        }

        ActiveImage = lvlIamges[Index];
        ActiveImage.SetActive(true);


        if (GameManager.instance.gradeSystem.LoadData(Index + 4))
        {
            ActiveRecord.SetActive(true);
        }
        else
        {
            ActiveRecord.SetActive(false);
        }

        SelectedScene = Index + 4;
    }


}
