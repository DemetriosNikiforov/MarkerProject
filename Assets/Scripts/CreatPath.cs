
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreatPath : MonoBehaviour
{

    
    public List<Transform> pointsPath;
    public List<Transform> points;

    public float offset = 0.1f;

    private void Awake()
    {
        GameObject pathBetweenTargetPoints = new GameObject("Путь между целевыми точками");
        for (int i = 1; i < pointsPath.Count; i++)
        {
            Vector3 guidingVector = pointsPath[i].position - pointsPath[i - 1].position;
            guidingVector.Normalize();

            float dist = Vector3.Distance(pointsPath[i].position, pointsPath[i - 1].position);

            int count = Convert.ToInt32(dist / offset);
            //Debug.Log(guidingVector);
            for (int j = 1; j < count; j++)
            {
                GameObject gO = new GameObject();
                gO.transform.parent = pathBetweenTargetPoints.transform;
                gO.transform.position = pointsPath[i - 1].position + guidingVector * j * offset;
                points.Add(gO.transform);
               
            }
        }
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        for (int i = 1; i < pointsPath.Count; i++)
        {
            Vector3 guidingVector = pointsPath[i].position - pointsPath[i - 1].position;
            guidingVector.Normalize();

            float dist = Vector3.Distance(pointsPath[i].position, pointsPath[i - 1].position);

            int count = Convert.ToInt32(dist / offset);
            //Debug.Log(guidingVector);
            for (int j = 1; j < count; j++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pointsPath[i - 1].position + guidingVector * j * offset, 0.1f);

            }

        }

    }
}
