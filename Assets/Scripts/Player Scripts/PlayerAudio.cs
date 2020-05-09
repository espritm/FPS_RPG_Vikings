using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
     AudioSource audioSource;

    [SerializeField]
    private AudioClip[] injuredClip, lowStaminaClip, gameoverClip;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    private void SetVolumeNormal()
    {
        audioSource.volume = 1;
    }

    private void SetVolumeLow()
    {
        audioSource.volume = 0.65f;
    }

    public void PlayInjuredSound()
    {
        SetVolumeNormal();
        audioSource.clip = injuredClip[Random.Range(0, injuredClip.Length)];
        audioSource.Play();
    }

    public void PlayLowStaminaSound()
    {
        SetVolumeLow();
        audioSource.clip = lowStaminaClip[Random.Range(0, lowStaminaClip.Length)];
        audioSource.Play();
    }

    public void PlayGameOverSound()
    {
        SetVolumeNormal();

        //Player first audio and once it is finished playe second audio
        audioSource.clip = gameoverClip[0];
        audioSource.Play();

        StartCoroutine("PlayGameOver");
    }

    IEnumerator PlayGameOver()
    {
        yield return new WaitForSeconds(3.5f);
        audioSource.clip = gameoverClip[1];
        audioSource.Play();
    }

}
