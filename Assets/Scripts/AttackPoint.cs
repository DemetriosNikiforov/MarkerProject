
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
