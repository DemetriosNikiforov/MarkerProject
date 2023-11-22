using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurePursuitToMarker : MonoBehaviour
{
    public List<Transform> pointsPath;


    Transform minDist;

    [Header("Угол поворота колес")]
    public float wheelRotationAngle = 30f;
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

    [SerializeField]
    private int target_index = 0;

    private int lastindex;


    public Transform pointMarker;

    public MarkerController markerController;
    // Start is called before the first frame update
    void Start()
    {
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        //dt = Time.deltaTime;
        target_index = CalcTargetIndex(pointsPath);
        lastindex = pointsPath.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        //dt = Time.deltaTime;
        if (lastindex > target_index)
        {
            float a = PContorl(target_speed, vSpeed);

            float delta = PurePursuitControl(pointsPath, ref target_index);

            UpdateParametrs(a, delta);


            markerController.speed = vSpeed;
            markerController.turn = vTurn;
            markerController.angle = delta;

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


        L = 0;

        float Lf = k * vSpeed + Lfc;


        while (Lf > L && (index + 1) < pointsPath.Count)
        {
            Vector3 distancePoint = pointsPath[index + 1].position - pointsPath[index].position;

            L += distancePoint.magnitude;


            index++;
        }



        return index;
    }


    private void UpdateParametrs(float a, float delta)
    {

        Vector3 dP = new Vector3(vSpeed * Mathf.Cos(vTurn) * dt, 0, vSpeed * Mathf.Sin(vTurn) * dt);
        //transform.position += dP;

        //vTurn += vSpeed / L * Mathf.Sin(delta) * dt;

        vTurn = ClampAngle((vTurn + vSpeed / L * Mathf.Sin(delta) * dt) * Mathf.Rad2Deg, -360, 360) * Mathf.Deg2Rad;
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
