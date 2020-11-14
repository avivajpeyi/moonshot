using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravitationalBodyController : MonoBehaviour {

	public GameObject planetPrefab;
	public int numBodies;
	

	public GravitationalBody CreatePlanet() {

		float dist = Camera.main.orthographicSize;

		Vector3 randomPosition = new Vector3 (Random.Range (-dist, dist), Random.Range (-dist, dist), 0f);

		var planetGO = Instantiate (planetPrefab, randomPosition, Quaternion.identity) as GameObject;
		planetGO.name = "Planet";

		var gravitationalBody = planetGO.GetComponent<GravitationalBody> ();

		float maxDistance = Random.Range (50f, 400f);
		gravitationalBody.maxDistance = maxDistance + 500f;

		//For shits and giggles we'll make the mass and scale relative to the max distance too. So larger bodies have more 
		//gravitation influence.
		float size = maxDistance / 15f + 0.5f;
		float mass = Mathf.Pow (maxDistance, 2f) + 10f;
		gravitationalBody.transform.localScale = new Vector3 (size, size, size);
		gravitationalBody.startingMass = mass;

		//We'll also have it moving in a random direction, so that all of the don't clump up immediatly
		Vector3 randomInitialVelocity = new Vector2(Random.Range (-1f,1f), Random.Range (-1f,1f)).normalized * 15f;
		gravitationalBody.initialVelocity = randomInitialVelocity;

		return gravitationalBody;
	}

	List<GravitationalBody> allBodies = new List<GravitationalBody> ();
	void Start() {

		for (var i = 0; i < numBodies; i ++) {
			allBodies.Add (CreatePlanet ());
		}

	}

	void FixedUpdate() {
		KeepAllBodiesInView ();
	}

	void KeepAllBodiesInView() {

		Vector2 lowestPosition = Vector2.zero;
		Vector2 highestPosition = Vector2.zero;

		bool first_body = true;

		foreach (var body in allBodies) {
			var pos = body.GetComponent<Rigidbody2D>().position;

			if (first_body) {
				lowestPosition = pos;
				highestPosition = pos;
				first_body = false;

			} else {

				if (pos.x < lowestPosition.x)
					lowestPosition.x = pos.x;

				if (pos.y < lowestPosition.y)
					lowestPosition.y = pos.y;
			
				if (pos.x > highestPosition.x)
					highestPosition.x = pos.x;
				
				if (pos.y > highestPosition.y)
					highestPosition.y = pos.y;
			}

		}
		
		float size = Mathf.Max (highestPosition.y - lowestPosition.y, highestPosition.x - lowestPosition.x) * 0.5f;
		size += 10f;

		Vector3 center = lowestPosition + (highestPosition - lowestPosition) * 0.5f;
		center -= Vector3.forward * 20f;

		var camera = Camera.main;

		camera.transform.position = Vector3.Lerp (camera.transform.position, center, Time.deltaTime);
		camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, size, Time.deltaTime);




	}
}
