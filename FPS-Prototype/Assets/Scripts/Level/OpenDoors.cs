using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;


public class OpenDoors : MonoBehaviour
{

    public bool isLocked;
    public bool isOpen = false;
    public bool keepDoorOpen;
    [SerializeField] private bool activateEvent;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector3 slideDirection = Vector3.forward;
    [SerializeField] private float slideAmount = 7.0f;
    [SerializeField] string sfxName; 

    private Vector3 startPosition;
    private Coroutine DoorAnimation;

    public UnityEvent onClosing;
    
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
            SoundManager.instance.PlaySFX(sfxName);
            Debug.Log("entering Door Trigger");
            Open(other.transform.position);
        }
        if (isLocked)
        {
            SoundManager.instance.PlaySFX("Door Lock");
        }

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
        if (activateEvent)
        {
            onClosing?.Invoke();
            yield return new WaitForFixedUpdate();
            onClosing?.RemoveAllListeners();
        }
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
