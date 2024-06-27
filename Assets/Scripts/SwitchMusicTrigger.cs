using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicTrigger : MonoBehaviour
{
    public AudioClip newTrack;
    private MusicController musicController;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (newTrack != null)
                musicController.ChangeBgMusic(newTrack);
        }
    }

    void Start()
    {
        musicController = FindAnyObjectByType<MusicController>();
    }
}
