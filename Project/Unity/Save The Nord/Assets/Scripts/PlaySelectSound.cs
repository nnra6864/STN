using UnityEngine;

public class PlaySelectSound : MonoBehaviour
{
    public void Play()
    {
        SoundManager.Instance.PlaySound("Select");
    }
}
