
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
                GameManager.instance.textComponent.text = "Leave some for the rest of us! Actually don't. You know what you're doing.";
            }
            else if (Time >= TimeS && Time <= TimeA)
            {
                GradeLetter.text = "A";
                GameManager.instance.textComponent.text = "Well done, kid. Gold star! I knew you had it in you.";
            }
            else if (Time >= TimeA && Time <= TimeB)
            {
                GradeLetter.text = "B";
                GameManager.instance.textComponent.text = "Keep it going, kid! Those bots ain't gonna just wait for ya.";
            }
            else if (Time >= TimeB && Time <= TimeC)
            {
                GradeLetter.text = "C";
                GameManager.instance.textComponent.text = "Eh. Middle of the road. I know you can do better.";
            }
            else if (Time >= TimeC && Time <= TimeD)
            {
                GradeLetter.text = "D";
                GameManager.instance.textComponent.text = "You want out, don't ya? Try not to stop and shop next time. I've got places to be.";
            }
            else if (Time >= TimeD && Time <= TimeE)
            {
                GradeLetter.text = "E";
                GameManager.instance.textComponent.text = "Almost fell asleep! Sight seeing is beautiful this time of year, isn't it?";
            }
            else 
            {
                GradeLetter.text = "F";
                GameManager.instance.textComponent.text = "Kid... I know I should have rescued someone else. There is no one else but it's that bad.";
            }
           
        }
    }
}
