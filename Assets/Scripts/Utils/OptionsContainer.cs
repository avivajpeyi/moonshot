using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OptionsContainer : MonoBehaviour
{

    public bool DebuggingMode = false;
    public bool linearGravity = true;
    public float gravityFactor = 1.5f;
    

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