using Agent;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;

        float masterVolume, musicVolume, sfxVolume;
        static AudioMixer cachedAudioMixer;
        void Start()
        {
            PrefsSaves.LoadAudioSettings(out this.masterVolume, out this.musicVolume, out this.sfxVolume);
            cachedAudioMixer = this.audioMixer;
            SetMasterVolume(this.masterVolume);
            SetMusicVolume(this.musicVolume);
            SetSoundEffectVolume(this.sfxVolume);
        }

        public static void SetMasterVolume(float level) => cachedAudioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(level, 0.0001f, 1f)) * 20f);
        public static void SetSoundEffectVolume(float level) => cachedAudioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(level, 0.0001f, 1f)) * 20f);
        public static void SetMusicVolume(float level) => cachedAudioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(level, 0.0001f, 1f)) * 20f);

        public static void PlayClip(AudioSource audioSource, AudioClip audioClip, bool loop = false)
        {
            if (audioClip == null || audioSource == null) return;
            audioSource.clip = audioClip;
            audioSource.loop= loop;
            audioSource.Play();
        }

        public static AudioClip PlayRundomCollectionClip(AudioSource audioSource, SOAudioCollection collection, AudioCollectionName collectionName, bool loop = false)
        {
            var audioClip = collection.GetRandomClipFromCollection(collectionName);
            PlayClip(audioSource, collection.GetRandomClipFromCollection(collectionName), loop);
            return audioClip;
        }
    }
}
