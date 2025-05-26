using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlSelectManager : MonoBehaviour
{
    public static LvlSelectManager instance;

    [Header("Images")]
    // put is sprite array 
    [SerializeField] GameObject Lvl1;
    [SerializeField] GameObject Lvl2;
    [SerializeField] GameObject Lvl3;
    [SerializeField] GameObject Lvl4;
    [SerializeField] GameObject Lvl5;
    [Header("Buttons")]
    // look at lists/array
    [SerializeField] GameObject Btnlvl1;
    [SerializeField] GameObject Btnlvl2;
    [SerializeField] GameObject Btnlvl3;
    [SerializeField] GameObject Btnlvl4;
    [SerializeField] GameObject Btnlvl5;
    [SerializeField] GameObject StartGame;

    [SerializeField] GameObject ActiveBtn;

    public int SelectedScene;

    private void Awake()
    {
        instance = this;
    }

    public void StartGameBtn()
    {
        SceneManager.LoadScene(SelectedScene);
    }

    public void lvl1()
    {
        if (ActiveBtn == null)
        {
            // set scene manager index to scene of the btn
            ActiveBtn = Btnlvl1;
            SelectedScene = 1;
            Debug.Log(SelectedScene);
            Lvl1.SetActive(true);
        }
        else
        {
            ActiveBtn = null;
        }
        // show your best times of that lvl 
        // on button click background image needs to pop up
        // start game should be only button that take you to the lvlv
    }
}
