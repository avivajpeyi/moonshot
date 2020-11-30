using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OptionsContainer : MonoBehaviour
{

    public bool DebuggingMode = false;
    public int numberPlanets = 1;
    public int depth = 2;
    public int numberStars = 3;
    
    
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("dont_destroy");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}