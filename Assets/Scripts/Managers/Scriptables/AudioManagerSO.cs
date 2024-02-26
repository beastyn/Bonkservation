namespace Managers
{
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CreateAssetMenu(fileName = "AudioManager", menuName = "Bonk/Managers/AudioManager")]
    public class AudioManagerSO : ScriptableObject
    {
        [SerializeField] AudioSource soundFXObject;

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
