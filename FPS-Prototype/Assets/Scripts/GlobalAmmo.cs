using UnityEngine;

public class GlobalAmmo : MonoBehaviour
{
    public int gun1 = 10;
    [SerializeField] GameObject ammoCount;

    // Update is called once per frame
    void Update()
    {
        ammoCount.GetComponent<TMPro.TMP_Text>().text = "" + gun1; 
    }
}
