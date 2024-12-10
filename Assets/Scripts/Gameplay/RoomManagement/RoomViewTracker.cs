using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomViewTracker : MonoBehaviour
{
    [SerializeField] int roomNum;
    [SerializeField] GameObject MainView;
    [SerializeField] GameObject BonkView;
    void OnEnable()
    {
        //ManagersSOHolder.CameraEvent.CameraTransitionStartEvent += OnCameraTransitionStartEvent;
        ManagersSOHolder.CameraEvent.CameraTransitionEndEvent += OnCameraTransitionEndEvent;
    }

    void OnDisable()
    {
        //ManagersSOHolder.CameraEvent.CameraTransitionStartEvent -= OnCameraTransitionStartEvent;
        ManagersSOHolder.CameraEvent.CameraTransitionEndEvent -= OnCameraTransitionEndEvent;
    }

    void Start()
    {
        this.BonkView.SetActive(false);
        this.MainView.SetActive(true);
    }

    void OnCameraTransitionEndEvent(int roomNum)
    {
        if (roomNum == this.roomNum)
        {
            this.BonkView.SetActive(true);
            this.MainView.SetActive(false);
        }
        else
        {
            this.BonkView.SetActive(false);
            this.MainView.SetActive(true);
        }
    }

}
