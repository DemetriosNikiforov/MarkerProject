
using UnityEngine;

public class DrawSearchEnemy : MonoBehaviour
{

    [SerializeField]
    private ControllerStates cs;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(cs.gameObject.transform.position, cs.Radius);
    }
}
