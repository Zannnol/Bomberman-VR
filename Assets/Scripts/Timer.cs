using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

	private Settings settings;
	private float timer;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		timer = settings.gameTimer;
	}

	void Update () {

		SetTimer ();

		if (settings.gamePaused)
			return;

		// Check if the timer is over
		if (timer < 0) {
		
			GetComponentInChildren<Text> ().text = "Cyka";

		} else {

			timer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Format and set the timer on the HUD
	/// </summary>
	void SetTimer() {

		int min = Mathf.FloorToInt (timer / 60.0f);
		int sec = Mathf.FloorToInt (timer - min * 60);
		string time = string.Format ("{0:00}:{1:00}", min, sec);

		GetComponentInChildren<Text> ().text = time;
	}
}
