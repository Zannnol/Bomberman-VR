using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerShoot : NetworkBehaviour {

	private Player player;
	private Transform myTransform;

	[SerializeField]
	private GameObject bombPrefab;

	void Awake() {
	
		player = GetComponent<Player> ();
		myTransform = GetComponent<Transform> ();
	}

    void Start() {

        if(!GameManager.hasGameStarted)
            Pause();
    }

	void Update() {

        if(GameManager.hasGameStarted && GameManager.isGamePaused)
            Pause();


		if (Input.GetButton ("Fire1") && !player.onBomb && player.numBomb > 0)
			Shoot ();

        if(Input.GetButton("Fire2"))
            GameManager.hasGameStarted = true;
	}

	[Client]
	void Shoot () {
	
		player.onBomb = true;
		CmdAddNumBomb (player.name, -1);
        CmdSpawnBomb (myTransform.position, player.range);
	}

	[Command]
	void CmdSpawnBomb (Vector3 pos, int power) {
	
		GameObject go = Instantiate (bombPrefab);
		go.transform.position = GameManager.RoundPosition(new Vector3 (pos.x, -0.5f, pos.z));
		go.GetComponent<Bomb> ().range = power;
		go.name = bombPrefab.name;
        go.GetComponent<Bomb>().owner = player;
		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
	}

	[Command]
	void CmdAddNumBomb(string playerName, int howMuch) {

		GameManager.GetPlayer(playerName).numBomb += howMuch;
	}

    void Pause() {

        if(Time.timeScale == 1) {
            Time.timeScale = 0;
            GameManager.isGamePaused = true;
        } else {
            Time.timeScale = 1;
            GameManager.isGamePaused = false;
        }
    }
}
