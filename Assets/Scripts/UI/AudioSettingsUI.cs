using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using Utils;
using Managers;

namespace UI
{
   
    public class AudioSettingsUI : BaseWindowUI
    {
        [SerializeField] Slider sliderMasterVolume;
        [SerializeField] Slider sliderMusicVolume;
        [SerializeField] Slider sliderSFXVolume;

        float masterVolume;
        float musicVolume;
        float sfxVolume;

        void OnEnable()
        {
            PrefsSaves.LoadAudioSettings(out this.masterVolume, out this.musicVolume, out this.sfxVolume);
            this.sliderMasterVolume.value = this.masterVolume;
            this.sliderMusicVolume.value = this.musicVolume;
            this.sliderSFXVolume.value = this.sfxVolume;
        }
        public void SetMasterVolume()
        {
            AudioMixerManager.SetMasterVolume(this.sliderMasterVolume.value);
            PrefsSaves.SaveMasterSettings(this.sliderMasterVolume.value);
        }
        public void SetSoundEffectVolume()
        {
            AudioMixerManager.SetSoundEffectVolume(this.sliderSFXVolume.value);
            PrefsSaves.SaveSFXSettings(this.sliderSFXVolume.value);
        }
        public void SetMusicVolume()
        {
            AudioMixerManager.SetMusicVolume(this.sliderMusicVolume.value);
            PrefsSaves.SaveMusicSettings(this.sliderMusicVolume.value);
        }
    }
}
