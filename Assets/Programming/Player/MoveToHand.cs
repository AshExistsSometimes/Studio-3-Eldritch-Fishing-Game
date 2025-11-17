using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveToHand : MonoBehaviour
{
    public Transform PlayerSocket;
    public Transform Item;
    public float LerpStrength = 0.1f;
    public float LerpRotStrength = 0.1f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(Item.position, PlayerSocket.position, LerpStrength);
        transform.rotation = Quaternion.Lerp(Item.rotation, PlayerSocket.rotation, LerpRotStrength);
    }
}
