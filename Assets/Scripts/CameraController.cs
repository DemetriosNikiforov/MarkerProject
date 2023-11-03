using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("������� ������ �������:")]
    public GameObject marker;
    private Vector3 _distance;

    void Start()
    {
        _distance = marker.transform.position - transform.position;
    }

    void LateUpdate()
    {
        //������� ������ �� � �� ��������
        Quaternion rotationCamera = Quaternion.Euler(0, marker.transform.eulerAngles.y, 0);
        //������ ������� �� ��������
        transform.position = marker.transform.position - (rotationCamera * _distance);

        transform.LookAt(marker.transform);
    }
}
