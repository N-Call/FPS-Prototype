using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public string[] lines;
    public string[] speaker;

    public float textSpeed;
    public float speedBetweenText;

    bool ran;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !ran)
        {
            ran = true;
            DialogueManager.instance.StartDialogue(this);
        } 
    }
}