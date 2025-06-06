using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioSource audioSource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickClip);
        }

        // Optional: deselect after click
        EventSystem.current.SetSelectedGameObject(null);
    }
}

