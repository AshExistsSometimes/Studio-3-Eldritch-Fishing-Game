using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;

public class CheatsManager : MonoBehaviour
{
    // konami coode (Sequence)
    public KeyCode[] KonamiCode = new KeyCode[]
    {
      KeyCode.UpArrow,
      KeyCode.UpArrow,
      KeyCode.DownArrow,
      KeyCode.DownArrow,
      KeyCode.LeftArrow,
      KeyCode.RightArrow,
      KeyCode.LeftArrow,
      KeyCode.RightArrow,
      KeyCode.B,
      KeyCode.A,
    };

    public GameObject CheatsMenuUI;

    private int index = 0;
    private float timer = 0f;

    private bool CodeStarted = false;

    private bool CheatMenuOpen = false;

    private PauseManager pauseLogic;

    private void Start()
    {
        CheatsMenuUI.SetActive(false);
        CheatMenuOpen = false;

        pauseLogic = FindFirstObjectByType<PauseManager>();
    }

    // Konami code (Input)
    private void Update()
    {
        if (!CodeStarted  && Input.GetKeyDown(KonamiCode[index]))
        {
            index = 1;
            timer = 3f;
            StartCoroutine(KonamiCodeInput());
        }

        if (CheatMenuOpen && Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        {
            CloseCheatMenu();
        }
    }

    public IEnumerator KonamiCodeInput()
    {
        CodeStarted = true;
        yield return null;

        while (CodeStarted)
        {
                if (Input.GetKeyDown(KonamiCode[index]))
                {
                    index++;

                    if (index == KonamiCode.Length - 1)
                    {
                        OpenCheatMenu();
                        timer = 0f;
                        index = 0;
                        CodeStarted = false;
                    }
                    else
                    {
                        timer = 5f;
                    }

                    timer -= Time.deltaTime;

                }

                if (timer < 0f)
                {
                    timer = 0f;
                    index = 0;
                    CodeStarted = false;
                }


            yield return null;
            
        }
    }

    // Cheat Menu

    public void OpenCheatMenu()
    {
        CheatMenuOpen = true;
        CheatsMenuUI.SetActive(true);
        pauseLogic.OnPause();
    }


    public void CloseCheatMenu()
    {
        CheatMenuOpen = false;
        CheatsMenuUI.SetActive(false);
        pauseLogic.OnResume();
    }

    // Cheat Functions

    // Anything that isnt just making gameobjects active can go here
}
