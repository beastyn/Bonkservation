namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        void Start()
        {
            if(!ManagersSOHolder.AudioManagerSO.TryLoadAudioData()) ManagersSOHolder.AudioManagerSO.SetDefaultAudioData();

        }
    }
}