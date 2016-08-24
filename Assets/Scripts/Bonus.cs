using UnityEngine;

public class Bonus : MonoBehaviour {

	private Transform myTransform;

	[SerializeField]
	private string bonusType = "Unknown";

	void Awake() {
	
		myTransform = GetComponent<Transform> ();
	}

	void Update() {
	
		myTransform.RotateAround(myTransform.position, Vector3.up, 40.0f * Time.deltaTime);
	}

	void Active(Player player) {

		switch (bonusType) {

			case "BonusNum":
				player.numBomb++;
				break;
			case "BonusRange":
				player.range++;
				break;
			case "BonusSpeed":
				player.speed++;
				break;
		}

		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
	
		if (col.tag == "Player")
			Active (col.GetComponent<Player> ());
	}
}
