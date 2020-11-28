using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDragRelease : MonoBehaviour
{
    [SerializeField] public Vector3 dragVector;
    [SerializeField] public bool dragging;
    private Vector3 start;
    private Vector3 end;


    public Vector3 GetMousePoint()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 0));
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
    }
}