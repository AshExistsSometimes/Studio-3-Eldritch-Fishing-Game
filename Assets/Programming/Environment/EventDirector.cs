using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class EventDirector : MonoBehaviour
{
    private SceneManager sceneManager;
    public GameObject[] eventObjectsToTrigger;

    [Header("Variables")]
    private float weirdness;
    public int eventChance;
    public float eventDuration;
    private float eventTimer;
    public float secondsBetweenEvents = 10;

    public Transform posToSpawn;

    private void Start()
    {
        sceneManager = FindAnyObjectByType<SceneManager>();
    }

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
                StartCoroutine(TriggerEvent());
            }

            eventTimer = 0;
        }
    }

    private IEnumerator TriggerEvent()
    {
        int n = Random.Range(0, eventObjectsToTrigger.Length);

        eventObjectsToTrigger[n].SetActive(true);

        yield return new WaitForSeconds(eventDuration);

        eventObjectsToTrigger[n].SetActive(false);
    }
}