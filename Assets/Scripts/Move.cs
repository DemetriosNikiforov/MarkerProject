using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("—корость маркера")]
    public float speed = 1f;
    [Header("“орможение колес маркера")]
    public float speedBreakTorque = 1f;
    [Header("”гол поворота колес")]
    public float wheelRotationAngle = 30f;
    [Header("—корость поворота колес до угла поворота")]
    public float speedRotationAngle = 0.2f;
    [Header(" оординаты центра масс:")]
    public Vector3 myCenterMass;
    

    [Space]
    [Header(" оллайдеры колес")]

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
    private float _move = 0f;
    private float _turn = 0f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;
    }

    private void FixedUpdate()
    {

        _move = Input.GetAxisRaw("Vertical");

        _turn = Input.GetAxisRaw("Horizontal");

        //поворот рулевых колес с определенной скоростью
        rf.steerAngle = lf.steerAngle =Mathf.Lerp(rf.steerAngle, _turn * wheelRotationAngle,speedRotationAngle);
        rb.steerAngle = lb.steerAngle = Mathf.Lerp(rb.steerAngle, _turn * wheelRotationAngle * -1, speedRotationAngle); 


        if (_move != 0)
        {
            //сброс замедлени€
            lm.brakeTorque = rm.brakeTorque = 0;
            rf.brakeTorque = rb.brakeTorque = 0;
            lb.brakeTorque = lf.brakeTorque = 0;

            //€ точно не знаю как работаю такие машины. ѕоэтому сделал движение по той формуле что нашел в интернете дл€ 6 колесных машин
            lm.motorTorque = rm.motorTorque = _move * speed;
            lf.motorTorque = lb.motorTorque = _move * speed;
            rb.motorTorque = rf.motorTorque = _move * speed;

        }
        //если кнопки передвижени€ не нажаты то замедл€ем маркер
        else
        {
            lb.brakeTorque = lf.brakeTorque = speedBreakTorque;
            rf.brakeTorque = rb.brakeTorque = speedBreakTorque;
            lm.brakeTorque = rm.brakeTorque = speedBreakTorque;
        }

        Vector3 postionWheels;

        // ручение GameObject колес и их поворот
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
    }
}
