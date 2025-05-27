using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public IEnumerator activeCoroutine = null;
    
    private int index;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        StopText();
        GameManager.instance.speakerUI.text = string.Empty;
        GameManager.instance.textComponent.text = string.Empty;
        GameManager.instance.textPopUp.SetActive(true);
        index = 0;
        if (activeCoroutine == null)
        {
            activeCoroutine = RunDialogue(dialogue);
            StartCoroutine(activeCoroutine);
        }
    }

    public void StopDialogue()
    {
        StopText();
        GameManager.instance.textPopUp.SetActive(false);
    }

    private void StopText()
    { 
        if (activeCoroutine != null)
        { 
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    IEnumerator RunDialogue(Dialogue dialogue)
    {
        while (index < dialogue.lines.Length)
        {
            GameManager.instance.textComponent.text = string.Empty;

            if (GameManager.instance.speakerUI.text == string.Empty)
            {
                foreach (char c in dialogue.speaker[index].ToCharArray())
                {
                    GameManager.instance.speakerUI.text += c;
                    yield return new WaitForSeconds(dialogue.textSpeed);
                }
                foreach (char c in dialogue.lines[index].ToCharArray())
                {
                    GameManager.instance.textComponent.text += c;
                    yield return new WaitForSeconds(dialogue.textSpeed);
                }
                yield return new WaitForSeconds(dialogue.speedBetweenText);
                index++;
            }
            else if (GameManager.instance.speakerUI.text != string.Empty)
            {
                foreach (char c in dialogue.lines[index].ToCharArray())
                {
                    GameManager.instance.textComponent.text += c;
                    yield return new WaitForSeconds(dialogue.textSpeed);
                }
                yield return new WaitForSeconds(dialogue.speedBetweenText);
                index++;
            }
        }
        GameManager.instance.textPopUp.SetActive(false);
        activeCoroutine = null;
    }
}
