using System.Collections;
using UnityEngine;

public class Payload : MonoBehaviour, IMarkerUPNP
{
    [Header("Угол поворота башни:")]
    [SerializeField]
    private float rotationAngleY = 30f;
    [Header("Минимальный угол поворота ракетницы:")]
    [SerializeField]
    private float minRotationAngleX = -10f;
    [Header("Максимальный угол поворота ракетницы:")]
    [SerializeField]
    private float maxRotationAngleX = 30f;

    [Space]

    [Header("Время включения/выключения башни:")]
    [SerializeField]
    public float smoothTime = 1f;
    [Header("Скорость поворота башни по оси Y:")]
    [SerializeField]
    private float speedRotationY = 0.1f;
    [Header("скорость поворота ракетницы по оси X:")]
    [SerializeField]
    private float speedRotationX = 0.1f;

    [Space]

    [Header("Обьект полезной нагрузки:")]
    [SerializeField]
    private GameObject playload;

    [Header("Обьект полезной нагрузки для поворота по сои Y:")]
    [SerializeField]
    private Transform playloadRotationY;

    [Header("Обьект полезной нагрузки для поворота по сои X:")]
    [SerializeField]
    private Transform playloadRotationX;

    [Space]

    [Header("Вес полезной нагрузки:")]
    [SerializeField]
    private float weight = 0f;
    [Header("Prefab пули:")]
    [SerializeField]
    private GameObject bulletPrefab;
    [Header("Сила выстрела:")]
    [SerializeField]
    private float power = 10f;
    [Header("Точки выстрела полезной нагрузки:")]
    [SerializeField]
    private Transform pointShootRL;
    [SerializeField]
    private Transform pointShootLR;
    [Header("Физика маркера:")]
    [SerializeField]
    private Rigidbody rb;



    private const float _min = 0.322f;
    private const float _max = 1.142f;
    private bool _enable = false;
    private bool _disable = true;


    float IMarkerUPNP.Weight
    {
        get { return weight; }
        set
        {
            weight = value;
        }
    }



    // Start is called before the first frame update
    void Awake()
    {
        Init();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _enable)
        {
            DisavleUPNP();
        }
        if (!_enable && Input.GetKeyDown(KeyCode.R))
        {
            EnableUPNP();
        }




        if (Input.GetMouseButton(1) && _enable && !_disable)
        {
            RotatePlayloadY();
            RotatePlayloadX();
        }

        if (Input.GetMouseButtonDown(0) && _enable && !_disable)
        {
            Shoot();
        }
    }

    #region Custom methods
    public void Init()
    {
        rb.mass += weight;
    }

    public void EnableUPNP()
    {
        _disable = false;
        StartCoroutine(UpPlayload());


    }

    public void DisavleUPNP()
    {
        _disable = true;
        StartCoroutine(DownPlayload());
        StartCoroutine(StartAnglePlayload());


    }

    /// <summary>
    /// функция выстрела
    /// </summary>
    void Shoot()
    {
        GameObject bullet1 = Instantiate(bulletPrefab, pointShootRL.position, playloadRotationX.localRotation);
        GameObject bullet2 = Instantiate(bulletPrefab, pointShootLR.position, playloadRotationX.localRotation);

        bullet1.GetComponent<Rigidbody>().AddForce(power * playloadRotationX.forward, ForceMode.Impulse);
        bullet2.GetComponent<Rigidbody>().AddForce(power * playloadRotationX.forward, ForceMode.Impulse);
    }

    /// <summary>
    /// Процесс выключения полезной нагрузки
    /// </summary>
    /// <returns></returns>
    IEnumerator DownPlayload()
    {
        float localTime = 0f;
        Vector3 minVector = new Vector3(playload.transform.localPosition.x, _min, playload.transform.localPosition.z);
        Vector3 velocity = Vector3.zero;
        while (localTime < smoothTime)
        {
            playload.transform.localPosition = Vector3.SmoothDamp(playload.transform.localPosition, minVector, ref velocity, smoothTime - localTime);
            localTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        playload.transform.localPosition = minVector;
        _enable = false;

    }
    /// <summary>
    /// Возращение углов поворота полезной нагрузки в исходное положение
    /// </summary>
    /// <returns></returns>
    IEnumerator StartAnglePlayload()
    {
        float localTime = 0f;
        Vector3 startAngle = Vector3.zero;
        float velocity = 0f;
        float velocityX = 0f;

        while (localTime < smoothTime)
        {
            playloadRotationX.localEulerAngles = Vector3.right * Mathf.SmoothDampAngle(playloadRotationX.localEulerAngles.x, startAngle.x, ref velocityX, smoothTime - localTime);
            playloadRotationY.localEulerAngles = Vector3.up * Mathf.SmoothDampAngle(playloadRotationY.localEulerAngles.y, startAngle.y, ref velocity, smoothTime - localTime);
            localTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        playloadRotationY.localEulerAngles = startAngle;
        playloadRotationX.localEulerAngles = startAngle;

    }

    /// <summary>
    /// Процесс активации полезной нагрузки
    /// </summary>
    /// <returns></returns>
    IEnumerator UpPlayload()
    {

        float localTime = 0f;
        Vector3 maxVector = new Vector3(playload.transform.localPosition.x, _max, playload.transform.localPosition.z);
        Vector3 velocity = Vector3.zero;

        while (localTime < smoothTime)
        {
            playload.transform.localPosition = Vector3.SmoothDamp(playload.transform.localPosition, maxVector, ref velocity, smoothTime - localTime);
            localTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        playload.transform.localPosition = maxVector;
        _enable = true;

    }


    /// <summary>
    /// Поворачивает элемент полезной нагрузки по оси Y
    /// </summary>
    private void RotatePlayloadY()
    {
        playloadRotationY.localEulerAngles += Vector3.up * Input.GetAxis("Mouse X") * speedRotationY * Time.deltaTime;
        playloadRotationY.localEulerAngles = Vector3.up * ClampAngle(playloadRotationY.localEulerAngles.y, -rotationAngleY, rotationAngleY);
    }

    /// <summary>
    /// Поворачивает элемент полезной нагрузки по оси X
    /// </summary>
    private void RotatePlayloadX()
    {
        playloadRotationX.localEulerAngles += -1 * Vector3.right * Input.GetAxis("Mouse Y") * speedRotationX * Time.deltaTime;
        playloadRotationX.localEulerAngles = Vector3.right * ClampAngle(playloadRotationX.localEulerAngles.x, minRotationAngleX, maxRotationAngleX);

    }

    /// <summary>
    /// Функция ограничения диапазона угла
    /// </summary>
    /// <param name="value">Тукущее значение угла</param>
    /// <param name="min">Минимальное значение угла</param>
    /// <param name="max">Максимальное значение угла</param>
    /// <returns>Возращает угол из диапазона</returns>
    private float ClampAngle(float value, float min, float max)
    {
        if (value > 180)
        {
            value -= 360;
        }

        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
    #endregion
}






