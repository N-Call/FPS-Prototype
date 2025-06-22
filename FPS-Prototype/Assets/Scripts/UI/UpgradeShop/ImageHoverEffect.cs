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
            upText.text = GetUpgradeDisplayValue().ToString();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("point is down");
        GameManager.instance.BuyUpgrade(upgradeData);

        if (upText != null)
        {
            upText.text = GetUpgradeDisplayValue().ToString();
        }
    }
    private object GetUpgradeDisplayValue()
    {
        var abilities = GameManager.instance.playerAbilities;

        switch (upgradeData.category)
        {
            case UpgradeCategory.Weapon1:
                switch (upgradeData.type)
                {
                    case UpgradeType.Damage: return abilities.w1DmgMod;
                    case UpgradeType.Speed: return abilities.w1AmmoMag;
                    case UpgradeType.Rate: return abilities.w1RateMod;
                    case UpgradeType.Major: return abilities.w1Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.Weapon2:
                switch (upgradeData.type)
                {
                    case UpgradeType.Damage: return abilities.w2DmgMod;
                    case UpgradeType.Speed: return abilities.w2SpeedMod;
                    case UpgradeType.Rate: return abilities.w2RateMod;
                    case UpgradeType.Major: return abilities.w2Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.Weapon3:
                switch (upgradeData.type)
                {
                    case UpgradeType.Damage: return abilities.w3DmgMod;
                    case UpgradeType.Speed: return abilities.w3SpeedMod;
                    case UpgradeType.Rate: return abilities.w3RateMod;
                    case UpgradeType.Major: return abilities.w3Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.Slide:
                switch (upgradeData.type)
                {
                    case UpgradeType.SlideSpeed: return abilities.moveSlideSpeed;
                    case UpgradeType.Major: return abilities.slideMajor ? 1 : 0;
                }
                break;
            case UpgradeCategory.WallRun:
                switch (upgradeData.type)
                {
                    case UpgradeType.WallRunSpeed: return abilities.moveWallRunSpeed;
                    case UpgradeType.WallRunJump: return abilities.moveWallRunJump;
                    case UpgradeType.Major: return abilities.wallRunMajor ? 1 : 0;
                }
                break;
            case UpgradeCategory.OrbSpeed:
                switch (upgradeData.type)
                {
                    case UpgradeType.OrbStrength: return abilities.o1Srt;
                    case UpgradeType.OrbDuration: return abilities.o1Dur;
                    case UpgradeType.Major: return abilities.o1Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.OrbJump:
                switch (upgradeData.type)
                {
                    case UpgradeType.OrbStrength: return abilities.o2Srt;
                    case UpgradeType.OrbDuration: return abilities.o2Dur;
                    case UpgradeType.Major: return abilities.o2Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.OrbShield:
                switch (upgradeData.type)
                {
                    case UpgradeType.OrbStrength: return abilities.o3Srt;
                    case UpgradeType.OrbDuration: return abilities.o3Dur;
                    case UpgradeType.Major: return abilities.o3Major ? 1 : 0;
                }
                break;
            case UpgradeCategory.OrbTime:
                switch (upgradeData.type)
                {
                    case UpgradeType.OrbStrength: return abilities.o4Srt;
                    case UpgradeType.OrbDuration: return abilities.o4Dur;
                    case UpgradeType.Major: return abilities.o4Major ? 1 : 0;
                }
                break;
        }

        return "-";
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