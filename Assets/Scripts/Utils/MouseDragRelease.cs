using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDragRelease : MonoBehaviour
{
    [SerializeField] public Vector3 dragVector;
    [SerializeField] public bool dragging;
    public Vector3 start;
    private Vector3 end;
    private LineRenderer DebugLine;

    private bool debuggingMode = false;

    

    private void Start()
    {
        OptionsContainer op = FindObjectOfType<OptionsContainer>();
        if (op != null)
        {
            debuggingMode = op.DebuggingMode;
            if (debuggingMode)
            {
                GameObject go = new GameObject();
                DebugLine = go.AddComponent<LineRenderer>();
                DebugLine.material = new Material(Shader.Find("Sprites/Default"));
                DebugLine.startColor = Color.green;
                DebugLine.endColor = Color.green;
                DebugLine.startWidth = 0.1f;
                DebugLine.endWidth = 0.1f;
            }
        }
    }

    public Vector3 GetMousePoint()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 0)); 
        return new Vector3(
            x: worldPoint.x,
            y: worldPoint.y,
            z:0
        );
    }

    
    void  OnDrawGizmos()
    {
        if (dragging)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, end);
        }

        
    }

    private void ClickStart()
    {
        if (!dragging)
        {
            dragging = true;
            dragVector = Vector3.zero;
            start = GetMousePoint();
            end = GetMousePoint();
        }
    }


    private void Drag()
    {
        end = GetMousePoint();
        dragVector = end - start;
    }

    private void ClickRelease()
    {
        if (dragging)
        {
            dragging = false;
            start = Vector3.zero;
            end = Vector3.zero;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (dragging)
                Drag();
            else
                ClickStart();
        }
        else
            ClickRelease();
        
        if (debuggingMode)
        {
            if (dragging)
            {
                DebugLine.enabled = true;
                DebugLine.SetPosition(0, start);
                DebugLine.SetPosition(1, end);   
            }
            else
            {
                DebugLine.enabled = false;
            }
            
        }
        
    }
}