using UnityEngine;

public class GlobalAmmo : MonoBehaviour
{
    public int gun1 = 10;
    [SerializeField] GameObject AmmoCount;

    // Update is called once per frame
    void Update()
    {
        AmmoCount.GetComponent<TMPro.TMP_Text>().text = "" + gun1; 
    }
}
