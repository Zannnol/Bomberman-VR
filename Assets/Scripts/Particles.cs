using UnityEngine;
using System.Collections;

public class Particles : MonoBehaviour {

	private Settings settings;

	private float destroyTimer = 1.0f;

	void Awake() {
	
		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
	}
	
	void Update () {

		ParticleSystem bonusParticles = GetComponent<ParticleSystem> ();
	
		if (settings.timeStopped) {
		
			bonusParticles.Pause ();

		} else {
		
			destroyTimer -= Time.deltaTime;

			if (bonusParticles.isPaused) {
			
				bonusParticles.Play ();
			}
		}

		if (destroyTimer <= 0) {

			Destroy (gameObject);
		}
	}
}
