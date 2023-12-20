using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 100f;
    [SerializeField]
    private Transform character;

    [SerializeField]
    Vector3 positionOffset;

    [SerializeField]
    private float minX = -90f;
    [SerializeField]
    private float maxX = 90f;

    [SerializeField]
    private float minY = -90f;
    [SerializeField]
    private float maxY = 90f;

    private float _mouseX;
    private float _mouseY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraRotate();

        transform.position = character.position + positionOffset;
    }

    private float ClampAngle(float value, float min, float max)
    {
        if (value > 180)
        {
            value -= 360;
        }

        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }


    private void CameraRotate()
    {
        _mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        _mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        transform.localEulerAngles += Vector3.left * _mouseY + Vector3.up * _mouseX;

        transform.localEulerAngles = new Vector3(ClampAngle(transform.localEulerAngles.x, minX, maxX), ClampAngle(transform.localEulerAngles.y, minY, maxY), transform.localEulerAngles.z);
    }
}
