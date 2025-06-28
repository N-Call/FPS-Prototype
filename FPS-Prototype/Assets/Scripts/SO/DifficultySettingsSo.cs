using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySettings", menuName = "Game/Difficulty Settings")]
public class DifficultySettingsSo : ScriptableObject
{
    public EDifficultyLevel difficulty;
    [Range(0.1f, 3)] public float enemyDamageMultiplier = 1f;
    [Range(0.1f, 3)] public float enemyAttackRateMultiplier = 1f;
    [Range(1, 20)] public int enemyAttackCount = 1;
    [Range(1, 2)] public float spawnRateMultiplier = 1f;
    [Range(1, 5)] public float laserDmgMod = 1;

}