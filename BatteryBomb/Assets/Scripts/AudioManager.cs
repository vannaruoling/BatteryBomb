using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;


    public float musicVolume = 0.6f;
    public float sfxVolume = 0.8f;

    // Music
    public AudioClip titleMusic;
    public AudioClip stageMusic;
    public AudioClip bossMusic;

    // SFX
    public AudioClip bombPickup;
    public AudioClip bombAttach;
    public AudioClip bombTick;
    public AudioClip bombExplode;
    public AudioClip chainExplode;
    public AudioClip enemyDeath;
    public AudioClip enemyLeak;
    public AudioClip turretShoot;
    public AudioClip turretPlace;
    public AudioClip cardPick;
    public AudioClip waveCleared;
    public AudioClip gameOver;
    public AudioClip victory;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }
}