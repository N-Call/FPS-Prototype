using UnityEngine;

public class SpawnerRandom : Spawner
{

    [Header("Randomized Spawn Settings")]
    [SerializeField]
    [Tooltip("Prevent the same randomized location from being used twice in a row?")]
    bool preventSameLocation;

    int lastIndex = -1;

    protected override int GetPositionIndex()
    {
        if (spawnPositions.Length < 1)
        {
            return 0;
        }

        int index = Random.Range(0, spawnPositions.Length);
        if (!preventSameLocation)
        {
            return index;
        }

        while (true)
        {
            index = Random.Range(0, spawnPositions.Length);
            if (index != lastIndex)
            {
                break;
            }
        }

        lastIndex = index;
        return index;
    }

}
