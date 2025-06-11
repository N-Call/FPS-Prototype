
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
    // look at lists/array
    //public GameObject[] lvlRecords;

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
        if (Input.GetButtonDown("Fire1") && BackButton.CompareTag("buttonLvl"))
        {
            return;
        }
        else
        {
            for (int i = 0; i < lvlsBtn.Length; i++)
            {
                if (!GameManager.instance.gradeSystem.LoadData(i + 3))
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


        if (GameManager.instance.gradeSystem.LoadData(Index + 3))
        {
            ActiveRecord.SetActive(true);
        }
        else
        {
            ActiveRecord.SetActive(false);
        }

        SelectedScene = Index + 3;
    }


}
