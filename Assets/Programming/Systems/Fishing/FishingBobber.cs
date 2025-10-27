using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    public FishingMinigame minigame;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ocean"))
        {
            Debug.Log("Collided with ocean");
            minigame.InitializeMinigame();
        }
    }
}
