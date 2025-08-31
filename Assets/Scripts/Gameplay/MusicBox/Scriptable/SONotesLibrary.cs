using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.MusicBox
{

    // =============================
    // 2) AUDIO LOOKUP (Note bank)
    // =============================
    [CreateAssetMenu(menuName = "Bonkservation/MusicBox/NotesLibrary", fileName = "NotesLibrary")]
    public class SONotesLibrary : ScriptableObject
    {

        [Serializable] public class Entry { public string Key; public AudioClip Clip; }
        public Entry[] entries;
        Dictionary<string, AudioClip> nameToClip;


        public void Build()
        {
            this.nameToClip = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);
            if (this.entries == null) return;
            foreach (var entry in entries)
                if (entry != null && !string.IsNullOrEmpty(entry.Key) && entry.Clip != null) this.nameToClip[entry.Key] = entry.Clip;
        }


        public AudioClip Get(string key)
        {
            if (this.nameToClip == null) Build();
            if (key == null) return null;
            return this.nameToClip != null && this.nameToClip.TryGetValue(key, out var clip) ? clip : null;
        }
    }
}
