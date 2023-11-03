using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Игровой обьект маркера:")]
    public GameObject marker;
    private Vector3 _distance;

    void Start()
    {
        _distance = marker.transform.position - transform.position;
    }

    void LateUpdate()
    {
        //поворот камеры по у за маркером
        Quaternion rotationCamera = Quaternion.Euler(0, marker.transform.eulerAngles.y, 0);
        //камера следует за маркером
        transform.position = marker.transform.position - (rotationCamera * _distance);

        transform.LookAt(marker.transform);
    }
}
