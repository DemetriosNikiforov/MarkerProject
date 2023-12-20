using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform character;

    [SerializeField]
    private float heightCharacter = 2f;

    [SerializeField]
    private Vector3 positionOffset;

    [SerializeField]
    private Vector3 angleOffset;

    [SerializeField]
    private float speedRotationCamer = 1f;

    [SerializeField]
    private float speedRotateAxis = 1f;

    private float _mouseX;
    private float _mouseY;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

       

    }

    private void CameraLook()
    {

    }

    private void CameraMove()
    {


    }
}
