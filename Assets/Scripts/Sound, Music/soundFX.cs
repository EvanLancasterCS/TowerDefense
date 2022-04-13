using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundFX : MonoBehaviour
{
    const int MAX_SFX = 4;
    int numAudioPlaying = 0;
    float audioBuffer = 0.3f;
    //probs shouldve just made an array
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;
    public AudioClip clip4;
    public AudioClip clip5;
    public AudioClip crash;
    public AudioClip towerHit;
    public AudioClip enemyNoise;
    public AudioClip towerPlace;
    AudioSource audiSource;

    public static soundFX inst;

    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        audiSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound(int soundID)
    {
        //if(numAudioPlaying < MAX_SFX)
        //{
            numAudioPlaying++;
            StartCoroutine(coSFX(soundID));
       // }

    }

    IEnumerator coSFX(int soundID)
    {
        // beginning implementation for SFX management 
        // so later game it doesn't sound like a jumbled mess
        

        switch (soundID)
        {
            case 1:
            case 7:
            case 12:
                audiSource.PlayOneShot(clip1);
                break;
            case 2:
            case 8:
            case 13:
                audiSource.PlayOneShot(clip2);
                break;
            case 3:
            case 9:
            case 14:
                audiSource.PlayOneShot(clip3);
                break;
            case 4:
            case 10:
            case 15:
                audiSource.PlayOneShot(clip4);
                break;
            case 5:
            case 11:
            case 16:
                audiSource.PlayOneShot(clip5);
                break;
        }

        // include a buffer between shots at some point after certain number of duplicate
        //yield return new WaitForSeconds(audiobuffer);
        yield return null;
        numAudioPlaying -= 1;
    }

    public void playCrashLanding()
    {
        audiSource.PlayOneShot(crash);
    }

    public void playerTowerTakeDamage()
    {
        audiSource.PlayOneShot(towerHit);
    }

    public void playEnemyNoise()
    {
        audiSource.PlayOneShot(enemyNoise);
    }

    public void playTowerPlacement()
    {
        audiSource.PlayOneShot(towerPlace);
    }
}
