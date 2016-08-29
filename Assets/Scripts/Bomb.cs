using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Bomb : NetworkBehaviour {

	private Transform myTransform;
    public Player owner;

	[SerializeField]
	private GameObject exploPrefab;
    [SerializeField]
    private GameObject brickExploPrefab;

	[SerializeField]
	private GameObject[] bonusPrefabs = new GameObject[3];

	private int _range;
	public int range {
		get { return _range; }
		set { _range = value; }
	}

	private float timeLeft = 3.0f;

	private enum Directions { Up, Right, Down, Left };
	private bool freeUp = true,
				 freeRight = true,
				 freeDown = true,
				 freeLeft = true;

	private enum BonusType { Num = 0, Range, Speed };

	void Awake() {
	
		myTransform = GetComponent<Transform> ();
	}

    void Start() {
        
        //owner.SetColliderBomb();
    }

	void Update() {
	
		timeLeft -= Time.deltaTime;

        if (timeLeft <= 0) {

			Explo ();

            // Object reference not set to this object
			if (GetComponent<NetworkIdentity> ().clientAuthorityOwner.playerControllers [0].gameObject != null) {

				string netID = GetComponent<NetworkIdentity> ().clientAuthorityOwner.playerControllers [0].gameObject.GetComponent<NetworkIdentity> ().netId.ToString ();
				Player player = GameManager.GetPlayer ("Player " + netID);
				if (player != null)
					player.numBomb += 1;
			}

			NetworkServer.Destroy (gameObject);
		}
	}

	void Explo() {

		SpawnExplo (myTransform.position);
		ExploPropag (Directions.Up, ref freeUp);
		ExploPropag (Directions.Right, ref freeRight);
		ExploPropag (Directions.Down, ref freeDown);
		ExploPropag (Directions.Left, ref freeLeft);
	}

	void ExploPropag(Directions direct, ref bool free) {

		Vector3 pos = myTransform.position;
		Vector3 nextPos = pos;

		for (int i = 0; i < _range; i++) {

			// Calcul de la prochaine position
			switch (direct) {

				case Directions.Up:
					nextPos += new Vector3 (0, 0, 2.0f);
					break;
				case Directions.Right:
					nextPos += new Vector3 (2.0f, 0, 0);
					break;
				case Directions.Down:
					nextPos += new Vector3 (0, 0, -2.0f);
					break;
				case Directions.Left:
					nextPos += new Vector3 (-2.0f, 0, 0);
					break;
			}

			Collider[] cols = Physics.OverlapSphere (nextPos, 0.5f);

			foreach (Collider col in cols) {
				if (free) {
				
					switch (col.tag) {
						case "Brick":
							free = false;
							NetworkServer.Destroy (col.gameObject);
							Bonus (col.transform.position);
                            CmdSpawnBrickExplo(col.transform.position);
							break;
						case "Wall":
							free = false;
							break;
						case "Bomb":
							col.GetComponent<Bomb> ().timeLeft = 0;
							break;
						case "Player":
							col.GetComponent<Player> ().Die ();
							break;
					}
				}
			}

			if (free)
				SpawnExplo (nextPos);
		}
		nextPos = pos;
	}

	void Bonus(Vector3 pos) {
	
		float randBonus = Mathf.Round(Random.Range (0, 100.0f) * 10) / 10;

		if (randBonus <= GameSettings.bonusSpawn) {

			randBonus = Mathf.Round(Random.Range (0, 100.0f) * 10) / 10;

			if (randBonus <= GameSettings.numBombSpawn) {

				CmdSpawnBonus (BonusType.Num, pos);

			} else if (randBonus <= (GameSettings.numBombSpawn + GameSettings.speedSpawn)) {

				CmdSpawnBonus (BonusType.Speed, pos);

			} else if (randBonus <= (GameSettings.numBombSpawn + GameSettings.speedSpawn + GameSettings.rangeSpawn)) {

				CmdSpawnBonus (BonusType.Range, pos);
			}
		}
	}

	[Client]
	void SpawnExplo(Vector3 pos) {
	
		CmdSpawnExplo (pos);
	}

	[Command]
	void CmdSpawnExplo(Vector3 pos) {

		GameObject go = Instantiate (exploPrefab);
		go.transform.position = new Vector3 (pos.x, 0, pos.z);
		go.name = exploPrefab.name;
		NetworkServer.Spawn (go);
	}

    [Command]
    void CmdSpawnBrickExplo(Vector3 pos) {
        GameObject go = Instantiate(brickExploPrefab);
        go.transform.position = new Vector3(pos.x, 0, pos.z);
        go.name = brickExploPrefab.name;
        NetworkServer.Spawn(go);
    }

	[Command]
	void CmdSpawnBonus(BonusType bonusType, Vector3 pos) {

		GameObject go = Instantiate (bonusPrefabs[(int)bonusType]);
		go.transform.position = new Vector3 (pos.x, 0, pos.z);
		go.name = bonusPrefabs [(int)bonusType].name;
		NetworkServer.Spawn (go);
	}

	void OnTriggerExit(Collider col) {
	
		if (col.tag == "Player") {
		
			col.GetComponent<Player> ().onBomb = false;
		}
	}
}
