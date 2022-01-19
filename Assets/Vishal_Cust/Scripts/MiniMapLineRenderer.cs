using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapLineRenderer : MonoBehaviour
{
    public Transform[] points;
    Vector3[] _points;
    LineRenderer lineRenderer;
    private void Start()
    {
        points = new Transform[this.transform.childCount];
        _points = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i);
            _points[i] = points[i].position;
        }
        DrawLine(3f, Color.red);


    }
    private void LateUpdate()
    {
        if (lineRenderer)
        {
        }
    }
    public void DrawLine(float width, Color colour)
    {
        LineRenderer lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.receiveShadows = false;
        lineRenderer.material.color = colour;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(_points);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

}
