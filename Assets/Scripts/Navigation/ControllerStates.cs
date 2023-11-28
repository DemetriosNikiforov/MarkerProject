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
    private int indexPath = 1;
    //показывает повернулся ли agent
    private bool isRotate;


    private Animator _animator;


    void Awake()
    {
        _animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        agent.updateRotation = false;

        transform.localScale = new Vector3(1, 1, -1);

    }


    void Update()
    {





        //создания пути
        NavMesh.CalculatePath(transform.position, finish.position, NavMesh.AllAreas, path);

        //сотояние ходьбы к точке
        if (state == States.Walking)
        {


            _animator.SetBool("isStand", agent.isStopped);
            _animator.SetBool("isWalk", isRotate);

            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {


                isRotate = RotateAgent(path);



            }


            if (isRotate)
            {


                agent.isStopped = false;

                if (path.corners.Length > 1)
                {
                    agent.destination = path.corners[indexPath];
                }
                else
                {
                    agent.destination = path.corners[0];
                }


            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }


        }

        else if (state == States.Stand)
        {
            agent.isStopped = true;

            _animator.SetBool("isStand", agent.isStopped);

        }
        else if (state == States.Shoot)
        {
            //shoot
        }




    }

    //функция поворота agenta в сторону следующей точки пути
    private bool RotateAgent(NavMeshPath path)
    {
        if (path.corners.Length > 1)
        {


            //Quaternion rotation = Quaternion.LookRotation((path.corners[0] - path.corners[1]).normalized);
            Quaternion rotation = Quaternion.LookRotation((path.corners[0] - path.corners[1]).normalized);

            //Debug.Log(rotation.eulerAngles);

            rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

            //Vector3 angle = Vector3.up * Mathf.LerpAngle(transform.eulerAngles.y, rotation.eulerAngles.y, speedRotation);


            //if (transform.eulerAngles.y != angle.y)
            if (transform.rotation != rotation)
            //if (transform.rotation.eulerAngles.y != rotation.eulerAngles.y)
            {

                agent.isStopped = true;
                transform.rotation = rotation;

                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle.y, transform.eulerAngles.z);
                return false;

            }
            return true;
        }
        return true;

    }

}

enum States
{
    Walking = 1,
    Stand = 0,
    Shoot = 2
}