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



    private bool _timeLock = false;

    private int _bullets = 0;

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
        _bullets = Bullets;

        _animator = GetComponent<Animator>();

        _animator.applyRootMotion = false;

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
                Shooting(null);
            }

            float distanceToFinish = (transform.position - finish.position).sqrMagnitude;


            if (distanceToFinish <= 0.8f)
            {
                Collider firsEnemy = SearchEnemy(radius);
                if (firsEnemy != null)
                {
                    bool isRotateToEnemy = true;

                    if (_timeLock == false)
                    {
                        isRotateToEnemy = RotateToEnemy(firsEnemy.gameObject.transform.position);

                        _animator.SetTrigger("stopShoot");
                    }

                    if (isRotateToEnemy)
                    {
                        Shooting(firsEnemy.gameObject);

                    }
                }
                else
                {
                    _animator.SetTrigger("stopShoot");
                }
               



            }


        }

        else if (state == States.Stand)
        {
            _agent.isStopped = true;

            _animator.SetBool("isWalk", false);



            if (_isShoot)
            {
                Shooting(null);
            }

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

        Vector3 lerp = Vector3.up * Mathf.LerpAngle(transform.localEulerAngles.y, rotation.y, speedRotation * Time.deltaTime); //Vector3.Lerp(transform.localEulerAngles, rotation, speedRotation * Time.deltaTime);

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



            return true;
        }

        return true;

    }

    /// <summary>
    /// Функция стрельбы
    /// </summary>
    /// <param name="enemy">Игровой обьект врага</param>
    private void Shooting(GameObject enemy)
    {
        if (_timeLock == false)
        {
            //переменная для кулдауна
            _timeLock = true;

            //если найден противник и есть пули то производится выстрел
            if (enemy != null && Bullets > 0)
            {
                Bullets -= 1;

                shootEffect.Play();

                _animator.SetTrigger("shoot");

                enemy.SendMessage("Damage", damage);
            }

            //когда произведено 5 выстрелов наступает кулдаун перезарядки
            if (_bullets - Bullets == 5)
            {
                _bullets = Bullets;
                StartCoroutine(CoolDownShoting(timeCoolDawn + shootEffect.main.duration));
            }
            //кулдаун для выполнения системы частиц при выстреле
            else if (_bullets != 0)
            {
                StartCoroutine(CoolDownShoting(shootEffect.main.duration));
            }

        }
        else
        {
            //если система частиц выстрела не воспроизводится то меняем анимацию
            if (!shootEffect.isPlaying)
            {
                _animator.SetTrigger("stopShoot");
                shootEffect.Stop();
            }
            //иначе оставляем анимацию выстрела
            else
            {
                _animator.SetTrigger("shoot");
            }


        }
    }

    /// <summary>
    /// Корутина для кулдауна выстрелов
    /// </summary>
    /// <param name="time">Время кулдауна</param>
    /// <returns></returns>
    IEnumerator CoolDownShoting(float time)
    {
        yield return new WaitForSeconds(time);
        _timeLock = false;

        //место для выполнения инструкций после кулдауна
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

