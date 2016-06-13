using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	private Settings settings;
	private Transform myTransform;

	[SerializeField] private float destroyTimer = 0.4f;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent<Transform> ();
	}

	void Update () {

		foreach (Transform child in myTransform) {

			ParticleSystem exploParticles = child.GetComponent<ParticleSystem> ();

			if (settings.timeStopped) {
			
				exploParticles.Pause ();

			} else if(exploParticles.isPaused) {
			
				exploParticles.Play ();
			} 
		}
			
		if (!settings.timeStopped) {
			
			destroyTimer -= Time.deltaTime;

			if (destroyTimer <= 0) {

				Destroy (gameObject);
			}
		}
	}
}
