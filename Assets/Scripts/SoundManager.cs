/**
 * Author: Shernan Javier
 * 
 * I wanted to replicate Dying Light 2's dynamic music system when parkouring,
 * I commented it out in the meantime because I wanted to figure it out once I get better
 * with game development
 * 
 * Example: https://www.youtube.com/watch?v=rnjRoNlEsuU&ab_channel=Raygunner
 */
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    //private float timer = 1;

    [SerializeField] private AudioSource _ambientSource, _effectsSource, _bassSource, _drumsSource, _pianoSource, _otherSource, _otherAltSource, _fallingSource;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        _effectsSource.PlayOneShot(clip);
    }

    /**
     * I Couldn't get this dynamic music code to work lol
     */
    public void PlayFalling(AudioClip clip)
    {
        //_fallingSource.volume = 1f;
        //_fallingSource.PlayOneShot(clip);
    }
    public void StopFalling(AudioClip clip)
    {
        //timer = 1;
        //timer -= Time.deltaTime;
        //print("Time in SM: " + timer);
        //_fallingSource.volume = timer;
        //if(timer < -1)
        //{
        //    _fallingSource.Stop();
        //}
    }

    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ChangeMusicOnJump(float value)
    {
        if (value < -10)
        {
            //_bassSource.volume = 1f + ((value / 100) * 3f);
            //_drumsSource.volume = 1f + ((value / 100) * 3f);
            _otherSource.volume = 1f + ((value / 100) * 3f);
            print(1f + ((value / 100) * 3f));
        } else
        {
            _bassSource.volume = 1f;
            _drumsSource.volume = 1f;
        }
    }
    public void ChangeMusicUnderCeiling(bool shouldChange)
    {
        if (shouldChange)
        {
            _bassSource.volume = 0.8f;
            _drumsSource.volume = 0.8f;
            _otherSource.volume = 0f;
            _otherAltSource.volume = 0.5f;
        }
        else
        {
            _bassSource.volume = 1f;
            _drumsSource.volume = 1f;
            _otherSource.volume = 1f;
            _otherAltSource.volume = 0f;
        }
    }
}
