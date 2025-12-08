using UnityEngine;

public class PlayUISound : MonoBehaviour
{
    public void PlayClick()
    {
        SoundManager.PlaySound(Sounds.UI_TICK);
    }
}
