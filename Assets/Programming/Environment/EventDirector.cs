using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventDirector : MonoBehaviour
{
    private SceneManager sceneManager;
    public List<EventObjects> eventObjects;
    int eventIndex = 0;

    [Header("Variables")]
    [SerializeField] private float randomChance;
    [SerializeField] private float eventTimer;
    [SerializeField] private float secondsBetweenEvents = 10;
    private bool isEventActive = false;

    private void Start()
    {
        sceneManager = FindAnyObjectByType<SceneManager>();
    }

    private void Update()
    {
        if (!isEventActive && sceneManager.IsNight) 
        {
            eventTimer += Time.deltaTime;
        }

        if (eventTimer >= secondsBetweenEvents)
        {
            PickEventToTrigger();
        }
    }

    private void PickEventToTrigger()
    {
        randomChance = UnityEngine.Random.Range(1, 100) + sceneManager.Weirdness;
        eventIndex = UnityEngine.Random.Range(0, eventObjects.Count);

        if (randomChance > 100)
        {
            randomChance = 100;
        }
        
        if (randomChance >= eventObjects[eventIndex].chanceToSpawn)
        {
            eventTimer = 0;
            isEventActive = true;
            StopAllCoroutines();
            StartCoroutine(TriggerEvent());
        }
    }

    private IEnumerator TriggerEvent()
    {
        eventObjects[eventIndex].eventGameObject.SetActive(true);

        yield return new WaitForSeconds(eventObjects[eventIndex].despawnTimer);

        if (eventObjects[eventIndex].usingDespawnTimer)
        {
            eventObjects[eventIndex].eventGameObject.SetActive(false);
        }

        isEventActive = false;
    }
}

[Serializable]
public class EventObjects
{
    public GameObject eventGameObject;
    public int chanceToSpawn;
    public float despawnTimer;
    public bool usingDespawnTimer;
    public Transform posToSpawn;
}