using System;
using UnityEngine;

namespace Agent
{
    public enum AudioCollectionName
    {
        GroundImpactSounds,
        JobMischieves,
        RumbleMischieves,
        Theme,

    }

    [Serializable]
    public struct AudioCollection
    {
        public AudioCollectionName CollectionName;
        public AudioClip[] AudioClips;
    }

    [CreateAssetMenu(fileName = "Idol Audio Collection", menuName = "Bonkservation/Idols/Audio Collection")]
    public class SOAudioCollection : ScriptableObject
    {
        public AudioCollection[] idolAudioCollections;

        public AudioCollection? GetAudioCollectionByName(AudioCollectionName collectionName)
        {
            AudioCollection? foundAudioCollection = null;

            foreach (var audioCollection in this.idolAudioCollections)
            {
                if (audioCollection.CollectionName == collectionName)
                    foundAudioCollection = audioCollection;
            }
            return foundAudioCollection;
        }

        public AudioClip GetRandomClipFromCollection(AudioCollectionName collectionName)
        {
            var foundCollection = this.GetAudioCollectionByName(collectionName);

            if (foundCollection == null) return null;
            if (foundCollection.Value.AudioClips.Length <= 0) return null; 

            return foundCollection.Value.AudioClips[UnityEngine.Random.Range(0, foundCollection.Value.AudioClips.Length)];
        }
        
    }
}
