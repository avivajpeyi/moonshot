using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Planetoid : MonoBehaviour
{
    [Header("Planetoid settings")] [SerializeField]
    public float mass;
    [SerializeField] public float size;
    [SerializeField] private Color color;
    private float sizeToMassFactor = 20.0f;
    private Rigidbody2D myBody;
    private SpriteRenderer myRenderer;
    private TrailRenderer myTrailRenderer;
    public SpriteRenderer myOutline;
    public ParticleSystem myPluseRings;
    public ParticleSystem mySparks;
    public AudioSource sound;
    public float tol;
    public float dispmag;
    Color startColor ;
    Color endColor ;
    

    [Header("Orbit settings")] [SerializeField]
    protected Transform centerOfGravity;
    [Tooltip("Speed of orbit")]
    [SerializeField] protected float orbitSpeed;
    [Tooltip("Distance from Center of Gravity")]
    [SerializeField] protected float orbitRadius;
    [SerializeField] private bool isRotatingClockwise = true;
    [SerializeField]protected float currentAngle = 0 ;
    [SerializeField]float startAngle;
    protected bool isMoving = false;


    public bool isStar = false;
    
    private float lastColorChangeTime;

    

    public void InitializePlanetoid(
        Transform centerOfGravity,
        float orbitSpeed,
        float orbitRadius,
        bool isRotatingClockwise,
        float size)
    {
        myBody = GetComponent<Rigidbody2D>();
        myRenderer = GetComponent<SpriteRenderer>();
        myTrailRenderer = gameObject.AddComponent<TrailRenderer>();
        color = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        this.centerOfGravity = centerOfGravity;
        this.orbitSpeed = orbitSpeed;
        this.orbitRadius = orbitRadius;
        this.isRotatingClockwise = isRotatingClockwise;
        this.size = size;
        SetupPlanetoid();
    }

    protected void SetupPlanetoid()
    {
        if (transform == centerOfGravity)
        {
            isStar = true;
        }
        SetupMass();
        SetupRigidbody();
        SetupRenderer();
        SetupOrbit();
    }
    

    void SetupRigidbody()
    {
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


    public void UpdateColors(Color newColor)
    {
        color = newColor;
        Debug.Log("Setting color "+ newColor);
        SetupRenderer();
    }

    void SetupRenderer()
    {
        myRenderer.color = color;
        startColor = color;
        endColor = color;
        endColor.a = 0.7f;
        
        if (!isStar)
        {
            SetupTrail();
            myPluseRings.gameObject.SetActive(false);
            mySparks.gameObject.SetActive(true);
            mySparks.colorOverLifetime.color.gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(color, 0.0f),
                    new GradientColorKey(color, 0.5f),
                    new GradientColorKey(color, 1.0f),
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0, 0.0f),
                    new GradientAlphaKey(1, 0.5f),
                    new GradientAlphaKey(0, 1.0f)
                });
            mySparks.Pause();
            
        }
        else
        {
            myPluseRings.gameObject.SetActive(true);
            mySparks.gameObject.SetActive(false);
            myPluseRings.colorOverLifetime.color.gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(color, 0.0f),
                    new GradientColorKey(color, 0.5f),
                    new GradientColorKey(color, 1.0f),
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0, 0.0f),
                    new GradientAlphaKey(1, 0.5f),
                    new GradientAlphaKey(0, 1.0f)
                });
            myPluseRings.Play();
        }

    }
    

    void SetupTrail()
    {
        myTrailRenderer.startWidth = size;
        myTrailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(color, 0.0f), new
                    GradientColorKey(color, 1.0f)
            },
            new GradientAlphaKey[]
                {new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0, 1.0f)}
        );
        myTrailRenderer.colorGradient = gradient;
    }

    void SetupOrbit()
    {
        currentAngle = Random.Range(0f, 360f);
        startAngle = currentAngle;
        if (!isStar)
        {
            transform.position = GetPositionOnCircle(currentAngle);
            isMoving = true;
        }
    }


    void PulseColor()
    {
        var ratio = (Time.time - lastColorChangeTime);
        ratio = Mathf.Clamp01(ratio);

        if (ratio == 1f)
        {
            lastColorChangeTime = Time.time;

            // Switch colors
            var temp = startColor;
            startColor = endColor;
            endColor = temp;
        }
        myRenderer.color = Color.Lerp(startColor, endColor, ratio);

    }


    // Update is called once per frame
    protected void Update()
    {
        if (isMoving)
            MoveAlongOrbit();
        if (isStar)
            PulseColor();
        if (SoundCheck(tol))
            PlaySound();
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
    bool SoundCheck(float tol)
    {
        Vector2 startPosition = GetPositionOnCircle(startAngle);
        Vector2 pos = transform.position;
        Vector2 disp = startPosition - pos;
        dispmag = disp.magnitude;
        if (dispmag<= tol){
            return true;
        }
        else {
            return false;
        }
        
    }
    void PlaySound(){
        //sound.Play();
        Debug.Log("Beep");
    }

}