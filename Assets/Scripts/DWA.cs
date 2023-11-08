using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DWA : MonoBehaviour
{
    [Header("Скорость маркера")]
    public float speed = 1f;
    [Header("Торможение колес маркера")]
    public float speedBreakTorque = 1f;
    [Header("Угол поворота колес")]
    public float wheelRotationAngle = 30f;
    [Header("Скорость поворота колес до угла поворота")]
    public float speedRotationAngle = 0.2f;
    [Header("Координаты центра масс:")]
    public Vector3 myCenterMass;

    [SerializeField]
    private Transform[] points;


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
    private float _move = 0f;
    private float _turn = 0f;


    private int i = 0;




    /*
     *z
     *x
     */
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 dist = points[i].position - transform.position;
        if (Mathf.Abs(dist.x) > 0.2f)
        {
            transform.LookAt(points[i].position);
            _rb.velocity = (points[i].position - transform.position).normalized * speed * Time.deltaTime;

        }
        else
        {
            if (i < points.Length - 1)
            {
                i++;
            }
            else
            {
                _rb.velocity = Vector3.zero;
            }
            
        }


    }
}
