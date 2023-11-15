using System;

using System.Collections.Generic;
using UnityEditor;
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

    
    public Transform[] points;


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


   
    public List<Transform> pointsPath;


    Transform minDist;
    int index;

    [SerializeField]
    private float k = 0.1f; // Коэффициент дальности обзора вперед
    [SerializeField]
    private float Lfc = 0.1f; // Расстояние переднего обзора
    [SerializeField]
    private float Kp = 1f; // Коэффициент регулятора скорости P
    //[SerializeField]
    //private float dt = 1f; // Временной интервал, единица: с
    [SerializeField]
    private float L = 0.6f; // Колесная база автомобиля, ед .: 

    [SerializeField]
    private float vSpeed = 0f;
    [SerializeField]
    private float vTurn = 0;//Convert.ToSingle(40.0 * Mathf.PI / 180.0f)

    [SerializeField]
    float target_speed = 10.0f / 3.6f;
    [SerializeField]
    int lastIndex;
    [SerializeField]
    int target_ind;
    public float delta = Convert.ToSingle(Mathf.PI / 400);

    public float max_speed = 1f;//[m/s]
    public float min_speed = -0.5f;//[m/s]
    public float max_yaw_rate = Convert.ToSingle(40.0 * Mathf.PI / 180.0f); //[rad/s]
    public float max_accel = 0.2f;  // [m/ss]
    public float max_delta_yaw_rate = Convert.ToSingle(40.0 * Mathf.PI / 180.0); //[rad/ss]
    public float v_resolution = 0.01f; //[m/s]
    public float yaw_rate_resolution = Convert.ToSingle(0.1 * Mathf.PI / 180.0);  //[rad/s]
    public float dt = 0.1f; //[s] Time tick for motion prediction
    public float predict_time = 3.0f; //[s]
    public float to_goal_cost_gain = 0.15f;
    public float speed_cost_gain = 1.0f;
    public float obstacle_cost_gain = 1.0f;
    public float robot_stuck_flag_cons = 0.001f; //constant to prevent robot stucked

    private float[] X;
    private List<float[]> Trajectory = new List<float[]>();

    public float Angle = Convert.ToSingle(Mathf.PI / 8.0f);

    public int i = 40;

    private float[] U;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = myCenterMass;
    }
    // Start is called before the first frame update
    void Start()
    {
        minDist = transform;
        pointsPath = GameObject.Find("GameObject").GetComponent<CreatPath>().points;
        SearchPoint(pointsPath, ref minDist);
        index = pointsPath.IndexOf(minDist);

        lastIndex = pointsPath.Count - 1;
        target_ind = CalcTargetIndex();

        X = new float[] { transform.position.x, transform.position.z, Angle, 0f, 0f };
        Debug.Log(X[0].ToString() + "\t" + X[1].ToString() + "\t" + X[2].ToString() + "\t" + X[3].ToString() + "\t" + X[4].ToString());
        Trajectory.Add(X);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //List<float[]> trajectory = new List<float[]>();
        U = DwaControl(X, pointsPath[i].position, ref Trajectory);
        //Trajectory = trajectory;
        //Debug.Log(U[0].ToString() + "\t" + U[1].ToString());

        X = Motion(X, U, dt);
        Trajectory.Add(X);
        //Debug.Log(X[0].ToString() + "\t" + X[1].ToString() + "\t" + X[2].ToString() + "\t" + X[3].ToString() + "\t" + X[4].ToString());
        


        //transform.position = new Vector3(X[0], transform.position.y, X[1] );//*Time.deltaTime

        //X = Motion(X, u, dt);
        */





        /*if (index < pointsPath.Count)
        {
            Vector3 v = new Vector3(pointsPath[index].position.x, 0, pointsPath[index].position.z) - new Vector3(transform.position.x, 0, transform.position.z);
            _rb.MovePosition(transform.position + v.normalized * Time.deltaTime * speed);
            if (Vector3.Distance(pointsPath[index].position, transform.position) < 0.5f)
            {
                index++;
            }
        }*/





        /*

        if (lastIndex > target_ind)
        {
            float a = PIDControl(target_speed, vSpeed);
            
            
            PurePursuitControl(vSpeed, vTurn, target_ind, ref delta, ref target_ind);
            Transform t = transform;
            UpdateParametrs(ref t, ref vSpeed, ref vTurn, a, delta);
            
            transform.position = t.position;
        }
        */





    }

  
    /*
     * DWA
     */
    /*

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="u"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    private float[] Motion(float[] x, float[] u, float dt)
    {
        x[2] += u[1] * dt;
        x[0] += u[0] * Mathf.Cos(x[2]) * dt;
        x[1] += u[0] * Mathf.Sin(x[2]) * dt;
        x[3] = u[0];
        x[4] = u[1];
        return x;
    }

    private float[] CalcDynamicWindow(float[] x)
    {
        float[] Vs = { min_speed, max_speed, -max_yaw_rate, max_yaw_rate };
        float[] Vd = { x[3] - max_accel * dt, x[3] + max_accel * dt, x[4] - max_delta_yaw_rate * dt, x[4] + max_delta_yaw_rate * dt };
        float[] dw = { Mathf.Max(Vs[0], Vd[0]), Mathf.Min(Vs[1], Vd[1]), Mathf.Max(Vs[2], Vd[2]), Mathf.Min(Vs[3], Vd[3]) };
        return dw;
    }


    private List<float[]> PredictTrajectory(float[] x_init, float v, float y)
    {
        float[] x = x_init;
        List<float[]> trajectory = new List<float[]>();
        trajectory.Add(x);
        float time = 0;
        while (time <= predict_time)
        {
            x = Motion(x, new float[] { v, y }, dt);
            trajectory.Add(x);
            time += dt;
        }

        return trajectory;
    }

    private float CalcToGoalCost(List<float[]> trajectory, Vector3 goal)
    {
        float dx = goal.x - trajectory[trajectory.Count - 1][0];
        float dy = goal.z - trajectory[trajectory.Count - 1][1];
        float error_angle = Mathf.Atan2(dx, dy);
        float cost_angle = error_angle - trajectory[trajectory.Count - 1][2];
        float cost = Mathf.Abs(Mathf.Atan2(Mathf.Sin(cost_angle), Mathf.Cos(cost_angle)));

        return cost;
    }

    private float[] CalcControlAndTrajectory(float[] x, float[] dw, Vector3 goal, ref List<float[]> best_trajectory)
    {
        float[] x_init = x;
        float min_cost = float.PositiveInfinity;
        float[] best_u = { 0, 0 };
        //List<float[]> best_trajectory = new List<float[]>();
        best_trajectory = new List<float[]>();
        best_trajectory.Add(x);

        for (float v = dw[0]; v < dw[1]; v += v_resolution)
        {
            for (float y = dw[2]; y < dw[3]; y += yaw_rate_resolution)
            {
                List<float[]> trajectory = PredictTrajectory(x_init, v, y);
                float to_goal_cost = to_goal_cost_gain * CalcToGoalCost(trajectory, goal);
                float speed_cost = speed_cost_gain * (max_speed - trajectory[trajectory.Count - 1][3]);

                //ob_cost = config.obstacle_cost_gain * calc_obstacle_cost(trajectory, ob, config)

                float final_cost = to_goal_cost + speed_cost;

                if (min_cost >= final_cost)
                {
                    min_cost = final_cost;
                    best_u = new float[] { v, y };
                    best_trajectory = trajectory;

                    if (Mathf.Abs(best_u[0]) < robot_stuck_flag_cons && Mathf.Abs(x[3]) < robot_stuck_flag_cons)
                    {
                        best_u[1] = -max_delta_yaw_rate;
                    }
                }

            }

        }
        return best_u;
    }

    private float[] DwaControl(float[] x, Vector3 goal, ref List<float[]> t)
    {
        float[] dw = CalcDynamicWindow(x);

        List<float[]> bt = new List<float[]>();

        float[] u = CalcControlAndTrajectory(x, dw, goal, ref bt);

        t = bt;
        return u;
    }
    */

    /*
     * pp
     */

    private void SearchPoint(List<Transform> points, ref Transform shortDisrance)
    {
        if (points.Count != 0)
        {
            float minsquareDistance = Mathf.Pow(points[0].position.x - transform.position.x, 2) + Mathf.Pow(points[0].transform.position.y - transform.position.y, 2) + Mathf.Pow(points[0].transform.position.z - transform.position.z, 2);
            shortDisrance = points[0];
            foreach (Transform point in points)
            {
                float squareDistance = Mathf.Pow(point.position.x - transform.position.x, 2) + Mathf.Pow(point.transform.position.y - transform.position.y, 2) + Mathf.Pow(point.transform.position.z - transform.position.z, 2);
                if (minsquareDistance > squareDistance)
                {
                    minsquareDistance = squareDistance;
                    shortDisrance = point;
                }
            }
        }
    }


    private float PIDControl(float target, float current)
    {

        return Kp * (target - current);
    }


    private void UpdateParametrs(ref Transform pos, ref float vSpeed, ref float vTurn, float a, float delta)
    {
        pos.position = new Vector3(pos.position.x + vSpeed * Mathf.Cos(vTurn) * dt, pos.position.y, pos.position.z + vSpeed * Mathf.Sin(vTurn) * dt);


        vTurn = vTurn + vSpeed / L * Mathf.Tan(delta) * dt;
        vSpeed = vSpeed + a * dt;

    }



    private int CalcTargetIndex()
    {
        Transform minPoint = transform;
        SearchPoint(pointsPath, ref minPoint);
        float J = 0f;
        float Lf = vSpeed * k * Lfc;
        int index = pointsPath.IndexOf(minPoint);
        /*while ((Lf > J) && ((index + 1) < pointsPath.Count))
        {
            Vector3 vectorDifference = points[index + 1].position - points[index].position;
            J += Vector3.SqrMagnitude(vectorDifference);
            index++;
        }*/
        return index;

    }




    private void PurePursuitControl(float vSpeed, float vTurn, int pind, ref float delta, ref int index)
    {
        index = CalcTargetIndex();
        float alpha = 0;
        if (pind >= index)
        {
            index = pind;
            Vector3 purpose;
            if (index < pointsPath.Count)
            {
                purpose = pointsPath[index].position;
            }
            else
            {
                index = pointsPath.Count - 1;
                purpose = pointsPath[index].position;
            }


            alpha = Mathf.Atan2((purpose - pointsPath[index].position).z, (purpose - pointsPath[index].position).x) - vTurn;
            Debug.Log(alpha);
            //alpha = Mathf.Atan2((purpose - transform.position).x, (purpose - transform.position).z) - vTurn;
            //alpha = Mathf.Atan2((purpose - transform.position).y, (purpose - transform.position).x) - vTurn;



        }
        if (vSpeed < 0)
        {
            alpha -= Mathf.PI;
            float Lf = k * vSpeed + Lfc;


            delta = Mathf.Atan2(2 * L * Mathf.Sin(alpha) / Lf, 1);

        }
    }




}

