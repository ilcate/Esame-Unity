using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;

    public void ChangeBgMusic(AudioClip music)
    {
        if (audioSource.clip.name == music.name)
            return;
        audioSource.Stop();
        audioSource.clip = music;
        audioSource.Play();
    }


    public void ResumeMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
   
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }


}
