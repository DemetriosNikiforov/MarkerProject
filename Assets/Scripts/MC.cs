using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MC : MonoBehaviour
{
    [Header("”гол поворота колес")]
    public float wheelRotationAngle = 30f;
    public List<Transform> pointsPath;

    public MarkerController markerController;

    public int indexPoint = 0;
    Rigidbody rb;

    public float va = 25;
    public float phi = 0.9f;
    public float Ru;
    public float theta = 0;
    public float thetaU = 0;
    public float beta = 0;
    public float R;
    public float e;
    public float delta = 5f;
    public float K = 0.5f;
    public float K2 = 35f;
    public float t = 0.05f;
    public float phiD;
    public float u;
    // Start is called before the first frame update
    void Start()
    {
        markerController.speed = va;

        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        indexPoint = SearchPoint(pointsPath);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        Vector3 Wa = pointsPath[indexPoint].position;
        Vector3 Wb = pointsPath[indexPoint + 1].position;

        Ru = Mathf.Sqrt(Mathf.Pow(Wb.z - transform.position.z, 2) + Mathf.Pow(Wb.x - transform.position.x, 2));
        theta = Mathf.Abs(Mathf.Atan2(Wb.z - Wa.z, Wb.x - Wa.x));
        thetaU = Mathf.Abs(Mathf.Atan2(transform.position.z - Wa.z, transform.position.x - Wa.x));

        beta = Mathf.Abs(theta - thetaU);
        //beta = (theta - thetaU);

        R = Ru * Mathf.Cos(beta);
        e = Ru * Mathf.Sin(beta);

        Vector2 vt = new Vector2(Wa.x + (R + delta) * Mathf.Cos(theta), Wa.z + (R + delta) * Mathf.Sin(theta));

        if (Mathf.Abs(Mathf.Abs(e) - Mathf.Abs(Ru)) > 1f)
        {
            phiD = Mathf.Abs(Mathf.Atan2(vt.y - transform.position.z, vt.x - transform.position.x));
            //phiD = Mathf.Atan2(vt.y - transform.position.z, vt.x - transform.position.x);

            u = K * (phiD - phi) * va - K2 * e;
            //u = K * (phiD - phi) * va;

            if (u > 1)
            {
                u = 1;
            }

            phi = phiD;
            phi = ClampAngle(phiD * Mathf.Rad2Deg, -wheelRotationAngle, wheelRotationAngle) * Mathf.Deg2Rad;
            phiD = ClampAngle(phiD * Mathf.Rad2Deg, -wheelRotationAngle, wheelRotationAngle) * Mathf.Deg2Rad;

            float Vz = va * Mathf.Sin(phiD) + u * t;
            float Vx = Mathf.Sqrt((va * va) - (Vz * Vz));


            //markerController.speed = Mathf.Sqrt(Mathf.Pow(va, 2) + Mathf.Pow(Vz, 2));
            markerController.speed = Mathf.Sqrt(Mathf.Pow(Vx, 2) + Mathf.Pow(Vz, 2));
            markerController.angle = phiD;
            //va = markerController.speed;
            //transform.position = new Vector3(transform.position.x + Vx * t, transform.position.y, transform.position.z + Vz * t);


            //t += 0.1f;
        }
        else
        {
            //e = 3;
            if (indexPoint < pointsPath.Count - 2)
            {
                //indexPoint = CalcTargetIndex(pointsPath);
                indexPoint++;
            }

            //indexPoint++;

        }

        Debug.Log(rb.velocity);


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
}
