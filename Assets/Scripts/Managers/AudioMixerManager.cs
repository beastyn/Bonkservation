using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public void SetMasterVolume(float level)      => this.audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    public void SetSoundEffectVolume(float level) => this.audioMixer.SetFloat("SoundEffectVolume", Mathf.Log10(level) * 20f);
    public void SetMusicVolume(float level)       => this.audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
}
