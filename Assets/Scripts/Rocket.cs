using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Object that can be shot
/// </summary>
public class Rocket : MonoBehaviour
{
    private float force;
    private float increase = 10;
    private float maxForce = 20;
    
    public GameObject arrow;


    void Start()
    {
        GetComponent<GravitationalBody>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        arrow.transform.localScale = new Vector3(force, force, 1);
        if (Input.GetMouseButton (0)) {
            force += increase*Time.deltaTime;
            if (force >= maxForce)
                force = maxForce;
        }
 
        if (Input.GetMouseButtonUp (0)) {
            Shoot();
            force = 0; //reset the force
//            Destroy(this);
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot");
//        GetComponent<GravitationalBody>().enabled = true;
//        GetComponent<Rigidbody2D>()
//            .AddForce(transform.up * force , ForceMode2D.Impulse);

    }
}
