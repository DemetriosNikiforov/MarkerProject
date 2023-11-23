using UnityEngine;
using UnityEngine.AI;



public class ControllerStates : MonoBehaviour
{
    [SerializeField]
    private States state;
    [SerializeField]
    private Transform finish;
    [SerializeField]
    private float speedRotation;

    private NavMeshAgent agent;
    private NavMeshPath path;
    public int indexPath = 1;

    public bool isRotate;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        agent.updateRotation = false;

        
    }

    // Update is called once per frame
    void Update()
    {



        if (state == States.Walking)
        {
            NavMesh.CalculatePath(transform.position, finish.position, NavMesh.AllAreas, path);

            isRotate = RotateAgent(path);

            
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {

                if (isRotate)
                {
                    agent.isStopped = false;

                    if (indexPath + 1 <= path.corners.Length)
                    {
                        indexPath = 1;
                    }
                    else if (path.corners.Length == 1)
                    {

                        indexPath = 0;
                        agent.SetDestination(path.corners[indexPath]);
                    }

                    agent.SetDestination(path.corners[indexPath]);
                }




            }


            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }


        }
        else if (state == States.Stand)
        {
            //indexPath = 0;
        }
        else if (state == States.Shoot)
        {
            //shoot
        }




    }

    private bool RotateAgent(NavMeshPath path)
    {

        Quaternion rotation = Quaternion.LookRotation((path.corners[0] - path.corners[1]).normalized);
        rotation = Quaternion.Slerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

        if (transform.rotation != rotation)
        {
            agent.isStopped = true;
            transform.rotation = rotation;
            return false;

        }
        return true;

    }

}

enum States
{
    Walking = 0,
    Stand = 1,
    Shoot = 2
}