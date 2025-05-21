using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpenDoors : MonoBehaviour
{
    public bool isAlarmDoor;
    public bool isOpen = false;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector3 slideDirection = Vector3.forward;
    [SerializeField] private float slideAmount = 7.0f;
    

    private Vector3 Forward;
    private Vector3 startPosition;
    private Coroutine DoorAnimation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Open(Vector3 userPosition)
    {
            if (!isOpen)
            {
                if (DoorAnimation != null)
                {
                    StopCoroutine(DoorAnimation);
                }
            Debug.Log("opening door");
            DoorAnimation = StartCoroutine(SlidingDoorOpen());
            }
    }
    private IEnumerator SlidingDoorOpen()
    {
        Vector3 endPosition = startPosition + slideAmount * slideDirection;
        Vector3 startPos = transform.position;

        float time = 0;
        
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
            isOpen = true;
        }
        
        transform.position = endPosition;
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Open(other.transform.position);
        }
        if (isAlarmDoor && !isOpen)
        {
            StartCoroutine(PlayAlarm());
        }
    }

    private IEnumerator PlayAlarm()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.instance.PlaySFX("danger", 0.7f);       
    }
}
