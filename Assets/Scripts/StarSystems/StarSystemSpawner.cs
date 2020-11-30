using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class StarSystemSpawner : MonoBehaviour
{
    public GameObject planetPrefab;
    public int numStar;
    public int depth = 5;

    public Planetoid start;
    public Planetoid end;
    private float dist = 0;
    public List<Planetoid> allBodies = new List<Planetoid>();

    private KeepAllBodiesinView cameraTracker;

    

    List<List<Planetoid>> PlanetoidLayerList = new List<List<Planetoid>>();

    public Planetoid CreateStarSystem()
    {
        // Set up a "Star System" Game object at a random position on screen
        GameObject starSystemGO = new GameObject("StarSystem");
        float starSize = Random.Range(dist * 0.2f, dist) / 5.0f + 0.5f;
        Vector3 newPos = GetNewPos(starSize);


        starSystemGO.transform.position = newPos;


        // Instantiate system's "star" 
        GameObject starGo = Instantiate(
            original: planetPrefab,
            parent: starSystemGO.transform
        );
        starGo.name = "Star";
        starGo.tag = "star";
        Planetoid starPlanetoid = starGo.GetComponent<Planetoid>();

        starPlanetoid.InitializePlanetoid(
            centerOfGravity: starGo.transform,
            orbitSpeed: 0,
            orbitRadius: 0,
            isRotatingClockwise: false,
            size: starSize
        );

        return starPlanetoid;
    }

    Vector3 GetNewPos(float starSize)
    {
        Vector3 newPos = new Vector3();
        bool newPointFound = false;
        newPos = new Vector3(
            Random.Range(-dist, dist),
            Random.Range(-dist, dist),
            0f
        );
        Collider2D overlapcol = Physics2D.OverlapCircle(newPos, starSize);
        while (overlapcol != null)
        {
            newPos = new Vector3(
                Random.Range(-dist, dist),
                Random.Range(-dist, dist),
                0f
            );
            overlapcol = Physics2D.OverlapCircle(newPos, starSize);
        }

        return newPos;
    }

    IEnumerator SetCamera()
    {
        cameraTracker = FindObjectOfType<KeepAllBodiesinView>();
        List<GameObject> planetGOs = new List<GameObject>();
        foreach (var p in allBodies)
        {
            planetGOs.Add(p.gameObject);
        }
        cameraTracker.allBodies = planetGOs;
        yield return new WaitForSeconds(1.0f);
        cameraTracker.keepUpdating = false;
    }


    void Start()
    {
        dist = Camera.main.orthographicSize;
        InitaliseOrbits();
        if (PlanetoidLayerList.Count > 1)
        {
            SetStartAndEnd();
        }
        StartCoroutine(SetCamera());
    }

    void SetStartAndEnd()
    {
        List<Planetoid> Planets = new List<Planetoid>();
        for (int i = 1; i < PlanetoidLayerList.Count; i++)
        {
            Planets = Planets.Concat(PlanetoidLayerList[i]).ToList();
        }

        start = Planets[0];
        start.name = "START";

        List<float> distances = new List<float>();
        foreach (var p in Planets)
        {
            Vector3 s = start.transform.position - p.transform.position;
            distances.Add(s.magnitude);
        }

        int furthestPlanet = distances.IndexOf(distances.Max());

        if (furthestPlanet != 0)
        {
            end = Planets[furthestPlanet];
            end.name = "END";
            end = Planets[furthestPlanet];
            Planetoid endPlanet = end.GetComponent<Planetoid>();
            endPlanet.myOutline.enabled = true;
        }
    }

    void InitaliseOrbits()
    {
        for (int i = 0; i < depth; i++)
        {
            List<Planetoid> newList = new List<Planetoid>();
            PlanetoidLayerList.Add(newList);
        }


        for (int l = 0; l < depth; l++)
        {
            if (l == 0)
            {
                for (var i = 0; i < numStar; i++)
                {
                    Planetoid starPlanetoid = CreateStarSystem();
                    PlanetoidLayerList[l].Add(starPlanetoid);
                }
            }
            else
            {
                foreach (var parent in PlanetoidLayerList[l - 1])
                {
                    List<Planetoid> parentsMoons = AddPlanets(parent, 1, l);

                    PlanetoidLayerList[l] =
                        PlanetoidLayerList[l].Concat(parentsMoons).ToList();
                }
            }
            allBodies = allBodies.Concat(PlanetoidLayerList[l]).ToList();
        }
    }


    List<Planetoid> AddPlanets(Planetoid parent, int numPlanets, int depth)
    {
        List<Planetoid> planetoids = new List<Planetoid>();

        for (int i = 0; i < numPlanets; i++)
        {
            float planetSize = Random.Range(parent.size * 0.5f, parent.size * 0.8f);
            GameObject planetGo = Instantiate(planetPrefab, parent.transform);
            planetGo.transform.parent = parent.transform.root;
            planetGo.name = "Planet (d" + depth + ")";
            planetGo.tag = "planet";

            Planetoid planetoid = planetGo.GetComponent<Planetoid>();
            planetoid.InitializePlanetoid(
                centerOfGravity: parent.transform,
                orbitSpeed: Random.Range(0.1f, 1f),
                orbitRadius: (planetSize + parent.size) * (2 + i) / depth,
                isRotatingClockwise: (Random.Range(0f, 1f) < 0.3f),
                size: planetSize
            );
            planetoids.Add(planetoid);
        }

        return planetoids;
    }
}