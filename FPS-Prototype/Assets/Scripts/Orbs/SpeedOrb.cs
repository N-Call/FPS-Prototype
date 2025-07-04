using UnityEngine;

public class SpeedOrb : BaseOrb
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        curAbility = EAbility.speedBoost;
        if(GameManager.instance.playerAbilities != null)
        {
            duration += GameManager.instance.playerAbilities.o1Dur;
            modifier += GameManager.instance.playerAbilities.o1Srt;
            major = GameManager.instance.playerAbilities.o1Major;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
