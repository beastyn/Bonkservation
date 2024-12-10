namespace Managers
{
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.Rendering;
    using UnityEngine.UIElements;

    [CreateAssetMenu(fileName = "AudioManager", menuName = "Bonk/Managers/AudioManager")]
    public class AudioManagerSO : ScriptableObject
    {
        [SerializeField] float masterVolume = 1f;
        [SerializeField] float musicVolume = 0.3f;
        [SerializeField] float sfxVolume = 1f;
        [SerializeField] AudioSource soundFXObject;
        [SerializeField] AudioMixer audioMixer;

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        public AudioMixer AudioMixer => audioMixer;


        public void SetMasterVolume(float masterVolume) => this.audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20f);
        public void SetMusicVolume(float musicVolume) => this.audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
        public void SetSFXVolume(float sfxVolume) => this.audioMixer.SetFloat("SoundEffectVolume", Mathf.Log10(sfxVolume) * 20f);

        public void SaveMasterVolume(float volume)
        {
            this.masterVolume = volume;
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
        public void SaveMusicVolume(float volume)
        {
            this.musicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);

        }
        public void SaveSFXVolume(float volume)
        {
            this.sfxVolume = volume;
            PlayerPrefs.SetFloat("SoundEffectVolume", volume);
        }

        public bool TryLoadAudioData()
        {
            var master = PlayerPrefs.GetFloat("MasterVolume", -1);
            var music = PlayerPrefs.GetFloat("MusicVolume", -1);
            var sfx = PlayerPrefs.GetFloat("SoundEffectVolume",-1);
            if (master == -1) return false;

            this.SetMasterVolume(master);
            this.SetMusicVolume(music);
            this.SetSFXVolume(sfx);

            this.masterVolume = master;
            this.musicVolume = music;
            this.sfxVolume = sfx;

            return true;
        }

        public void SetDefaultAudioData()
        {
            this.SetMasterVolume(this.masterVolume);
            this.SetMusicVolume(this.musicVolume);
            this.SetSFXVolume(this.sfxVolume);

            this.SaveMasterVolume(this.masterVolume);
            this.SaveMusicVolume(this.musicVolume);
            this.SaveSFXVolume(this.sfxVolume);

        }

        public void PlaySFX(AudioClip audioClip, Transform parent, float volume)
        {
            var audioSource = GameObject.Instantiate(this.soundFXObject, parent.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            Timing.RunCoroutine(this.DestroySource(audioSource).CancelWith(audioSource.gameObject));
        }

        IEnumerator<float> DestroySource(AudioSource soundObject)
        {
            yield return Timing.WaitForSeconds(soundObject.clip.length);
            GameObject.Destroy(soundObject.gameObject);
        }
    }
}
