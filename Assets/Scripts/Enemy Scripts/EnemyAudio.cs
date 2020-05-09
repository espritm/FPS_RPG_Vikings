using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    private AudioSource audioSource, secondaryAudioSource;

    [SerializeField]
    private AudioClip[] attackClips, screamClip, dieClip, injuredClip;
    
    
    void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 1)
            audioSource = sources[0];
        if (sources.Length >= 2)
            secondaryAudioSource = sources[1];
    }

    public void PlayScreamSound()
    {    
        audioSource.clip = screamClip[Random.Range(0, screamClip.Length)];
        audioSource.Play();
    }

    public void PlayAttackSound()
    {
        audioSource.clip = attackClips[Random.Range(0, attackClips.Length)];
        audioSource.Play();
    }

    public void PlayDeadSound()
    {
        audioSource.clip = dieClip[Random.Range(0, dieClip.Length)];
        audioSource.Play();
    }


    public void PlayInjuredSound()
    {
        if (secondaryAudioSource.isPlaying)
            return;

        secondaryAudioSource.clip = injuredClip[Random.Range(0, injuredClip.Length)];
        secondaryAudioSource.Play();
    }
}
