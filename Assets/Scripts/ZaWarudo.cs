using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class ZaWarudo : MonoBehaviour {

	public AudioClip resumeSound;
	public float destroyTime = 9.0f;

	private Settings settings;
	private Transform myTransform;
	private bool test = false;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent<Transform> ();
	}

	void Update() {
	
		if (settings.timeStopped) {
		
			destroyTime -= Time.deltaTime;

			if (destroyTime <= 2.5f) {

				if (!test) {
					
					AudioSource.PlayClipAtPoint (resumeSound, myTransform.position);
					test = true;
				}

				if (destroyTime <= 0) {
			
					settings.timeStopped = false;
					Destroy (gameObject);
				}
			}
		}
	}

	void OnTriggerEnter(Collider collider) {

		if (collider.CompareTag ("Player")) {

			settings.timeStopped = true;
		}
	}
}
