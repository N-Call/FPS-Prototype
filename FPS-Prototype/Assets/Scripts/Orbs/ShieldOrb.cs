using UnityEngine;

public class ShieldOrb : BaseOrb
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        curAbility = EAbility.invensBoost;
        if (GameManager.instance.playerAbilities != null)
        {
            duration += GameManager.instance.playerAbilities.o3Dur;
            modifier += GameManager.instance.playerAbilities.o3Srt;
            major = GameManager.instance.playerAbilities.o3Major;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
