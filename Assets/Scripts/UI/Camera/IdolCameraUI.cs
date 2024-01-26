namespace UI.Cameras
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IdolCameraUI : MonoBehaviour
    {
        [SerializeField] int camNum;

        public int GetCameraNum() => this.camNum;


    }
}
