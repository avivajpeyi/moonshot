using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Object that can be shot
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    private float shootForce;
    private float increase = 10;
    public float maxForce = 20;

    [Tooltip("Max distance rocket will gravitationally be affected by planets")]
    public float maxDistance = 1000;

    public float offset = 0.0f;
    public float mass = 20;
    public Vector2 initialVelocity;

    [SerializeField] private Vector2 NetForce;

    [SerializeField] private bool gravityOn = false;
    private Rigidbody2D myBody;

    //make a static list of bodies so that we don't need to Find them every frame
    public List<Planetoid> attractableBodies = new List<Planetoid>();

    public GameObject arrow;

    private StarSystemSpawner starSystemSpawner;
    

    void Start()
    {
        starSystemSpawner = FindObjectOfType<StarSystemSpawner>();
        myBody = GetComponent<Rigidbody2D>();
        myBody.Sleep();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gravityOn)
        {
            attractableBodies = starSystemSpawner.allBodies;
            arrow.transform.localScale = new Vector3(shootForce, shootForce, 1);
            if (Input.GetMouseButton(0))
            {
                shootForce += increase * Time.deltaTime;
                if (shootForce >= maxForce)
                    shootForce = maxForce;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Shoot();
                shootForce = 0;
            }
        }
        
        if (gravityOn)
        {
            NetForce = GetNetGravitationalForce();
            myBody.AddForce(NetForce);
            Rotate();
        }
    }
    
    

    void Shoot()
    {
        Debug.Log("Shoot");
        gravityOn = true;
        initialVelocity = shootForce * transform.up;
        Debug.Log(initialVelocity);
        SetupRigidbody2D();
        arrow.SetActive(false);
    }


    void SetupRigidbody2D()
    {
        myBody.gravityScale = 0f;
        myBody.drag = 0f;
        myBody.angularDrag = 0f;
        myBody.mass = mass;
        myBody.WakeUp();
        myBody.velocity = initialVelocity;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, NetForce * 100);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, myBody.velocity.normalized * 5);
        Vector2 myPos = transform.position;
        Vector2 myVel = myBody.velocity.normalized * 5 + myPos;
        Gizmos.DrawSphere(  myVel , 0.5f);
        
    }


    // called when the cube hits the floor
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Rocket Collided");
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Rocket Collided");
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