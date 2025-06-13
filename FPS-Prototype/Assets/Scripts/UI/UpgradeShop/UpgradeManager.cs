using UnityEngine;
using System.Collections.Generic;






public class UpgradeManager : MonoBehaviour
{
    int totalScrap = 10000;
    public List<UpgradeData> allUpgrades;
    int completed = 0;
    int total = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool SpendScrap(int amount)
    {
        if (totalScrap >= amount)
        {
            totalScrap -= amount;
            Debug.Log(totalScrap + "My Money");
            GameManager.instance.ShowScrap();

            return true;
        }
        return false;
    }
    public bool CanBuy(UpgradeData upgrade)
    {
        if (upgrade.isMajor)
        {
            Debug.Log("Can I buy major");
            return CanBuyMajor(upgrade);
        }
        else
        {
            Debug.Log("Maxed out lvl need to prompt player");
            return upgrade.currentLevel < upgrade.maxLevel &&
                   totalScrap >= upgrade.costPerLevel[upgrade.currentLevel];
        }
    }
    private bool CanBuyMajor(UpgradeData upgrade)
    {

        // Check if it's a weapon upgrade
        bool isWeapon = upgrade.category.ToString().Contains("Weapon 1");
        bool isWeapon2 = upgrade.category.ToString().Contains("Weapon 2");
        bool isWeapon3 = upgrade.category.ToString().Contains("Weapon 3");

        foreach (UpgradeData up in allUpgrades)
        {
            // For weapons, count all non-major weapon upgrades (across all weapon categories)
            if (isWeapon && !up.isMajor)
            {
                total++;
                if (up.currentLevel == up.maxLevel)
                {
                    completed++;
                }
            }
            else if (isWeapon2 && !up.isMajor)
            {
                total++;
                if (up.currentLevel == up.maxLevel)
                {
                    completed++;
                }
            }
            else if (isWeapon3 && !up.isMajor)
            {
                total++;
                if (up.currentLevel == up.maxLevel)
                {
                    completed++;
                }
            }

            // For movement or orbs, count only same-category minor upgrades
            else if (!isWeapon && up.category == upgrade.category && !up.isMajor)
            {
                total++;
                if (up.currentLevel == up.maxLevel)
                {
                    completed++;
                }
            }
        }
        // weapons : unlock after 10 of 15
        if (isWeapon)
        {
            Debug.Log("Checking for weapon");
            Debug.Log(completed);
            return completed >= 10 && totalScrap >= upgrade.majorCost;

        }
        // movement /orbs unlock after all upgrades
        return completed == total && totalScrap > +upgrade.majorCost;
    }

    public void BuyUpgrade(UpgradeData upgrade)
    {
        if (!CanBuy(upgrade)) return;

        if (upgrade.isMajor && upgrade.currentLevel < upgrade.maxLevel)
        {
            Debug.Log("I bought a major");
            SpendScrap(upgrade.majorCost);
            upgrade.currentLevel++;
            ApplyMajorUpgrade(upgrade);
        }
        else
        {
            Debug.Log("I bought minors");
            int cost = upgrade.costPerLevel[upgrade.currentLevel];
            SpendScrap(cost);
            upgrade.currentLevel++;
            ApplyMinorUpgrade(upgrade);
        }
    }

    private void ApplyMinorUpgrade(UpgradeData upgrade)
    {
        Debug.Log("Minor upgrade applied: " + upgrade.name + " to level " + upgrade.currentLevel);
        // Apply minor upgrade effect here

    }

    private void ApplyMajorUpgrade(UpgradeData upgrade)
    {
        Debug.Log("Major upgrade unlocked: " + upgrade.name);
        // Apply major upgrade effect here
    }
}
