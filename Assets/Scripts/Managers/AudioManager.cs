using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("---Audio Source---")]
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioSource Music;

    [Header("---Audio Clip---")]
    public AudioClip Fight;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    public void PlaySound(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }
}
