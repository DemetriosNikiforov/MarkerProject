using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ControllerStates : MonoBehaviour
{
    [Header("Состояние GameObject:")]
    [SerializeField]
    private States state;

    [Header("Точка маршрута:")]
    [SerializeField]
    private Transform finish;

    [Header("Скорость поворота GameObject:")]
    [SerializeField]
    private float speedRotation;

    [Header("Время перезарядки:")]
    [SerializeField]
    private float timeCoolDawn = 1f;

    [Header("Количество патронов:")]
    [SerializeField]
    private int bullets = 15;

    [Header("эффект выстрела:")]
    [SerializeField]
    private ParticleSystem shootEffect;


    private NavMeshAgent _agent;
    private NavMeshPath _path;

    private int _indexPath = 1;

    private bool _isRotate;

    private Vector3 _lastPoint;

    private Animator _animator;

    private bool _coolDawn = false;

    private int Bullets
    {

        get
        {
            return bullets;
        }
        set
        {
            bullets = value;
            if (bullets < 0)
            {
                bullets = 0;
            }
        }
    }


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _path = new NavMeshPath();

        _agent.updateRotation = false;

        transform.localScale = new Vector3(1, 1, -1);

    }


    // !!!надо по другому анимировать!!!
    void Update()
    {
        //вычисление пути к указанной точке 
        NavMesh.CalculatePath(transform.position, finish.position, NavMesh.AllAreas, _path);

        //сотояние ходьбы 
        if (state == States.Walking)
        {
            _animator.SetBool("isShoot", false);


            if (_lastPoint == null)
            {
                _lastPoint = _agent.destination;
            }




            if (_lastPoint != _path.corners[0])
            {
                _isRotate = RotateAgent(_path);
            }


            if (_isRotate)
            {
                _lastPoint = _path.corners[0];

                _agent.isStopped = false;

                if (_path.corners.Length > 1)
                {
                    _agent.destination = _path.corners[_indexPath];
                }
                else
                {
                    _agent.destination = _path.corners[0];
                }

            }


            //анимация ходьбы в зависимости от вектора скорости
            if (_agent.velocity.sqrMagnitude > 0 && _isRotate)
            {
                _animator.SetBool("isStand", false);
                _animator.SetBool("isWalk", true);
            }
            else
            {
                _animator.SetBool("isStand", true);
                _animator.SetBool("isWalk", false);
            }



            for (int i = 0; i < _path.corners.Length - 1; i++)
            {

                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
            }


        }

        else if (state == States.Stand)
        {
            _agent.isStopped = true;

            _animator.SetBool("isStand", _agent.isStopped);
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isShoot", false);

        }
        else if (state == States.Shoot)
        {


            _agent.isStopped = true;
            _animator.SetBool("isStand", _agent.isStopped);
            _animator.SetBool("isWalk", false);

            if (!_coolDawn && Bullets > 0)
            {
                _animator.SetBool("isShoot", !_coolDawn);
                shootEffect.Play();

                StartCoroutine(ShootCoolDawn());
            }
            else
            {
                shootEffect.Stop();
                _animator.SetBool("isShoot", false);
            }
        }
    }

    //функция поворота agenta в сторону следующей точки пути
    private bool RotateAgent(NavMeshPath path)
    {
        if (path.corners.Length > 1)
        {

            _agent.isStopped = true;

            Quaternion rotation = Quaternion.LookRotation((path.corners[0] - path.corners[1]).normalized);

            rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

            //if (transform.rotation.eulerAngles.y != rotation.eulerAngles.y)
            if (transform.rotation != rotation)
            {

                transform.rotation = rotation;

                return false;
            }

            transform.rotation = rotation;

            return true;
        }
        return true;

    }

    IEnumerator ShootCoolDawn()
    {
        _coolDawn = true;
        yield return new WaitForSeconds(timeCoolDawn);

        Bullets -= 5;
        _coolDawn = false;

    }

}

enum States
{
    Walking = 1,
    Stand = 0,
    Shoot = 2
}