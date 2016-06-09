using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Bonus : NetworkBehaviour {

	public GameObject BonusParticles;

	private Settings settings;
	private Transform myTransform;
	private Text[] playerStatsTexts;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent<Transform> ();
		playerStatsTexts = settings.playerHUD [0].GetComponentsInChildren<Text> ();
	}

	void Update () {

		if(!settings.timeStopped)
			myTransform.RotateAround(transform.position, Vector3.up, 40.0f * Time.deltaTime);
	}

	/// <summary>
	/// Called if an object enter the trigger of the bonus.
	/// </summary>
	/// <param name="collider">The GameObject who enter the trigger.</param>
	void OnTriggerEnter(Collider collider) {

		// Check if the object who entered the trigger is a player
		if (collider.CompareTag ("Player")) {

			// Choose and increase the corresponding bonus
			switch (name) {

				case "BonusPower":
					collider.GetComponent<PlayerMovements> ().bombPower++;
					playerStatsTexts[0].text = collider.GetComponent<PlayerMovements> ().bombPower.ToString();
					break;

				case "BonusNum":
					collider.GetComponent<PlayerMovements> ().bombNum++;
					playerStatsTexts[1].text = collider.GetComponent<PlayerMovements> ().bombNum.ToString();
					break;

				case "BonusSpeed":
					collider.GetComponent<PlayerMovements> ().speed += 1.0f;
					playerStatsTexts[2].text = (collider.GetComponent<PlayerMovements> ().speed - 7).ToString();
					break;
			}

			// Create the particles
			GameObject particles = Instantiate (BonusParticles);
			particles.transform.position = transform.position;
			particles.name = gameObject.name + "Particules";

			// Destroy itself
			Destroy (gameObject);
		}
	}
}
