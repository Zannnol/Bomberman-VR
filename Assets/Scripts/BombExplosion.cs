using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Collections;

public class BombExplosion : NetworkBehaviour {

	private Settings settings;
	private Transform myTransform;

	[HideInInspector] public PlayerMovements playerMove;
	[HideInInspector] public int bombPower;

	// Explosions parameters
	public AudioClip exploSound;
	public float exploTimer = 3.0f;
	public float exploDispa = 0.3f;

	// Bombs pushing
	private Vector3 oldPos = Vector3.zero;
	private Vector3 nextPos = Vector3.zero;
	[HideInInspector] public bool pushing = false;

	// Explosions directions
	private enum Directions { Up, Right, Down, Left };
	private bool up = true;
	private bool right = true;
	private bool down = true;
	private bool left = true;

	private Vector3 midAxe = Vector3.zero;

	void Awake () {

		// Set the variables
		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent <Transform> ();
	}
		
	void Update () {

		ParticleSystem sparks = myTransform.FindChild ("Sparks").GetComponent<ParticleSystem> ();

		if (settings.timeStopped) {
			
			sparks.Pause ();

		} else {
			
			exploTimer -= Time.deltaTime;

			if (sparks.isPaused) {
			
				sparks.Play ();
			}
		}

		// Check if the timer is over
		if (exploTimer <= 0) {

			exploTimer = 10.0f;

			GameObject exploParent = new GameObject ();
			exploParent.AddComponent<NetworkIdentity> ().localPlayerAuthority = true;
			ClientScene.RegisterPrefab (exploParent);
			exploParent.name = "Explosions";
			Destroy (exploParent, exploDispa);

			// Center of the explosion
			if (playerMove) {
				
				var position = settings.RoundPosition(myTransform.position + new Vector3 (0, 0.6f, 0));
				playerMove.CmdSpawnExplo (position.x, position.z, exploParent);
			}

			// Propagation of the explosion
			ExploPropag (ref up, Directions.Up, exploParent);
			ExploPropag (ref right, Directions.Right, exploParent);
			ExploPropag (ref down, Directions.Down, exploParent);
			ExploPropag (ref left, Directions.Left, exploParent);

			if (playerMove) {
				
				playerMove.RpcIncrem ();
			}

			// Play the explosion sound
			AudioSource.PlayClipAtPoint (exploSound, myTransform.position);

			// Destruction of the bomb
			Destroy (gameObject);
		}

		// Check if the bomb is pushed
		if (pushing && !settings.timeStopped) {

			// Make the animation of the push
			myTransform.position = Vector3.MoveTowards (myTransform.position, nextPos, Time.deltaTime * 10.0f);

			// If bomb pushing animation finished
			if (myTransform.position == nextPos) {

				pushing = false;
				CmdPush (Vector3.zero);
				Push (Vector3.zero);
			}
		}

		if (midAxe != Vector3.zero) {

			myTransform.RotateAround (midAxe, Vector3.left, 100.0f * Time.deltaTime);
		}
	}

	/// <summary>
	/// Push the bomb into a direction.
	/// </summary>
	/// <param name="parentPos">Position of the pusher.</param>
	[Command]
	public void CmdPush(Vector3 parentPos) {

		Push (parentPos);
	}

	public void Push(Vector3 parentPos) {
	
		Vector3 pos = Vector3.zero;

		if (parentPos != Vector3.zero) {
			pos = settings.RoundPosition (parentPos - myTransform.position);
		} else {
			Vector3 tempPos = nextPos - oldPos;
			pos = new Vector3 (-tempPos.x, 0, -tempPos.z);
		}

		oldPos = myTransform.position;
		nextPos = myTransform.position - new Vector3 (pos.x, -0.6f, pos.z);
		Collider[] cols = Physics.OverlapSphere (nextPos, 0.5f);
		nextPos -= new Vector3 (0, 0.6f, 0);

		if (!pushing) {
			if (cols.Length == 0) {

				pushing = true;

			} else {

				foreach (Collider col in cols) {

					if (col.CompareTag ("Explosion")) {

						exploTimer = 0;
					}
				}
			}
		}
	}

	public void Throw(Vector3 callPos) {
		
		Vector3 nextPos = settings.RoundPosition (callPos + new Vector3 (0, 0, -4.0f));
		float mid = (Vector3.Distance (callPos, nextPos) / 2.0f);
		midAxe = callPos - new Vector3(0, 0, mid);
	}

	/// <summary>
	/// Spawn the explosions into a direction.
	/// </summary>
	/// <param name="isFree">Check if the position is free.</param>
	/// <param name="direct">The direction of the explosions.</param>
	/// <param name="parent">GameObject who posed the bomb.</param>
	void ExploPropag(ref bool isFree, Directions direct, GameObject parent) {

		float xPropag = 0;
		float zPropag = 0;

		// Repeat according to the power of the explosion
		for (int i = 0; i < bombPower; i++) {

			// Check the direction
			switch (direct) {

				case Directions.Up:
					zPropag += 2.0f;
					break;
				case Directions.Right:
					xPropag += 2.0f;
					break;
				case Directions.Down:
					zPropag -= 2.0f;
					break;
				case Directions.Left:
					xPropag-= 2.0f;
					break;
			}

			// Get the explosion position and get what's on it
			Vector3 position = settings.RoundPosition(myTransform.position + new Vector3 (xPropag, 0.25f, zPropag));
			Collider[] collider = Physics.OverlapSphere (position, 0.5f);

			for (int j = 0; j < collider.Length; j++) {
					
				// Check what's on the position
				if (collider [j].CompareTag ("Wall")) {
			
					isFree = false;

				} else if (collider [j].CompareTag ("Brick") && isFree) {

					isFree = false;
					
					if ((int)Random.Range (0, 100.999f) > 70) {

						// Spawn a bonus
						playerMove.GetComponent<PlayerMovements> ().CmdSpawnBonus (position);
					}

					// Destroy the block
					Destroy (collider[j].gameObject);

				} else if(collider[j].CompareTag("Bonus") && isFree) {

					// Destroy the bonus
					Destroy (collider [j].gameObject);

				} else if (collider [j].CompareTag ("Bomb") && isFree) {

					// Detonated another bomb
					collider [j].GetComponent<BombExplosion> ().exploTimer = 0;
				}
			}	

			// Check if the explosion can spawn
			if (isFree) {
				
				if (playerMove) {

					// Spawn an explosion
					playerMove.CmdSpawnExplo (position.x, position.z, parent);
				}
			}
		}
	}
}