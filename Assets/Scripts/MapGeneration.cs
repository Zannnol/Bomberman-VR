using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MapGeneration : NetworkBehaviour {

	public GameObject wallPrefab;
	public GameObject blockPrefab;
	public GameObject spawnObject;

	[SerializeField] private int xMap = 15;
	[SerializeField] private int zMap = 13;

	private GameObject walls;
	private GameObject blocks;
	private GameObject spawnpoints;
	private float wallSize;

	void Awake() {
		
		wallSize = wallPrefab.transform.lossyScale.x * 2.0f;
		walls = GameObject.Find ("Walls");
		blocks = GameObject.Find ("Blocks");
		spawnpoints = GameObject.Find ("Spawnpoints");
	}
		
	void Start() {

		if (xMap % 2 == 1 && zMap % 2 == 1) {
			
			// Fill the map
			float xFill = 0;
			for (int i = 0; i < xMap; i++) {
			
				float zFill = 0;
				for (int j = 0; j < zMap; j++) {
				
					createWall (xFill, zFill, "Wall", wallPrefab);
					zFill += wallSize;
				}

				xFill += wallSize;
				zFill = 0;
			}
			
			// Transform the map into a grid
			float xGrid = wallSize;
			for (int i = 0; i < xMap - 2; i++) {
			
				float zGrid = wallSize;
				for (int j = 0; j < zMap - 2; j++) {
				
					Collider[] wallCollider = Physics.OverlapSphere (new Vector3 (xGrid, wallSize, zGrid), 0.5f);

					if (wallCollider.Length != 0) {
						
						Destroy (wallCollider [0].gameObject);

						if (xGrid == wallSize && zGrid == wallSize || xGrid == xMap * 2 - 4 && zGrid == zMap * 2 - 4) {
						
							var currentSpawn = Instantiate (spawnObject);
							currentSpawn.transform.position = new Vector3 (xGrid, 1.0f, zGrid);
							currentSpawn.transform.parent = spawnpoints.transform;
							currentSpawn.name = "Spawnpoint";

							if (GameObject.FindGameObjectsWithTag ("Respawn").Length == 2) {
								currentSpawn.transform.eulerAngles += new Vector3 (0, 180.0f, 0);
							}

						} else {
							
							GameObject block = Instantiate (blockPrefab);
							block.transform.position = new Vector3 (xGrid, 1.0f, zGrid);
							block.transform.parent = blocks.transform;
							block.name = "Brick";
						}
					}

					if (i % 2 == 0) {
						zGrid += wallSize;
					} else {
						zGrid += wallSize * 2;
					}
				}

				xGrid += wallSize;
				zGrid = 0;
			}

			// Floor adjustment
			var floor = GameObject.CreatePrimitive (PrimitiveType.Plane);

			floor.transform.localScale = new Vector3 (xMap * 0.2f, 1f, zMap * 0.2f);
			floor.transform.position = new Vector3 (xMap - 1.0f, 0f, zMap - 1.0f);
			floor.transform.parent = transform;
			floor.name = "Floor";

			// Generation of the spawnpoints
			GameObject[] respawn = GameObject.FindGameObjectsWithTag("Respawn");

			// Genererationm of the space for the spawnpoints
			for (int i = 0; i < respawn.Length; i++) {

				foreach (Transform child in respawn[i].transform) {

					Collider[] spawnCollider = Physics.OverlapSphere (child.transform.position, 0.5f);

					for (int j = 0; j < spawnCollider.Length; j++) {

						if (spawnCollider [j].CompareTag ("Brick")) {
							Destroy (spawnCollider [j].gameObject);
						}
					}
				}
			}

		} else {
			
			Debug.LogError ("The values of xMap and zMap must be odd !");
		}
	}

	// Function who create a wall on a specified position
	void createWall(float x, float z, string name, GameObject gameObject) {
		
		var currentWall = Instantiate (gameObject);

		currentWall.transform.position = new Vector3 (x, 1.15f, z);
		currentWall.name = name;
		currentWall.tag = "Wall";
		currentWall.transform.parent = walls.transform;
	}
}
