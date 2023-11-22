using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


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
    private NavMeshPath localPath;
    public int indexPath = 1;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        localPath = new NavMeshPath();

        //agent.updatePosition = false;
    }

    // Update is called once per frame
    void Update()
    {



        if (state == States.Walking)
        {
            NavMesh.CalculatePath(transform.position, finish.position, NavMesh.AllAreas, path);

            agent.SetDestination(path.corners[indexPath]);

            
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);

          

            //Vector3 dir = (path.corners[indexPath] - transform.position).normalized;
            //Vector3 dir = path.corners[indexPath] - transform.position;

            //float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            //transform.eulerAngles = Mathf.LerpAngle(transform.eulerAngles.y, angle, speedRotation)*Vector3.up;

            //transform.eulerAngles

            //Quaternion rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y),Vector3.up);

            //transform.rotation = rotation;
            //Debug.Log(rotation);
            Debug.Log(transform.rotation);

            //indexPath = 1;










            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }




            //agent.destination = finish.position;
            //agent.path.corners[indexPath]

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

    private bool LookPoint(NavMeshPath path)
    {

        Vector3 dir = path.corners[indexPath] - transform.position;

        float angle = Mathf.LerpAngle(transform.localEulerAngles.y, Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg, speedRotation);


        if (transform.eulerAngles.y != angle)
        {
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
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