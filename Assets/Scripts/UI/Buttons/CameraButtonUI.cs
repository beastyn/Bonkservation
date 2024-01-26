namespace UI.Buttons
{
    using Managers;
    using UnityEngine;

    public class CameraButtonUI : MonoBehaviour
    {
        [SerializeField] int cameraNum;

        public void OnButtonClick() => ManagersSOHolder.CameraEvent.CameraNumRequestEvent?.Invoke(cameraNum);
    }
}