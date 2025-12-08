using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    public Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
public float pulseSpeed = 2f;
public bool pulseOnCall = false;

private Vector3 originalScale;
private bool pulsing = false;
private float pulseTime;

void Awake()
{
    originalScale = transform.localScale;
}

void Update()
{
    if (pulseOnCall && !pulsing) return;

    pulseTime += Time.deltaTime * pulseSpeed;
    float scaleFactor = (Mathf.Sin(pulseTime) + 1f) / 2f; // 0 to 1

    transform.localScale = Vector3.Lerp(originalScale, maxScale, scaleFactor);
}

public void TriggerPulse()
{
    pulsing = true;
    pulseTime = 0f;
}

public void StopPulse()
{
    pulsing = false;
    transform.localScale = originalScale;
}
}
