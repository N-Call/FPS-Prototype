using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeID;
    public string majorUpgradeID;

    public UpgradeCategory category;
    public UpgradeType type;
    public int maxLevel = 5;
    public int[] costPerLevel = new int[5];
    public int currentLevel = 0;

    public bool isMajor = false;
    public int majorCost = 100;
}