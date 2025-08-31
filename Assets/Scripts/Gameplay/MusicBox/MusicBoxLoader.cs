using System;
using UnityEngine;

namespace Gameplay
{
    // =============================
    // 1) DATA — JSON schema
    // =============================
    [Serializable]
    public class MusicBoxComb
    {
        public string id; // e.g., "C4" (must be unique)
        public string display; // e.g., "C" shown near the comb tooth
        public float combLength = 1; // visual length scale for tooth (0.2..1.5)
        public string audioKey; // lookup key for AudioClip (optional)
    }


    [Serializable]
    public class MusicBoxPinsForComb
    {
        public string tooth; // row.id that these beats belong to
        public int[] beats; // 1-based beat indices where pins exist
    }


    [Serializable]
    public class MusicBoxSheet
    {
        public string title;
        public int bpm = 100;
        public int beats = 16; // columns
        public MusicBoxComb[] teeth; // comb definition (top?bottom order)


        // Target (solution) pins that define the correct melody for validation & preview
        public MusicBoxPinsForComb[] targetPins;


        // Optional: a separate preview/humming track if you want different guidance than targetPins
        public MusicBoxPinsForComb[] previewPins;
    }
    public class MusicBoxLoader : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
