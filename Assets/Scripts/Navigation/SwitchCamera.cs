using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Tooltip("Камера от 1 лица:")]
    [SerializeField]
    private GameObject fpc;

    [Tooltip("Камера следящая за персонажем:")]
    [SerializeField]
    private GameObject fc;

    [Tooltip("Камера от 3 лица:")]
    [SerializeField]
    private GameObject tpc;

    [Tooltip("Если false то отключаются все камеры:")]
    [SerializeField]
    private bool ActiveCinemachine = false;


    void Awake()
    {
        if (ActiveCinemachine == false)
        {
            fpc.SetActive(ActiveCinemachine);
            fc.SetActive(ActiveCinemachine);
            tpc.SetActive(ActiveCinemachine);
        }
        else
        {
            fc.SetActive(ActiveCinemachine);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1) && ActiveCinemachine)
        {
            fpc.SetActive(true);

            fc.SetActive(false);
            tpc.SetActive(false);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && ActiveCinemachine)
        {
            fc.SetActive(true);

            fpc.SetActive(false);
            tpc.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && ActiveCinemachine)
        {
            tpc.SetActive(true);

            fc.SetActive(false);
            fpc.SetActive(false);

        }
        else if (ActiveCinemachine == false)
        {
            fpc.SetActive(ActiveCinemachine);
            fc.SetActive(ActiveCinemachine);
            tpc.SetActive(ActiveCinemachine);
        }



    }
}
