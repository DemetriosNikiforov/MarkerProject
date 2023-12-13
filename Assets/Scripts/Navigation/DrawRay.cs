using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRay : MonoBehaviour
{

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, -transform.forward * 100);
    }
}
