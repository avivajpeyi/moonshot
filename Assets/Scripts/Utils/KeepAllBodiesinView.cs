using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAllBodiesinView : MonoBehaviour
{
    public List<GameObject> allBodies = new List<GameObject>();
    public bool keepUpdating = true;
    
    
    void Update()
    {
        if (keepUpdating)
        {
            KeepAllBodiesInView();            
        }
    }

    void KeepAllBodiesInView()
    {
        Vector2 lowestPosition = Vector2.zero;
        Vector2 highestPosition = Vector2.zero;

        bool first_body = true;

        foreach (var body in allBodies)
        {
            var pos = body.GetComponent<Rigidbody2D>().position;

            if (first_body)
            {
                lowestPosition = pos;
                highestPosition = pos;
                first_body = false;
            }
            else
            {
                if (pos.x < lowestPosition.x)
                    lowestPosition.x = pos.x;

                if (pos.y < lowestPosition.y)
                    lowestPosition.y = pos.y;

                if (pos.x > highestPosition.x)
                    highestPosition.x = pos.x;

                if (pos.y > highestPosition.y)
                    highestPosition.y = pos.y;
            }
        }

        float size = Mathf.Max(highestPosition.y - lowestPosition.y,
                         highestPosition.x - lowestPosition.x) * 0.5f;
        size += 10f;

        Vector3 center = lowestPosition + (highestPosition - lowestPosition) * 0.5f;
        center -= Vector3.forward * 20f;

        var camera = Camera.main;

        camera.transform.position =
            Vector3.Lerp(camera.transform.position, center, Time.deltaTime);
        camera.orthographicSize =
            Mathf.Lerp(camera.orthographicSize, size, Time.deltaTime);
    }
}