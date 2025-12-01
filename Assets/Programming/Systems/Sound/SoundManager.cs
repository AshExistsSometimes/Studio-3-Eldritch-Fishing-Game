using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Sounds
{
    // Add an enum here, and it will appear in the list this script creates with a custom name.
    // Then, just add the sound to the list element
    // Each element of the list has a list, and a random clip is picked from that embedded list when a sound is played
    // This can be used if you want a random ambience sound to be played if you have many ambience sounds for example

    UI_TICK
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    [HideInInspector] public static SoundManager instance;
    [HideInInspector] public AudioSource soundSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(Sounds sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.soundSource.PlayOneShot(randomClip);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] soundNames = Enum.GetNames(typeof(Sounds));

        Array.Resize(ref soundList, soundNames.Length);

        for (int i = 0; i < soundNames.Length; i++)
        {
            soundList[i].name = soundNames[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
