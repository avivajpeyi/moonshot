using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarSystemSpawner : MonoBehaviour
{
    public GameObject planetPrefab;
    public int numBodies;

    private float dist = 0;
    public List<Planetoid> allBodies = new List<Planetoid>();

    public List<Planetoid> CreateStarSystem()
    {
        List<Planetoid> planetoids = new List<Planetoid>();

        Vector3 starPosition = new Vector3(
            Random.Range(-dist, dist),
            Random.Range(-dist, dist),
            0f
        );

        var starGo = Instantiate(planetPrefab, starPosition, Quaternion.identity);
        starGo.name = "Star";
        Planetoid starPlanetoid = starGo.GetComponent<Planetoid>();

        float starSize = Random.Range(dist * 0.2f, dist) / 15f + 0.5f;
        starPlanetoid.InitializePlanetoid(
            centerOfGravity: starGo.transform,
            orbitSpeed: 0,
            orbitRadius: 0,
            isRotatingClockwise: false,
            size: starSize
        );

        for (int i = 0; i < 1; i++)
        {
            float planetSize = Random.Range(starSize * 0.5f, starSize * 0.9f);
            GameObject planetGo = Instantiate(planetPrefab, starGo.transform);
            planetGo.name = "Planet";
            Planetoid planetoid = planetGo.GetComponent<Planetoid>();
            planetoid.InitializePlanetoid(
                centerOfGravity: starGo.transform,
                orbitSpeed:1,
                orbitRadius:(planetSize+starSize) * (2 + i),
                isRotatingClockwise:(Random.Range(0f, 1f) < 0.3f),
                size: planetSize
            );
            planetoids.Add(planetoid);
        }

        planetoids.Add(starPlanetoid);


        return planetoids;
    }


    void Start()
    {
        dist = Camera.main.orthographicSize;
        for (var i = 0; i < numBodies; i++)
        {
            List<Planetoid> planetoids = CreateStarSystem();
            allBodies = allBodies.Concat(planetoids).ToList();
        }
    }
}