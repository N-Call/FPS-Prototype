using System.Collections;
using UnityEngine;

public class DestructableParent : MonoBehaviour
{
    [SerializeField] DestuctableObject[] destuctables;
    [SerializeField] Animator animator;
    [SerializeField] float waitTime;
    [SerializeField] float refreshTime;

    private float curRefreshTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curRefreshTime = refreshTime;
    }

    // Update is called once per frame
    void Update()
    {
        curRefreshTime += Time.deltaTime;
    }

    public void CheckDestructables()
    {
        if(curRefreshTime < refreshTime) { return; }

        int inactiveCount = 0;
        foreach (var destuctable in destuctables)
        {
            if (!destuctable.CheckModelActivity())
            {
                inactiveCount++;

            }
        }

        if (inactiveCount == destuctables.Length)
        {
            StartCoroutine(Collapes());
        }
    }

    void ActivateAnim()
    {
        animator.CrossFade("CollapsingFloor", 0.2f);
    }

    IEnumerator Collapes()
    {
        ActivateAnim();

        foreach (var destructable in destuctables)
        {
            destructable.isStoped = true;
        }
        yield return new WaitForSeconds(waitTime);
        foreach (var destructable in destuctables)
        {
            destructable.isStoped = false;
        }
        curRefreshTime = 0;
    }
}
