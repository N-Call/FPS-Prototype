
using UnityEngine;
using TMPro;
public class GradeSystem : MonoBehaviour
{
    [SerializeField] TMP_Text GradeLetter;

    public int TimeS;
    public int TimeA;
    public int TimeB;
    public int TimeC;
    public int TimeD;
    public int TimeE;
    
    
    public void GradeSystemWin(float Time)
    {
        {
            if (Time <= TimeS)
            {
                GradeLetter.text = "S";
                Debug.Log("Grade S");
            }
            else if (Time >= TimeS && Time <= TimeA)
            {
                GradeLetter.text = "A";
                Debug.Log("Grade A");
            }
            else if (Time >= TimeA && Time <= TimeB)
            {
                GradeLetter.text = "B";
                Debug.Log("Grade B");
            }
            else if (Time >= TimeB && Time <= TimeC)
            {
                GradeLetter.text = "C";
                Debug.Log("Grade C");
            }
            else if (Time >= TimeC && Time <= TimeD)
            {
                GradeLetter.text = "D";
                Debug.Log("Grade D");
            }
            else if (Time >= TimeD && Time <= TimeE)
            {
                GradeLetter.text = "E";
                Debug.Log("Grade E");
            }
            else 
            {
                GradeLetter.text = "F";
                Debug.Log("Grade F");
            }
           
        }
    }
    // then find time of player of end time
    // if or math for A,B,C,D,F grade for time 
    // then display in win menu
}
