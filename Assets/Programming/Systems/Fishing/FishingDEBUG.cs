using UnityEngine;

public class FishingDEBUG : MonoBehaviour
{
    public StressBar StressBarManager;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1) && !StressBarManager.gameActive)// On Right Click
        {
            StressBarManager.StartFishing();
        }
    }
}
