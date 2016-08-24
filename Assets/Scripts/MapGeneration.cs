using UnityEngine;

public class MapGeneration : MonoBehaviour {

	[SerializeField]
	private GameObject wallPrefab, brickPrefab, spawnPrefab;
	[SerializeField]
	private int xMap = 15, zMap = 13;

	private Transform myTransform;

	void Awake() {

		myTransform = GetComponent<Transform> ();
	}

	void Start() {

		// Creation of the parent for the walls
		GameObject walls = new GameObject ();
		walls.name = "Walls";
		walls.isStatic = true;
		walls.transform.parent = myTransform;

		// Creation of the parent for the bricks
		GameObject bricks = new GameObject ();
		bricks.name = "Bricks";
		bricks.isStatic = true;
		bricks.transform.parent = myTransform;

		Vector3 blockPos = new Vector3 (0, 0, 0);

		// Placement of the walls
		for (int j = 0; j < zMap; j++) {

			if (j % 2 == 0) {

				for (int i = 0; i < xMap; i++) {

					if (i % 2 == 0 || j == 0 || j == zMap - 1) {
						
						PlaceObject (wallPrefab, blockPos + new Vector3(0, 0.1f, 0), walls);

					} else {
					
						PlaceObject (brickPrefab, blockPos, bricks);
					}
					blockPos += new Vector3 (2, 0, 0);
				}

			} else {

				PlaceObject (wallPrefab, blockPos + new Vector3(0, 0.1f, 0), walls);
				PlaceObject (wallPrefab, blockPos + new Vector3 (xMap * 2 - 2, 0.1f, 0), walls);

				for (int i = 0; i < xMap - 2; i++) {

					blockPos += new Vector3 (2, 0, 0);
					PlaceObject (brickPrefab, blockPos, bricks);
				}
			}
			blockPos += new Vector3 (0, 0, 2);
			blockPos.x = 0;
		}

		// Creation of the parent for the spawnpoints
		GameObject spawnpoints = new GameObject ();
		spawnpoints.name = "Spawnpoints";
		spawnpoints.isStatic = true;
		spawnpoints.transform.parent = myTransform;

		// Placement of the spawnpoints
		PlaceObject (spawnPrefab, new Vector3 (2, 0, 2), spawnpoints);
		PlaceObject (spawnPrefab, new Vector3 (xMap * 2 - 4, 0, zMap * 2 - 4), spawnpoints);

		// Free the space for the spawnpoints
		foreach(Transform spawnpoint in spawnpoints.transform) {

			foreach(Transform child in spawnpoint.transform) {

				Collider[] colliders = Physics.OverlapSphere (child.transform.position, 0.5f);

				if (colliders.Length > 0) {
					if (colliders [0].tag == "Brick") {
						Destroy (colliders [0].gameObject);
					}
				}
			}
		}
	}

	/// <summary>
	/// Place an object on a position.
	/// </summary>
	/// <param name="go">The object to spawn.</param>
	/// <param name="pos">The position where the object will be spawned.</param>
	/// <param name="parent">The parent of the object.</param>
	void PlaceObject(GameObject go, Vector3 pos, GameObject parent = default(GameObject)) {

		Collider[] colliders = Physics.OverlapSphere(pos, 0.75f);

		if (colliders.Length > 0) {
			Destroy (colliders [0].gameObject);
		}

		GameObject block = (GameObject)Instantiate (go, pos, Quaternion.identity);
		block.name = go.name;

		if (parent == default(GameObject)) {

			parent = gameObject;
		}
		
		block.transform.parent = parent.transform;
	}
}
