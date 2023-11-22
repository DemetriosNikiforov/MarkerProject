using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPM : MonoBehaviour
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

    public float L = 1;
    public int wp = 0;
    public float vx = 0.3f;
    public float theta = 0.5f;
    public float w_limit = 0.48f;
    public float w = 0;
    public float dt = 0.1f;
    public float tdot = 0;

    public float alpha = 0;
    [SerializeField]
    private float k = 0.1f; // Коэффициент дальности обзора вперед
    [SerializeField]
    private float Lfc = 2.0f; // Расстояние переднего обзора

    public List<Transform> pointsPath;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;
    }
    // Start is called before the first frame update
    void Start()
    {
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        wp = SearchPoint(pointsPath);

    }

    // Update is called once per frame
    void Update()
    {
        //dt = Time.deltaTime;
        if (Mathf.Abs(Vector3.Distance(transform.position, pointsPath[pointsPath.Count - 1].position)) > 0.1)
        {
            float error = 0;
            w = pure_pursuit_controller(pointsPath, ref error);
            tdot = position_vector(vx, w, theta);

            theta = theta + dt * tdot;

            //theta = PurePursuitControl(pointsPath);


            lf.steerAngle = Mathf.Lerp(lf.steerAngle, theta * Mathf.Rad2Deg, speedRotationAngle);
            rf.steerAngle = Mathf.Lerp(rf.steerAngle, theta * Mathf.Rad2Deg, speedRotationAngle);
            rb.steerAngle = Mathf.Lerp(rb.steerAngle, theta * Mathf.Rad2Deg * -1, speedRotationAngle);
            lb.steerAngle = Mathf.Lerp(lb.steerAngle, theta * Mathf.Rad2Deg * -1, speedRotationAngle);

            lm.motorTorque = rm.motorTorque = vx;
            rb.motorTorque = lb.motorTorque = vx;
            lf.motorTorque = rf.motorTorque = vx;

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

    float position_vector(float vx, float w, float theta)
    {

        //transform.position = transform.position + new Vector3(vx * Mathf.Cos(theta), 0, vx * Mathf.Sin(theta));
        float tdot = w;

        if (tdot >= w_limit)
        {
            tdot = w_limit;
        }
        else if (tdot <= -w_limit)
        {
            tdot = -w_limit;
        }

        return tdot;
    }

    float pure_pursuit_controller(List<Transform> waypoints, ref float error)
    {
        Vector3 point0 = waypoints[wp].position;
        Vector3 point1 = waypoints[wp + 1].position;

        float up = ((transform.position.x - point0.x) * (point1.x - point0.x) + (transform.position.z - point0.z) * (point1.z - point0.z)) / (Mathf.Pow(point1.x - point0.x, 2) + Mathf.Pow(point1.z - point0.z, 2));

        Vector3 pos0 = new Vector3(point0.x + up * (point1.x - point0.x), point1.y, point0.z + up * (point1.z - point0.z));

        error = Mathf.Sqrt(Mathf.Pow(transform.position.x - pos0.x, 2) + Mathf.Pow(transform.position.z - pos0.z, 2));

        if (Mathf.Sqrt(Mathf.Pow(point1.x - pos0.x, 2) + Mathf.Pow(point1.z - pos0.z, 2)) <= L)

        {
            if (wp < waypoints.Count - 2)
            {
                wp++;
            }
        }

        float dO = Mathf.Sqrt(Mathf.Pow(transform.position.x - pos0.x, 2) + Mathf.Pow(transform.position.z - pos0.z, 2));
        float dl = 0;

        if (Mathf.Pow(dO, 2) >= Mathf.Pow(L, 2))
        {
            dl = 0;
        }
        else
        {
            dl = Mathf.Sqrt(Mathf.Pow(L, 2) - Mathf.Pow(dO, 2));

        }

        Vector3 wpvec = new Vector3(point1.x - point0.x, 0, point1.z - point0.z);

        Vector3 norm_vec = wpvec.normalized;

        Vector3 v1 = pos0 + dl * norm_vec;

        float zd = Mathf.Sin(-theta) * (v1.x - transform.position.x) + Mathf.Cos(-theta) * (v1.z - transform.position.z);

        float w = 2 * zd * vx / (Mathf.Pow(L, 2));
        //float w = vx / L * Mathf.Sin(-theta);

        return w;

        // ?? float norm_vec = wpvec/wpvec. ;



    }

    private int SearchPoint(List<Transform> points)
    {
        int index = 0;
        if (points.Count != 0)
        {
            float minsquareDistance = Mathf.Pow(points[0].position.x - transform.position.x, 2) + Mathf.Pow(points[0].transform.position.y - transform.position.y, 2) + Mathf.Pow(points[0].transform.position.z - transform.position.z, 2);

            foreach (Transform point in points)
            {
                float squareDistance = Mathf.Pow(point.position.x - transform.position.x, 2) + Mathf.Pow(point.transform.position.y - transform.position.y, 2) + Mathf.Pow(point.transform.position.z - transform.position.z, 2);
                if (minsquareDistance > squareDistance)
                {
                    minsquareDistance = squareDistance;
                    index = points.IndexOf(point);
                }
            }
        }
        return index;
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

    private float PurePursuitControl(List<Transform> pointsPath)
    {

        int index = wp;//SearchPoint(pointsPath);

        Debug.Log(index);

        Vector3 point = new Vector3();

        if (index < pointsPath.Count)
        {
            point = pointsPath[index].position;
        }
        else
        {

            point = pointsPath[pointsPath.Count - 1].position;

        }

        Vector3 r = point - transform.position;
        alpha = Mathf.Atan2(r.z, r.x) - w;



        float Lf = k * vx + Lfc;



        float delta = ClampAngle(Mathf.Atan2(2 * L * Mathf.Sin(alpha) / Lf, 1) * Mathf.Rad2Deg, -wheelRotationAngle, wheelRotationAngle) * Mathf.Deg2Rad;


        return delta;

    }

}
