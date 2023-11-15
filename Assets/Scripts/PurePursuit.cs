
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurePursuit : MonoBehaviour
{
    //[SerializeField]
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
    private float target_speed = 1f / 3.6f;

    private float radians = (Mathf.PI / 180);

    [SerializeField]
    private int target_index = 0;

    private int lastindex;


    // Start is called before the first frame update
    void Start()
    {
        minDist = transform;
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;

        dt = Time.deltaTime;

        target_index = CalcTargetIndex(pointsPath);
        lastindex = pointsPath.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        dt = Time.deltaTime;

        if (lastindex > target_index)
        {
            float a = PContorl(target_speed, vSpeed);

            float delta = PurePursuitControl(pointsPath, ref target_index);
            UpdateParametrs(a, delta);

        }
    }


    private int CalcTargetIndex(List<Transform> pointsPath)
    {
        //
        int index = 0;

        Vector3[] distances = new Vector3[pointsPath.Count];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = pointsPath[i].position - transform.position;
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
            L += Mathf.Sqrt(distancePoint.sqrMagnitude);
            //L += Vector3.Distance(pointsPath[index + 1].position, pointsPath[index].position);
            //L = distancePoint.sqrMagnitude/4;
            index++;
        }



        return index;
    }


    private void UpdateParametrs(float a, float delta)
    {

        Vector3 dP = new Vector3(vSpeed * Mathf.Cos(vTurn) * dt, 0, vSpeed * Mathf.Sin(vTurn) * dt);
        transform.position += dP;

        vTurn += vSpeed / L * Mathf.Sin(delta) * dt;

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

        Vector3 r = point - transform.position;
        float alpha = Mathf.Atan2(r.z, r.x) - vTurn;

        if (vSpeed < 0)
        {
            alpha = Mathf.PI - alpha;
        }

        float Lf = k * vSpeed + Lfc;

        float delta = Mathf.Atan2(2 * L * Mathf.Sin(alpha) / Lf, 1);

        pindex = index;

        return delta;

    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, L);
    }


}
