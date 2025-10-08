using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("----------Sound SO----------")]
    [SerializeField] private SoundsSO soundSO;

    [Header("----------Audio Sources----------")]
    [SerializeField] private AudioSource musicAS;
    [SerializeField] private AudioSource SFXAS;
    [SerializeField] private AudioSource SubSFXAS;

    [Header("----------Audio Mixer----------")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("----------Sound Config----------")]
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Toggle musicIsMute;
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Toggle SFXIsMute;

    private readonly Dictionary<string, AudioClip> audioMap = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        foreach (Sound sound in soundSO.sounds)
            audioMap.TryAdd(sound.key, sound.audioClip);

        LoadSound();
    }

    private void LoadSound()
    {
        musicAS.mute = !musicIsMute.isOn;
        SFXAS.mute = !SFXIsMute.isOn;

        musicIsMute.onValueChanged.AddListener(SetMusicMute);
        SFXIsMute.onValueChanged.AddListener(SetSFXMute);

        SetMusicVolume();
        SetSFXVolume();
    }

    private void SetMusicMute(bool isOn)
    {
        musicAS.mute = !isOn;
        musicVolume.interactable = isOn;
    }

    private void SetSFXMute(bool isOn)
    {
        SFXAS.mute = !isOn;
        SFXVolume.interactable = isOn;
    }

    public void SetMusicVolume()
    {
        float volume = musicVolume.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXVolume.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }

    public void PlayAudioClip(string keyAC)
    {
        if (audioMap.TryGetValue(keyAC, out AudioClip ac))
        {
            if (!SFXAS.isPlaying)
            {
                SFXAS.clip = ac;
                SFXAS.Play();
            }
            else
            {
                SubSFXAS.clip = ac;
                SubSFXAS.Play();
            }
        }
    }

    public void StopSound()
    {
        SFXAS.Stop();
        SubSFXAS.Stop();
    }
}
