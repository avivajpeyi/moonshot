using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Planetoid : MonoBehaviour
{
    [Header("Planetoid settings")] [SerializeField]
    public float mass;
    [SerializeField] private float size;
    [SerializeField] private Color color;
    private float sizeToMassFactor = 20.0f;
    private Rigidbody2D myBody;


    [Header("Orbit settings")] [SerializeField]
    protected Transform centerOfGravity;
    [Tooltip("Speed of orbit")]
    [SerializeField] protected float orbitSpeed;
    [Tooltip("Distance from Center of Gravity")]
    [SerializeField] protected float orbitRadius;
    [SerializeField] private bool isRotatingClockwise = true;
    protected float currentAngle = 0 ;

    protected bool isMoving = false;

    
    public void InitializePlanetoid(
        Transform centerOfGravity,
        float orbitSpeed,
        float orbitRadius,
        bool isRotatingClockwise,
        float size)
    {
        this.centerOfGravity = centerOfGravity;
        this.orbitSpeed = orbitSpeed;
        this.orbitRadius = orbitRadius;
        this.isRotatingClockwise = isRotatingClockwise;
        this.size = size;
        SetupPlanetoid();
    }

    protected void SetupPlanetoid()
    {
        SetupMass();
        SetupRigidbody();
        SetupColor();
        SetupOrbit();
    }
    

    void SetupRigidbody()
    {
        myBody = GetComponent<Rigidbody2D>();
        myBody.gravityScale = 0f;
        myBody.drag = 0f;
        myBody.angularDrag = 0f;
        myBody.mass = mass;
    }

    void SetupMass()
    {
        mass = size * sizeToMassFactor;
        transform.localScale = new Vector3(size, size, 0);
    }


    void SetupColor()
    {
        color = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.color = color;
        
        if (transform != centerOfGravity)
        {
            SetupTrail();
        }
    }

    void SetupTrail()
    {
        TrailRenderer tr = gameObject.AddComponent<TrailRenderer>();
        tr.material = new Material(Shader.Find("Sprites/Default"));
        tr.startWidth = size;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(color, 0.0f), new
                    GradientColorKey(color, 1.0f)
            },
            new GradientAlphaKey[]
                {new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(0, 1.0f)}
        );
        tr.colorGradient = gradient;
    }



    void SetupOrbit()
    {
        currentAngle = Random.Range(0f, 360f);
        if (transform != centerOfGravity)
        {
            transform.position = GetPositionOnCircle(currentAngle);
            isMoving = true;
        }
    }


    

    // Update is called once per frame
    protected void Update()
    {
        if (isMoving)
            MoveAlongOrbit();
    }

    protected Vector2 GetPositionOnCircle(float angle)
    {
        Vector2 centerPos = centerOfGravity.position;
        float x = centerPos.x + Mathf.Cos(angle) * orbitRadius;
        float y = centerPos.y + Mathf.Sin(angle) * orbitRadius;
        return new Vector2(x, y);
    }

    

    void MoveAlongOrbit()
    {
        float clockwiseMultiplier = (isRotatingClockwise) ? 1f : -1f;
        currentAngle += orbitSpeed * Time.deltaTime * clockwiseMultiplier;
        if (currentAngle >= 360f)
        {
            currentAngle = currentAngle - 360f * clockwiseMultiplier;
        }
        transform.position = GetPositionOnCircle(currentAngle);
    }
}