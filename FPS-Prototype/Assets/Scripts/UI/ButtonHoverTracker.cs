using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverTracker : MonoBehaviour, IPointerEnterHandler
{

    public static GameObject lastHighlighted;

    public void OnPointerEnter(PointerEventData eventData)
    {
        lastHighlighted = gameObject;
        MenuManager.instance.RemoveCurrentlySelectedButton();
    }

}
