using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Object that can be shot
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    private float shotForce;
    public float maxForce = 20;

    [Tooltip("Max distance rocket will gravitationally be affected by planets")]
    public float maxDistance = 1000;

    public float offset = 90.0f;
    public float mass = 20;
    public Vector2 initialVelocity;

    private MouseDragRelease mouseController;

    [SerializeField] private Vector2 NetForce;

    [SerializeField] private bool gravitationOn = false;
    [SerializeField] private bool grounded = true;
    private Rigidbody2D myBody;
    private TrailRenderer myTrailRender;
    private float trailTime;
    private Color myColor;
    private SpriteRenderer mySpriteRenderer;

    //make a list of bodies so that we don't need to Find them every frame
    public List<Planetoid> attractableBodies = new List<Planetoid>();

    private Planetoid parentPlanet;

    public GameObject arrow;

    private StarSystemSpawner starSystemSpawner;
    private Collider2D myCollider2D;


    void Start()
    {
        mouseController = gameObject.AddComponent<MouseDragRelease>();
        starSystemSpawner = FindObjectOfType<StarSystemSpawner>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myColor = mySpriteRenderer.color;
        myBody = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();
        myTrailRender = GetComponent<TrailRenderer>();
        
        myTrailRender.autodestruct = false;
        trailTime = myTrailRender.time;
        myTrailRender.time = 0;
        StartCoroutine(InitAttractorList());
    }


    IEnumerator WaitBeforeEnablingParentGravity()
    {
        if (parentPlanet != null)
            parentPlanet.GetComponent<Collider2D>().enabled = true;


        Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(
            point:transform.position,
            radius:mySpriteRenderer.size.magnitude
            );

        foreach (var col in allOverlappingColliders)
        {
            col.enabled = false;
        }

        float waitTime = 1.0f;
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Adding parent --> curr len " + attractableBodies.Count);
        foreach (var col in allOverlappingColliders)
        {
            col.enabled = true;
        }
        Debug.Log("new len " + attractableBodies.Count);
        gravitationOn = true;
    }


    private IEnumerator InitAttractorList()
    {
        yield return new WaitForSeconds(.1f);
        attractableBodies = starSystemSpawner.allBodies;
        LandOnPlanet(starSystemSpawner.start.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (grounded)
        {
            CalculateShot(mouseController.dragVector);
        }
        else
        {
            GravitationalMovement();
        }
    }


    IEnumerator TurnOffTrail()
    {
        float lerpDuration = 1.0f;
        float timeElapsed = 0;

        float currentTime = myTrailRender.time;
        while (timeElapsed < lerpDuration)
        {
            myTrailRender.time =
                Mathf.Lerp(currentTime, 0.01f, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }


    void GravitationalMovement()
    {
        if (gravitationOn)
        {
            NetForce = GetNetGravitationalForce();
            myBody.AddForce(NetForce);
        }

        Rotate();
    }


    void CalculateShot(Vector3 shotVector)
    {
        if (shotVector == Vector3.zero)
        {
            shotVector = mouseController.GetMousePoint() - transform.position;
            shotVector.Normalize();
        }

        shotForce = Mathf.Clamp(
            value: shotVector.magnitude,
            min: 1.0f,
            max: maxForce
        );


        if (Input.GetMouseButtonUp(0))
        {
            TakeShot(shotVector);
        }

        UpdateShotArrow(shotVector);
    }


    void TakeShot(Vector3 shotVector)
    {
        Debug.Log("Take shot");
        initialVelocity = shotVector.normalized * shotForce;
        shotForce = 0;
        parentPlanet.myOutline.enabled = false;
        StartCoroutine(WaitBeforeEnablingParentGravity());
        TogglePhysics(true);
    }

    void UpdateShotArrow(Vector3 shotVector)
    {
        if (mouseController.dragging)
        {
            // Update rotation
            float angle = Mathf.Atan2(shotVector.y, shotVector.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
            arrow.transform.rotation = rotation;


            // Update scaling
            float scaleX = Mathf.Log(1 + shotForce / 2, 2) * 10.2f;
            float scaleY = Mathf.Log(1 + shotForce / 2, 2) * 10.2f;
            arrow.transform.localScale = new Vector3(1 + scaleX, 1 + scaleY, 0.001f);
        }
        else
        {
            arrow.transform.localScale = new Vector3(0, 0, 0.001f);
        }
    }


    void TogglePhysics(bool physicsOn)
    {
        if (physicsOn)
        {
            grounded = false;
//            gravitationOn = true;
            myBody.gravityScale = 0f;
            myBody.drag = 0f;
            myBody.angularDrag = 0f;
            myBody.mass = mass;
            myBody.WakeUp();
            myCollider2D.enabled = true;
            myBody.velocity = initialVelocity;
            transform.parent = null;
            myTrailRender.time = trailTime;
        }
        else
        {
            grounded = true;
            gravitationOn = false;
            myBody.Sleep();
            myCollider2D.enabled = false;
            myBody.velocity = Vector2.zero;
            NetForce = Vector2.zero;
            StartCoroutine(TurnOffTrail());
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, NetForce * 100);
        Gizmos.color = Color.blue;
        if (gravitationOn)
        {
            Gizmos.DrawRay(transform.position, myBody.velocity.normalized * 5);
        }
        Gizmos.DrawSphere(transform.position, radius: mySpriteRenderer.size.magnitude);
        
    }


    void LandOnPlanet(Transform planetTransform)
    {
        transform.SetParent(planetTransform, worldPositionStays: true);
        transform.SetPositionAndRotation(planetTransform.position,
            Quaternion.identity);
        parentPlanet = planetTransform.GetComponent<Planetoid>();
        parentPlanet.UpdateColors(myColor);
        parentPlanet.mySparks.Play();
        
        parentPlanet.myOutline.enabled = true;
        Debug.Log("Removeing parent" + planetTransform.name + " --> curr len " +
                  attractableBodies.Count);
        attractableBodies.Remove(parentPlanet);
        parentPlanet.GetComponent<Collider2D>().enabled = false;
        Debug.Log("new len " + attractableBodies.Count);
        TogglePhysics(false);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Crashed");
        if (col.CompareTag("planet"))
        {
            LandOnPlanet(planetTransform: col.transform);
        }
        else if (col.CompareTag("star"))
        {
            Debug.Log("I have died!");
            Destroy(gameObject);
        }
    }

    Vector2 GetNetGravitationalForce()
    {
        Vector2 NetForce = new Vector2(0, 0);
        foreach (Planetoid otherBody in attractableBodies)
        {
            if (otherBody == null)
                continue;
            if (otherBody != myBody) // our own, skip 
                NetForce += GravitationalForceFrom(otherBody);
        }

        return NetForce;
    }


    void Rotate()
    {
        float rotationSpeed = 10f;
        Vector3 direction = myBody.velocity.normalized;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        transform.rotation = rotation;
    }

    Vector2 GravitationalForceFrom(Planetoid otherBody)
    {
        float m1 = myBody.mass;
        float m2 = otherBody.mass;
        Vector2 r = transform.position - otherBody.transform.position;
        float magnitude = 0.0f;
        if (r.magnitude < maxDistance)
            magnitude = (m1 * m2) / r.sqrMagnitude;
        return -r.normalized * magnitude;
    }
}