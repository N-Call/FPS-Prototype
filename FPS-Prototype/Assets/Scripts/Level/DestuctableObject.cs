using UnityEngine;

public class DestuctableObject : MonoBehaviour, IDamage
{
    [Header("Refereances")]
    [SerializeField] GameObject model;
    [SerializeField] GameObject[] parts;

    [Header("Settings")]
    [SerializeField] int health;
    [SerializeField] float regeanRate;

    private DestructableParent destructableParent;

    public bool isStoped;

    private int currentHealth;

    private float regeanTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destructableParent = GetComponentInParent<DestructableParent>();

        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {

        if (!model.activeSelf && !isStoped)
        {
            regeanTimer += Time.deltaTime;
            if (regeanTimer >= regeanRate)
            {
                ResetObject();
            }
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    public bool CheckModelActivity()
    {
        return model.activeSelf;
    }
    private void ResetObject()
    {
        currentHealth = health;
        model.SetActive(true);
        GetComponent<Collider>().enabled = true;
        regeanTimer = 0;
    }

    private void Death()
    {
        model.SetActive(false);
        GetComponent<Collider>().enabled = false;
        destructableParent.CheckDestructables();
    }
}
