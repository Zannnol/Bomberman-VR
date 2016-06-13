using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	[Header("Players")]
	public GameObject[] playerHUD;

	[Header("Prefabs")]
	public GameObject bombPrefab;
	public GameObject exploPrefab;
	public GameObject theWorldPrefab;
	public GameObject[] bonusPrefabs;

	[Header("Sounds")]
	public AudioClip theWorldSound;
	public AudioClip timeResumeSound;

	[Header("Game")]
	public bool gamePaused = true;
	public bool timeStopped = false;
	public bool nightMode = false;
	[Space(10)]

	public float gameTimer = 180.0f;
	public float theWorldTimer = 9.0f;
	[Space(10)]

	[Range(0, 100.0f)]
	public float powerProbability = 100.0f;

	[Range(0, 100.0f)]
	public float numProbability = 0;

	[Range(0, 100.0f)]
	public float speedProbability = 0;

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
