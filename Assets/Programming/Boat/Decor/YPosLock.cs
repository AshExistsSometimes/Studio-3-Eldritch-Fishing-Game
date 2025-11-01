using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YPosLock : MonoBehaviour
{
    public float HeightToLockAt = 1.7f;
    public float ReturnStrength = 1f;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, HeightToLockAt, transform.position.z), ReturnStrength);
    }
}
