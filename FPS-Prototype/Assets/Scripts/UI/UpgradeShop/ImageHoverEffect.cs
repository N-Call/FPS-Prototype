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


    void Start()
    {
        LoadUpgradeLevel();

        if (upText != null)
        {
            Debug.Log($"[{upgradeData.name}] Level on load: {upgradeData.currentLevel}");
            upText.text = upgradeData.currentLevel.ToString();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("point is down");
        GameManager.instance.BuyUpgrade(upgradeData);

        if (upText != null)
        {
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
        if (hoverImage != null)
        {
            hoverImage.gameObject.SetActive(false); // Hide hover image
        }
    }

    private void LoadUpgradeLevel()
    {
        if (upgradeData == null || string.IsNullOrEmpty(upgradeData.upgradeID))
        {
            return;
        }
        // this is to get the upgradeID in the Script Objects of the upgrades
        var field = typeof(PlayerAbilities).GetField(upgradeData.upgradeID);

        if (field != null && field.FieldType == typeof(int))
        {
            upgradeData.currentLevel = (int)field.GetValue(GameManager.instance.playerAbilities);
            if (upText != null)
            {
                upText.text = upgradeData.currentLevel.ToString();
            }

            Debug.Log($"[Load] {upgradeData.upgradeID} = {upgradeData.currentLevel}");
        }
    }
}