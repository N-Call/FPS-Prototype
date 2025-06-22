using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
  
    public void OnPointerEnter(PointerEventData eventData)
    { 
        SoundManager.instance.PlaySFX("hoverClip");
    }

    public void OnPointerClick(PointerEventData eventData)
    { 
        SoundManager.instance.PlaySFX("clickClip");
        // Optional: deselect after click
        EventSystem.current.SetSelectedGameObject(null);
    }
}

