using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Reflection;

public class ImageHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image hoverImage;
    public Image initialImage;

    public UpgradeData upgradeData;
    public TMP_Text upText;

    void Start()
    {
        LoadUpgradeLevel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.BuyUpgrade(upgradeData);

        if (upText != null)
            upText.text = upgradeData.currentLevel.ToString();

        SaveUpgradeLevel();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
            hoverImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverImage != null)
            hoverImage.gameObject.SetActive(false);
    }

    private void LoadUpgradeLevel()
    {
        if (upgradeData == null || string.IsNullOrEmpty(upgradeData.upgradeID)) return;

        var abilities = GameManager.instance.playerAbilities;
        var field = typeof(PlayerAbilities).GetField(upgradeData.upgradeID);

        if (field != null && field.FieldType == typeof(int))
        {
            upgradeData.currentLevel = (int)field.GetValue(abilities);
            if (upText != null)
                upText.text = upgradeData.currentLevel.ToString();

            Debug.Log($"[Load] {upgradeData.upgradeID} = {upgradeData.currentLevel}");
        }
    }

    private void SaveUpgradeLevel()
    {
        if (upgradeData == null || string.IsNullOrEmpty(upgradeData.upgradeID)) return;

        var abilities = GameManager.instance.playerAbilities;
        var field = typeof(PlayerAbilities).GetField(upgradeData.upgradeID);

        if (field != null && field.FieldType == typeof(int))
        {
            field.SetValue(abilities, upgradeData.currentLevel);
            Debug.Log($"[Save] {upgradeData.upgradeID} = {upgradeData.currentLevel}");
        }

        // Save major upgrade flag if max level is reached
        if (!string.IsNullOrEmpty(upgradeData.majorUpgradeID) && upgradeData.currentLevel >= upgradeData.maxLevel)
        {
            var majorField = typeof(PlayerAbilities).GetField(upgradeData.majorUpgradeID);

            if (majorField != null && majorField.FieldType == typeof(bool))
            {
                majorField.SetValue(abilities, true);
                Debug.Log($"[Major Save] {upgradeData.majorUpgradeID} = true");
            }
        }
    }
}