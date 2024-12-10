using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        this.masterSlider.value = ManagersSOHolder.AudioManagerSO.MasterVolume;
        this.musicSlider.value = ManagersSOHolder.AudioManagerSO.MusicVolume;
        this.sfxSlider.value = ManagersSOHolder.AudioManagerSO.SFXVolume;
    }

    public void SetMasterVolume(float level)      => ManagersSOHolder.AudioManagerSO?.SetMasterVolume(level);
    public void SaveMasterVolume()     => ManagersSOHolder.AudioManagerSO?.SaveMasterVolume(this.masterSlider.value);

    public void SetMusicVolume(float level) => ManagersSOHolder.AudioManagerSO?.SetMusicVolume(level);
    public void SaveMusicVolume() => ManagersSOHolder.AudioManagerSO?.SaveMusicVolume(this.musicSlider.value);

    public void SetSoundEffectVolume(float level) => ManagersSOHolder.AudioManagerSO?.SetSFXVolume(level);
    public void SaveSoundEffectVolume() => ManagersSOHolder.AudioManagerSO?.SaveSFXVolume(this.sfxSlider.value);
}
