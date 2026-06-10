using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    private static string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    
    public static MusicManager instance { get; private set; }

    private AudioSource audioSource;
    private float volume = .3f;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        audioSource.volume = volume;
    }

    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }

}
