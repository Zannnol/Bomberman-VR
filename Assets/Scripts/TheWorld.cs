using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Collections;

public class TheWorld : NetworkBehaviour {

	public GameObject owner;

	private Settings settings;
	private Transform myTransform;
	private float theWorldTimer;
	private bool audioPlaying = false;

	void Awake() {

		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent<Transform> ();
		theWorldTimer = settings.theWorldTimer;
	}

	void Update() {
	
		if (settings.timeStopped) {
		
			theWorldTimer -= Time.deltaTime;

			if (theWorldTimer <= 2.5f) {

				if (!audioPlaying) {
					
					AudioSource.PlayClipAtPoint (settings.timeResumeSound, myTransform.position);
					audioPlaying = true;
				}

				if (theWorldTimer <= 0) {
			
					settings.timeStopped = false;
					Destroy (gameObject);
				}
			}
		}
	}
}
