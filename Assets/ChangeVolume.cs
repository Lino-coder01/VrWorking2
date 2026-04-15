using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Scrollbar volumeScrollbar;

    void Start()
    {
        // Volume change is saved
        float saved = PlayerPrefs.GetFloat("volume", 1f);

        volumeScrollbar.value = saved;

        // Appliquer volume
        SetVolume(saved);

        //Listener when we move the scrollback
        volumeScrollbar.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }
}