using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 
/// </summary>
public class ControllerStates : MonoBehaviour
{
    [Header("Состояния и параметры GameObject:")]

    [Tooltip("Состояние GameObject:")]
    [SerializeField]
    private States state;

    [Tooltip("Начать стрельбу в данном состоянии:")]
    [SerializeField]
    private bool _isShoot = false;

    [Tooltip("Скорость поворота GameObject:")]
    [SerializeField]
    private float speedRotation;

    [Tooltip("Время перезарядки:")]
    [SerializeField]
    private float timeCoolDawn = 1f;

    [Tooltip("Количество патронов:")]
    [SerializeField]
    private int bullets = 15;

    [Header("эффект выстрела:")]

    [Tooltip("Система частиц выстрела:")]
    [SerializeField]
    private ParticleSystem shootEffect;

    [Tooltip("Время промежутка мкежду эффектот выстрела:")]
    [SerializeField]
    private float timeEffectCoolDawn = 0.1f;

    [Header("Точки:")]
    [Tooltip("Точка маршрута:")]
    [SerializeField]
    private Transform finish;

    [Tooltip("Точка атаки:")]
    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private float radius = 1;


    [SerializeField]
    private LayerMask layerMask;

    

    private NavMeshAgent _agent;
    private NavMeshPath _path;

    private int _indexPath = 1;

    private bool _isRotate;

    private Vector3 _lastPoint;

    private Animator _animator;

    private bool _coolDawn = false;

    private bool _startEffect = false;

    public float Radius
    {

        get { return radius; }
    }

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

            if (_isShoot)
            {
                Shoot(2);
            }

            float distanceToFinish = (transform.position - finish.position).sqrMagnitude;


            if (distanceToFinish <= 0.65f)
            {
                Collider firsEnemy = SearchEnemy(radius);
                if (firsEnemy != null)
                {
                    RotateToEnemy(firsEnemy.gameObject.transform.position);
                }


            }


        }

        else if (state == States.Stand)
        {
            _agent.isStopped = true;

            _animator.SetBool("isStand", _agent.isStopped);
            _animator.SetBool("isWalk", false);
            _animator.SetBool("isShoot", false);

            if (_isShoot)
            {
                Shoot(1);
            }

        }
    }



    /// <summary>
    /// Функция стрельбы
    /// </summary>
    /// <param name="state">Параметр слоя состояния анимации. Для ходьбы 2 а для состояния покоя 1 </param>
    private void Shoot(int state)
    {
        if (!_coolDawn && Bullets > 0)
        {
            _coolDawn = true;
            Bullets -= 1;

            _animator.SetBool("isShoot", true);


            StartCoroutine(ShootCoolDawn());

        }
        else
        {
            shootEffect.Stop();
            _animator.SetBool("isShoot", false);
        }


        //вызов системы частиц для выстрела когда начинается анимация
        if (_animator.GetCurrentAnimatorStateInfo(state).IsName("Shoot") && !_startEffect)
        {
            _startEffect = true;

            StartCoroutine(EffectCoolDown());
        }

    }

    /// <summary>
    /// Поиск противников
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private Collider SearchEnemy(float radius)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius, layerMask);

        foreach (Collider enemy in enemies)
        {
            return enemy;
        }
        return null;
    }


    /// <summary>
    /// Поворот в сторону противника
    /// </summary>
    /// <param name="enemyPosition"></param>
    private void RotateToEnemy(Vector3 enemyPosition)
    {

        Debug.DrawLine(_path.corners[0], enemyPosition);

        

        Quaternion rotation = Quaternion.LookRotation((transform.position - enemyPosition).normalized);

        rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

        //if (transform.rotation.eulerAngles.y != rotation.eulerAngles.y)
        if (transform.rotation != rotation)
        {

            transform.rotation = rotation;

        }

        transform.rotation = rotation;

    }

    /// <summary>
    /// функция поворота agenta в сторону следующей точки пути
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Имитация перезарядки 
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootCoolDawn()
    {

        yield return new WaitForSeconds(timeCoolDawn);
        _coolDawn = false;

    }

    /// <summary>
    /// Кд между вызовом ситемы частиц для выстрела
    /// </summary>
    /// <returns></returns>
    IEnumerator EffectCoolDown()
    {
        shootEffect.Play();
        yield return new WaitForSeconds(timeEffectCoolDawn);

        _startEffect = false;


    }

}

/// <summary>
/// Перечисление состояний
/// </summary>
enum States
{
    Walking = 1,
    Stand = 0,
}