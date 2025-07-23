using UnityEngine;

namespace Utils
{
    public static class PrefsSaves
    {
        public static void SaveMasterSettings(float master) => PlayerPrefs.SetFloat("Volume_Master", master);
        public static void SaveMusicSettings(float music) => PlayerPrefs.SetFloat("Volume_Music", music);
        public static void SaveSFXSettings(float sfx) => PlayerPrefs.SetFloat("Volume_SFX", sfx);

        public static void SaveAudioSettings(float master, float music, float sfx)
        {
            PlayerPrefs.SetFloat("Volume_Master", master);
            PlayerPrefs.SetFloat("Volume_Music", music);
            PlayerPrefs.SetFloat("Volume_SFX", sfx);
            PlayerPrefs.Save();
        }

        public static void LoadAudioSettings(out float master, out float music, out float sfx)
        {
            master = PlayerPrefs.GetFloat("Volume_Master", 1f); // default 1 (100%)
            music = PlayerPrefs.GetFloat("Volume_Music", 1f);
            sfx = PlayerPrefs.GetFloat("Volume_SFX", 1f);
        }
    }
}
