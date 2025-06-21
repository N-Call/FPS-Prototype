using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine;

public class ImageHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image hoverImage;
    public Image initialImage;

    public UpgradeData upgradeData;

    public TMP_Text upText;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("point is down");
        GameManager.instance.BuyUpgrade(upgradeData);
        if (upText != null)
        {// need to add UI count to major Upgrades
            upText.text = upgradeData.currentLevel.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
        {
            hoverImage.gameObject.SetActive(true); // Show hover image
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverImage.gameObject.SetActive(false); // Hide hover image
    }
}