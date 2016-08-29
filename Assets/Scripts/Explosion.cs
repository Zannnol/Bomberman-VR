using UnityEngine;
using UnityEngine.Networking;

public class Explosion : NetworkBehaviour {

	private ParticleSystem particles;

	private float timeLeft = 0.4f;

	void Awake() {
	
		particles = GetComponent<ParticleSystem> ();
	}

	void Update() {
	
		if (!GameManager.isTimeStopped) {
			
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0)
				NetworkServer.Destroy (gameObject);
			else if (particles.isPaused)
				particles.Play ();
			
		} else {
		
			particles.Pause ();
		}
	}
}
