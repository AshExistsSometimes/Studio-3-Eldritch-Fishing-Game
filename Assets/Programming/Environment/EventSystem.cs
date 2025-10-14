using System.Collections;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public GameObject hallucination;

    //private int weirdness;
    private int eventChance;
    private float eventTimer;
    //private int pickEvent;

    public float secondsBetweenEvents = 10;

    private void Update()
    {
        eventTimer += Time.deltaTime;

        if (eventTimer >= secondsBetweenEvents)
        {
            eventChance = (Random.Range(1, 100));

            if (eventChance > 100)
            {
                eventChance = 100;
            }

            if (eventChance >= 80)
            {
                StopAllCoroutines();
                TriggerEvent();
            }

            eventTimer = 0;
        }
    }

    private void TriggerEvent()
    {
        //pickEvent = Random.Range(1, 3);

        //if (pickEvent == 1)
        //{
            StartCoroutine(TriggerHallucination());
        //}
    }

    private IEnumerator TriggerHallucination()
    {
        hallucination.SetActive(true);

        yield return new WaitForSeconds(1f);

        hallucination.SetActive(false);
    }
}
