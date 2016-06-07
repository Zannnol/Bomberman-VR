using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wait : MonoBehaviour {

	private Settings settings;
	private Text waitText;
	private int numDots = 0;
	private string defaultString;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		waitText = GetComponentInChildren<Text> ();
		defaultString = waitText.text;
	}

	void Start() {

		InvokeRepeating ("Waiting", 1, 1.0f);
	}

	void Update() {

		if (!settings.gamePaused)
			gameObject.SetActive (false);
	}

	/// <summary>
	/// Make an animation with the waiting screen dots
	/// </summary>
	void Waiting() {

		if (!settings.gamePaused)
			return;

		if (numDots < 3) {

			waitText.text += ".";
			numDots++;

		} else {

			waitText.text = defaultString;
			numDots = 0;
		}
	}
}
