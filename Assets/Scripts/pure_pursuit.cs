using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class pure_pursuit : MonoBehaviour
{

    public float L = 1;
    public int wp = 0;
    public float vx = 0.3f;
    public float theta = 0.5f;
    public float w_limit = 0.48f;
    public float w = 0;
    public float dt = 0.1f;


    public List<Transform> pointsPath;


    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            float tdot = position_vector(vx, w, theta);
            theta = theta + dt * tdot;

        }
    }

    float position_vector(float vx, float w, float theta)
    {

        //transform.position = transform.position + new Vector3(vx * Mathf.Cos(theta), 0, vx * Mathf.Sin(theta
        rb.velocity= new Vector3(vx * Mathf.Cos(theta), 0, vx * Mathf.Sin(theta)); 
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



}
