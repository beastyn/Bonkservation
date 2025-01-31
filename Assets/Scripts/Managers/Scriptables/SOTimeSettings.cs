using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Bonkservation/Time Settings")]
public class SOTimeSettings : ScriptableObject {
    public float timeMultiplier = 2000;
    public float startHour = 12;
    public float sunriseHour = 6;
    public float sunsetHour = 18;
}