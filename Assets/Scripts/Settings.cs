using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	[Header("Players")]
	public GameObject[] playerHUD;

	[Header("Prefabs")]
	public GameObject bombPrefab;
	public GameObject zaWaroduPrefab;
	public GameObject exploPrefab;
	public GameObject[] bonusPrefabs;

	[Header("Game")]
	public bool gamePaused = true;
	public bool timeStopped = false;
	public bool nightMode = false;
	public float gameTimer = 180.0f;

	void Awake() {

		if (nightMode) {
		
			GameObject.Find ("Sun").SetActive (false);
		}
	}

	void Update() {
	
		if (gamePaused)
			return;

		foreach (GameObject hud in playerHUD) {

			hud.SetActive (true);
		}
	}

	/// <summary>
	/// Round a position.
	/// </summary>
	/// <returns>The rounded position.</returns>
	/// <param name="pos">The position to round.</param>
	public Vector3 RoundPosition(Vector3 pos) {

		// Round the coordinates x and z
		float x = Mathf.Round (pos.x / 2.0f) * 2.0f;
		float z = Mathf.Round (pos.z / 2.0f) * 2.0f;

		Vector3 newPos = new Vector3 (x, pos.y, z);

		return newPos;
	}
}
