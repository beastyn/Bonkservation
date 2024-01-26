using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPositionRandomizer : MonoBehaviour
{
    [SerializeField] Transform originalPosition;
    [SerializeField] Animation animationForPositionp;

    void OnEnable()
    {
        this.transform.position = this.originalPosition.position;
        float spawnY = Random.Range
              (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        Vector3 finalPosition = new Vector3(spawnX, spawnY, 0);

        DOTween.Kill(this);
        DOTween.To(() => this.transform.position, x => this.transform.position = x, finalPosition, this.animationForPositionp.clip.length);

    }



}
