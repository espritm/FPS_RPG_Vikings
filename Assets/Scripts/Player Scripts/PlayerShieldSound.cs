using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldSound : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField]
    private AudioClip[] shieldBlockSounds;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBlockedSound()
    {
        audioSource.clip = shieldBlockSounds[Random.Range(0, shieldBlockSounds.Length)];
        audioSource.Play();
    }
}