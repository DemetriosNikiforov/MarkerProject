using System.Collections;
using UnityEngine;

public class Payload : MonoBehaviour, IMarkerUPNP
{
    [Header("���� �������� �����:")]
    [SerializeField]
    private float rotationAngleY = 30f;
    [Header("����������� ���� �������� ���������:")]
    [SerializeField]
    private float minRotationAngleX = -10f;
    [Header("������������ ���� �������� ���������:")]
    [SerializeField]
    private float maxRotationAngleX = 30f;

    [Space]

    [Header("����� ���������/���������� �����:")]
    [SerializeField]
    public float smoothTime = 1f;
    [Header("�������� �������� ����� �� ��� Y:")]
    [SerializeField]
    private float speedRotationY = 0.1f;
    [Header("�������� �������� ��������� �� ��� X:")]
    [SerializeField]
    private float speedRotationX = 0.1f;

    [Space]

    [Header("������ �������� ��������:")]
    [SerializeField]
    private GameObject playload;

    [Header("������ �������� �������� ��� �������� �� ��� Y:")]
    [SerializeField]
    private Transform playloadRotationY;

    [Header("������ �������� �������� ��� �������� �� ��� X:")]
    [SerializeField]
    private Transform playloadRotationX;

    [Space]

    [Header("��� �������� ��������:")]
    [SerializeField]
    private float weight = 0f;
    [Header("Prefab ����:")]
    [SerializeField]
    private GameObject bulletPrefab;
    [Header("���� ��������:")]
    [SerializeField]
    private float power = 10f;
    [Header("����� �������� �������� ��������:")]
    [SerializeField]
    private Transform pointShootRL;
    [SerializeField]
    private Transform pointShootLR;
    [Header("������ �������:")]
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
    /// ������� ��������
    /// </summary>
    void Shoot()
    {
        GameObject bullet1 = Instantiate(bulletPrefab, pointShootRL.position, playloadRotationX.localRotation);
        GameObject bullet2 = Instantiate(bulletPrefab, pointShootLR.position, playloadRotationX.localRotation);

        bullet1.GetComponent<Rigidbody>().AddForce(power * playloadRotationX.forward, ForceMode.Impulse);
        bullet2.GetComponent<Rigidbody>().AddForce(power * playloadRotationX.forward, ForceMode.Impulse);
    }

    /// <summary>
    /// ������� ���������� �������� ��������
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
    /// ���������� ����� �������� �������� �������� � �������� ���������
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
    /// ������� ��������� �������� ��������
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
    /// ������������ ������� �������� �������� �� ��� Y
    /// </summary>
    private void RotatePlayloadY()
    {
        playloadRotationY.localEulerAngles += Vector3.up * Input.GetAxis("Mouse X") * speedRotationY * Time.deltaTime;
        playloadRotationY.localEulerAngles = Vector3.up * ClampAngle(playloadRotationY.localEulerAngles.y, -rotationAngleY, rotationAngleY);
    }

    /// <summary>
    /// ������������ ������� �������� �������� �� ��� X
    /// </summary>
    private void RotatePlayloadX()
    {
        playloadRotationX.localEulerAngles += -1 * Vector3.right * Input.GetAxis("Mouse Y") * speedRotationX * Time.deltaTime;
        playloadRotationX.localEulerAngles = Vector3.right * ClampAngle(playloadRotationX.localEulerAngles.x, minRotationAngleX, maxRotationAngleX);

    }

    /// <summary>
    /// ������� ����������� ��������� ����
    /// </summary>
    /// <param name="value">������� �������� ����</param>
    /// <param name="min">����������� �������� ����</param>
    /// <param name="max">������������ �������� ����</param>
    /// <returns>��������� ���� �� ���������</returns>
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






