using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerController : MonoBehaviour
{


    [Header("Скорость поворота колес до угла поворота")]
    public float speedRotationAngle = 0.2f;
    [Header("Координаты центра масс:")]
    public Vector3 myCenterMass;


    [Space]
    [Header("Коллайдеры колес")]

    [SerializeField]
    private WheelCollider lf;
    [SerializeField]
    private WheelCollider rf;

    [SerializeField]
    private WheelCollider lm;
    [SerializeField]
    private WheelCollider rm;

    [SerializeField]
    private WheelCollider lb;
    [SerializeField]
    private WheelCollider rb;

    [Space]
    [Header("GameObject колес")]

    [SerializeField]
    private GameObject lbWheel;
    [SerializeField]
    private GameObject rbWheel;

    [SerializeField]
    private GameObject lfWheel;
    [SerializeField]
    private GameObject rfWheel;

    [SerializeField]
    private GameObject lmWheel;
    [SerializeField]
    private GameObject rmWheel;

    private Rigidbody _rb;

    public float speed = 25f;
    public float turn = 0f;
    public float angle = 0f;

    // Start is called before the first frame update
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;

    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Debug.Log(turn * Mathf.Rad2Deg);

        //lf.steerAngle = Mathf.Lerp(lf.steerAngle, angle * Mathf.Rad2Deg, speedRotationAngle);
        //rf.steerAngle = Mathf.Lerp(rf.steerAngle, angle * Mathf.Rad2Deg, speedRotationAngle);
        //rb.steerAngle = Mathf.Lerp(rb.steerAngle, angle * Mathf.Rad2Deg * -1, speedRotationAngle);
        //lb.steerAngle = Mathf.Lerp(lb.steerAngle, angle * Mathf.Rad2Deg * -1, speedRotationAngle);


        lf.steerAngle = angle * Mathf.Rad2Deg;
        rf.steerAngle = angle * Mathf.Rad2Deg;
        rb.steerAngle = angle * Mathf.Rad2Deg * -1;
        lb.steerAngle = angle * Mathf.Rad2Deg * -1;

        lm.motorTorque = rm.motorTorque = speed;
        rb.motorTorque = lb.motorTorque = speed;
        lf.motorTorque = rf.motorTorque = speed;

        //Debug.Log(lm.motorTorque);

        Vector3 postionWheels;

        //Кручение GameObject колес и их поворот
        Quaternion backWheelsrRotation;
        lb.GetWorldPose(out postionWheels, out backWheelsrRotation);
        lbWheel.transform.rotation = backWheelsrRotation;
        rbWheel.transform.rotation = backWheelsrRotation;


        Quaternion frontWheelsRotation;
        lf.GetWorldPose(out postionWheels, out frontWheelsRotation);
        lfWheel.transform.rotation = frontWheelsRotation;
        rfWheel.transform.rotation = frontWheelsRotation;

        Quaternion middleWheelsRotation;
        lm.GetWorldPose(out postionWheels, out middleWheelsRotation);
        rmWheel.transform.rotation = middleWheelsRotation;
        lmWheel.transform.rotation = middleWheelsRotation;
        lmWheel.transform.rotation = middleWheelsRotation;
    }
}
