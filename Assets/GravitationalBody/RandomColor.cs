using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    
    
    public Color color;
    
    // Start is called before the first frame update
    void Start()
    {
        color = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));

        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
        
        myRenderer.color = color;
        
        // SetTrail(color);
    }

    void SetTrail(Color color)
    {
        TrailRenderer tr = GetComponent<TrailRenderer>();
        
        tr.material = new Material(Shader.Find("Sprites/Default"));

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new 
            GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(0, 1.0f) }
        );
        tr.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
