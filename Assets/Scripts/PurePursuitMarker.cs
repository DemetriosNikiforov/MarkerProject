using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurePursuitMarker : MonoBehaviour
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


    public List<Transform> pointsPath;


    Transform minDist;

    [SerializeField]
    private float k = 0.1f; // Коэффициент дальности обзора вперед
    [SerializeField]
    private float Lfc = 2.0f; // Расстояние переднего обзора
    [SerializeField]
    private float Kp = 1f; // Коэффициент регулятора скорости P
    [SerializeField]
    private float dt = 0.1f; // Временной интервал, единица: с
    [SerializeField]
    private float L = 2.9f; // Колесная база автомобиля, ед .: 

    [SerializeField]
    private float vSpeed = 0f;
    [SerializeField]
    private float vTurn = 0;//Convert.ToSingle(40.0 * Mathf.PI / 180.0f)

    [SerializeField]
    private float target_speed = 0.0001f / 3.6f;

    private float radians = (Mathf.PI / 180);

    [SerializeField]
    private int target_index = 0;

    private int lastindex;

    [SerializeField]
    private Transform p1;
    [SerializeField]
    private Transform p2;

    public Transform pointMarker;

    //[SerializeField]
    //private float _L;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;

        //_L = (p1.position - p2.position).sqrMagnitude;


    }

    private void Start()
    {
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        dt = Time.fixedDeltaTime;
        target_index = CalcTargetIndex(pointsPath);
        lastindex = pointsPath.Count - 1;
    }

    private void FixedUpdate()
    {
        dt = Time.fixedDeltaTime;

        if (lastindex > target_index)
        {
            float a = PContorl(target_speed, vSpeed);

            float delta = PurePursuitControl(pointsPath, ref target_index);

            UpdateParametrs(a, delta);


         




            lf.steerAngle = Mathf.Lerp(lf.steerAngle, delta * Mathf.Rad2Deg, speedRotationAngle);
            rf.steerAngle = Mathf.Lerp(rf.steerAngle, delta * Mathf.Rad2Deg, speedRotationAngle);
            rb.steerAngle = Mathf.Lerp(rb.steerAngle, (delta * Mathf.Rad2Deg) * -1, speedRotationAngle);
            lb.steerAngle = Mathf.Lerp(lb.steerAngle, (delta * Mathf.Rad2Deg) * -1, speedRotationAngle);


            if (vSpeed != 0)
            {

                //сброс замедления
                lm.brakeTorque = rm.brakeTorque = 0;
                rf.brakeTorque = rb.brakeTorque = 0;
                lb.brakeTorque = lf.brakeTorque = 0;

                Debug.Log((vSpeed / Mathf.Abs(vTurn)) * dt);

                //я точно не знаю как работаю такие машины. Поэтому сделал движение по той формуле что нашел в интернете для 6 колесных машин
                //lm.motorTorque = rm.motorTorque = (vSpeed / vTurn) * dt;//Mathf.Abs()
                lm.motorTorque = rm.motorTorque = (vSpeed / vTurn) * dt;//Mathf.Abs()
                //rb.motorTorque = lb.motorTorque = (vSpeed / Mathf.Abs(vTurn)) * dt;
                //lf.motorTorque = rf.motorTorque = (vSpeed / Mathf.Abs(vTurn)) * dt;
                //lf.motorTorque = lb.motorTorque = (vSpeed / Mathf.Abs(vTurn)) * dt;
                //rb.motorTorque = rf.motorTorque = (vSpeed / Mathf.Abs(vTurn)) * dt;

            }
            //если кнопки передвижения не нажаты то замедляем маркер
            else
            {
                lb.brakeTorque = lf.brakeTorque = speedBreakTorque;
                rf.brakeTorque = rb.brakeTorque = speedBreakTorque;
                lm.brakeTorque = rm.brakeTorque = speedBreakTorque;
            }

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


        }
    }

    private int CalcTargetIndex(List<Transform> pointsPath)
    {
        //
        int index = 0;

        Vector3[] distances = new Vector3[pointsPath.Count];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = pointsPath[i].position - pointMarker.position;
        }

        List<float> squareDistances = new List<float>();

        for (int i = 0; i < distances.Length; i++)
        {
            squareDistances.Add(distances[i].magnitude);

        }

        index = squareDistances.IndexOf(squareDistances.Min());

        //float J = 0;
        L = 0;

        float Lf = k * vSpeed + Lfc;

        while (Lf > L && (index + 1) < pointsPath.Count)
        {
            Vector3 distancePoint = pointsPath[index + 1].position - pointsPath[index].position;
            //J += Mathf.Sqrt(distancePoint.sqrMagnitude);
            L += Mathf.Sqrt(distancePoint.sqrMagnitude);


            index++;
        }



        return index;
    }


    private void UpdateParametrs(float a, float delta)
    {

        Vector3 dP = new Vector3(vSpeed * Mathf.Cos(vTurn) * dt, 0, vSpeed * Mathf.Sin(vTurn) * dt);
        //transform.position += dP;

        vTurn += vSpeed / L * Mathf.Sin(delta) * dt;
        ///vTurn += vSpeed / _L * Mathf.Sin(delta) * dt;

        vSpeed += a * dt;
    }

    private float PContorl(float target, float current)
    {
        return (target - current) * Kp;
    }


    private float PurePursuitControl(List<Transform> pointsPath, ref int pindex)
    {
        int index = CalcTargetIndex(pointsPath);

        if (pindex >= index)
        {
            index = pindex;
        }

        Vector3 point = new Vector3();

        if (index < pointsPath.Count)
        {
            point = pointsPath[index].position;
        }
        else
        {
            index = pointsPath.Count - 1;
            point = pointsPath[index].position;

        }

        Vector3 r = point - pointMarker.position;
        float alpha = Mathf.Atan2(r.z, r.x) - vTurn;

        if (vSpeed < 0)
        {
            alpha = Mathf.PI - alpha;
        }

        float Lf = k * vSpeed + Lfc;

        float delta = ClampAngle(Mathf.Atan2(2 * L * Mathf.Sin(alpha) / Lf, 1) * Mathf.Rad2Deg, -wheelRotationAngle, wheelRotationAngle) * Mathf.Deg2Rad;

        pindex = index;

        return delta;

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

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, L);
    }*/
}
