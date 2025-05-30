using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


public class OpenDoors : MonoBehaviour
{

    public bool isAlarmDoor;
    public bool isOpen = false;
    public bool keepDoorOpen;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector3 slideDirection = Vector3.forward;
    [SerializeField] private float slideAmount = 7.0f;

    private Vector3 startPosition;
    private Coroutine DoorAnimation;
  
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        startPosition = transform.position;
    }

    public void Open(Vector3 userPosition)
    {
        // if door is not open start coroutine to slide the door open
        if (!isOpen)
        {
            if (DoorAnimation != null)
            {
                Debug.Log("opening door");
                StopCoroutine(DoorAnimation);
                
            }
            DoorAnimation = StartCoroutine(SlidingDoorOpen());
        }
    }


    public void Close(Vector3 userPosition)
    {
        // if door is open start coroutine to slide the door close
        if (isOpen && !keepDoorOpen)
        {
            if (DoorAnimation != null)
            {
                StopCoroutine(DoorAnimation);
                
            }
            DoorAnimation = StartCoroutine(SlidingDoorClose());
        }    
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("entering Door Trigger");
            Open(other.transform.position);
        }
        //if (isAlarmDoor && !isOpen)
        //{
        //    StartCoroutine(PlayAlarm());
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Close(other.transform.position);
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
        }
        transform.position = endPosition;
        isOpen = true;

    }
    private IEnumerator SlidingDoorClose()
    {
        Vector3 endPosition = startPosition;
        Vector3 startPos = transform.position;

        float time = 0;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
        transform.position = endPosition;
        isOpen = false;

    }


    //private IEnumerator PlayAlarm()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    SoundManager.instance.PlaySFX("danger", 0.7f);
    //    if(GameManager.instance.isPaused)
    //    {
    //        SoundManager.instance.sfxSource.Pause();
    //    }
    //}
}
