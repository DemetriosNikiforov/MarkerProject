using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerStatesVersion : MonoBehaviour
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

    [SerializeField]
    [Tooltip("Радиус зрения у GameObject:")]
    private float radius = 1;

    [Tooltip("Время перезарядки:")]
    [SerializeField]
    private float timeCoolDawn = 1f;

    [Tooltip("Количество патронов:")]
    [SerializeField]
    private int bullets = 15;

    [SerializeField]
    [Tooltip("Наносимый урон:")]
    private int damage = 1;

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




    [SerializeField]
    private LayerMask layerMask;

    public GameObject gO;




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

        _animator.SetFloat("speed", _agent.speed);

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
                _animator.SetBool("isWalk", true);

            }
            else
            {
                _animator.SetBool("isWalk", false);


            }



            for (int i = 0; i < _path.corners.Length - 1; i++)
            {

                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
            }

            if (_isShoot)
            {
                Shoot(1, null);
            }

            float distanceToFinish = (transform.position - finish.position).sqrMagnitude;


            if (distanceToFinish <= 0.8f)
            {
                _animator.applyRootMotion = false;
                Collider firsEnemy = SearchEnemy(radius);
                if (firsEnemy != null)
                {
                    bool isRotateToEnemy = RotateToEnemy(firsEnemy.gameObject.transform.position);
                    if (isRotateToEnemy)
                    {
                        Shoot(1, firsEnemy.gameObject);

                    }
                }



            }


        }

        else if (state == States.Stand)
        {
            _agent.isStopped = true;

            _animator.SetBool("isWalk", false);



            if (_isShoot)
            {
                Shoot(1, null);
            }

        }
    }


    /// <summary>
    /// Функция стрельбы
    /// </summary>
    /// <param name="state">Параметр слоя состояния анимации. Для ходьбы 2 а для состояния покоя 1 </param>
    private void Shoot(int state, GameObject enemy)
    {
        if (!_coolDawn && Bullets > 0)
        {
            _coolDawn = true;
            Bullets -= 1;
            if (enemy != null)
            {
                enemy.SendMessage("Damage", damage);
            }
            _animator.SetTrigger("shoot");


            StartCoroutine(ShootCoolDawn());

        }
        else
        {
            shootEffect.Stop();

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
    private bool RotateToEnemy(Vector3 enemyPosition)
    {

        Debug.DrawLine(_path.corners[0], enemyPosition);


        float angle = Mathf.Atan2(Vector3.Dot(Vector3.up, Vector3.Cross(gO.transform.forward, (enemyPosition - transform.position).normalized)), Vector3.Dot(gO.transform.forward, (enemyPosition - transform.position).normalized)) * Mathf.Rad2Deg;
        Vector3 rotation = transform.localEulerAngles + Vector3.up * angle;

        Vector3 lerp = Vector3.Lerp(transform.localEulerAngles, rotation, speedRotation * Time.deltaTime);

        if (transform.eulerAngles.y != lerp.y && Mathf.Abs(angle) > 1f)
        {
            transform.localEulerAngles = lerp;
            return false;
        }
        return true;

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

            _animator.applyRootMotion = false;

            //Quaternion rotation = Quaternion.LookRotation((path.corners[0] - path.corners[1]).normalized);
            Quaternion rotation = Quaternion.LookRotation((-path.corners[0] + path.corners[1]).normalized);

            rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

            //if (transform.rotation.eulerAngles.y != rotation.eulerAngles.y)
            if (transform.rotation != rotation)
            {

                transform.rotation = rotation;

                return false;
            }

            transform.rotation = rotation;

            _animator.applyRootMotion = true;

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

    /// <summary>
    /// Перечисление состояний
    /// </summary>
    enum States
    {
        Walking = 1,
        Stand = 0,
    }
}

