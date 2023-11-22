using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corrow : MonoBehaviour
{

    [Header("”гол поворота колес")]
    public float wheelRotationAngle = 30f;
    public List<Transform> pointsPath;

    public MarkerController markerController;

    public int indexPoint = 0;

    public float Ru;
    public float theta;
    public float thetau;
    public float beta;
    public float R;
    public float alpha;
    public float sigmad;
    public float u;

    public float k;
    public float sigma;

    public float V = 25f;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        indexPoint = SearchPoint(pointsPath);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (indexPoint < pointsPath.Count - 1)
        {
            Vector3 W = pointsPath[indexPoint].position;
            Vector3 Wi = pointsPath[indexPoint + 1].position;
            Ru = (W - transform.position).magnitude;
            theta = Mathf.Atan2(Wi.z - W.z, Wi.x - W.x);
            thetau = Mathf.Atan2(transform.position.z - W.z, transform.position.x - W.x);
            beta = theta - thetau;
            R = Mathf.Sqrt(Mathf.Pow(Ru, 2) - Mathf.Pow(Ru * Mathf.Sin(beta), 2));



            Vector3 S = new Vector3(((alpha + R) * Mathf.Cos(theta)), 0, ((alpha + R) * Mathf.Sin(theta)));

            sigmad = Mathf.Atan2(S.z - transform.position.z, S.x - transform.position.x);

            u = k * (sigmad - sigma);

            sigma = sigmad;

            rb.velocity = new Vector3(V*Mathf.Cos(sigma), 0, V* Mathf.Sin(sigma));

            if (Ru < 0.6f)
            {
                indexPoint++;
            }
        }
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
}
