using UnityEngine;

public class FishingBobber : MonoBehaviour
{
    public FishingMinigame minigame;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ocean"))
        {
            minigame.InitializeMinigame();
        }
    }
}
