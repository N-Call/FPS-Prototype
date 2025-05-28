using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    
    [SerializeField] GameObject ActiveImage;

    public int SelectedScene;

    private void Awake()
    {
        instance = this;
        
    }

    public void StartGameBtn()
    {
        if(SelectedScene != 0)
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


        //if (ActiveRecord != null)
        //{
        //    ActiveRecord.SetActive(false);
        //}

        //ActiveRecord = lvlRecords[Index];
        //ActiveRecord.SetActive(true);

        SelectedScene = Index + 2;
    }


}
