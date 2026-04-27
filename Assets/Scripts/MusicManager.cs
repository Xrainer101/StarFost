using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip levelMusic;
    public AudioClip victoryJingle;

    // Start is called before the first frame update
    void Start()
    {
        bgmSource.clip = levelMusic;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopMusic()
    {
        bgmSource.Stop();
    }

    public void PlayVictoryMusic()
    {
        StopMusic();

        bgmSource.clip = victoryJingle;
        bgmSource.loop = false;
        bgmSource.Play();
    }
}
