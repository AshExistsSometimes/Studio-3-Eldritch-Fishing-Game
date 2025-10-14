using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    public Transform RespawnPos;

    private void Update()
    {
        if(this.transform.position.y < 0)
        {
            GetComponent<CharacterController>().enabled = false;
            this.transform.position = RespawnPos.position;
            GetComponent<CharacterController>().enabled = true;
        }
    }
}
