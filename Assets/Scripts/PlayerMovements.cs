using UnityEngine;
using WiimoteApi;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovements : NetworkBehaviour {

	[SyncVar] public int bombNum = 1;
	[SyncVar] public int bombPower = 1;
	[SyncVar] public float speed = 7.0f;
	[SyncVar] public bool bombPush = false;

	private Transform myTransform;
	private CharacterController controller;
	private Settings settings;
	private GameObject playerStats;

	private BombExplosion bombBE;

	private bool onBomb = false;
	private bool zaWarodu = true;

    //Wiimote
    private WiimoteApi.Wiimote wiimote;
    private NunchuckData nunchuck;
    private int wiimoteData;
    private bool isWiimote = false;

	[Command]
	public void CmdSpawn(GameObject obj) {

		NetworkServer.Spawn(obj);
	}

	[Command]
	void CmdSpawnBomb(Vector3 pos) {

		GameObject bomb = Instantiate (settings.bombPrefab);
		bomb.transform.position = new Vector3 (pos.x, 0.7f, pos.z);
		bomb.GetComponent<BombExplosion> ().playerMove = GetComponent<PlayerMovements> ();
		bomb.GetComponent<BombExplosion> ().bombPower = bombPower;
		bomb.name = "Bomb";

		//var bombCollider = bomb.GetComponent<BoxCollider> ();

		NetworkServer.Spawn (bomb);

		// Server who called
		/*
		if (localPlayerAuthority) {

			print ("Called by server");
			bombCollider.isTrigger = false;
			NetworkServer.Spawn (bomb);
			bombCollider.isTrigger = true;
			// Trigger true only on server
		
		// Client who called
		} else {
		
			print ("Called by client");
			NetworkServer.Spawn (bomb);
			bombCollider.isTrigger = false;
		}*/
	}

	[Command]
	public void CmdSpawnExplo(Vector3 pos, GameObject parent) {

		GameObject explo = Instantiate (settings.exploPrefab);
		explo.transform.position = pos;
		explo.transform.parent = parent.transform;
		explo.name = "Explosion";

		NetworkServer.Spawn(explo);
	}

	[Command]
	public void CmdSpawnBonus(int bonusId, Vector3 pos) {
	
		GameObject bonus = Instantiate (settings.bonusPrefabs[bonusId]);
		bonus.transform.position = new Vector3 (pos.x, 1.0f, pos.z);
		bonus.transform.eulerAngles += new Vector3 (15.0f, 0, 0);
		bonus.name = settings.bonusPrefabs[bonusId].name;

		NetworkServer.Spawn (bonus);
	}

	[Command]
	void CmdSpawnTheWorld() {
	
		GameObject theWorld = Instantiate (settings.theWorldPrefab);
		theWorld.transform.position = myTransform.position;
		theWorld.GetComponent<TheWorld> ().owner = gameObject;
		theWorld.name = "The World";

		NetworkServer.Spawn (theWorld);
	}

	/// <summary>
	/// Make the server pause the game.
	/// </summary>
	[Command]
	void CmdPause() {
	
		settings.gamePaused = false;
	}

	/// <summary>
	/// Increment the bomb number.
	/// </summary>
	[ClientRpc]
	public void RpcIncrem() {

		if (!isLocalPlayer)
			return;
			
		bombNum++;
	}

	void Awake() {

		// Set the variables
		settings = GameObject.Find ("GameSettings").GetComponent<Settings> ();
		myTransform = GetComponent<Transform> ();
		controller = GetComponent<CharacterController> ();
		playerStats = settings.playerHUD [0];

		// Set the HUD
		Text[] playerStatsTexts = playerStats.GetComponentsInChildren<Text> ();
		playerStatsTexts[0].text = bombPower.ToString();
		playerStatsTexts[1].text = bombNum.ToString();
		playerStatsTexts[2].text = (speed - 7).ToString();
	}

	void Start() {

		// Resume the game if the gameObject is a client
		if (!isServer) {

			settings.gamePaused = false;
			CmdPause ();
		}

		if (!isLocalPlayer)
			return;

		gameObject.layer = 0;
		myTransform.FindChild ("Bomberman").gameObject.layer = 0;

		// Correct the minimap rotation
		settings.playerHUD[1].transform.eulerAngles = new Vector3 (90.0f, 180.0f + myTransform.eulerAngles.y, 0);

		// Create the camera
		var cam = new GameObject ();
		cam.AddComponent<Camera> ();
		cam.AddComponent<FlareLayer> ();
		cam.AddComponent<AudioListener> ();
		cam.transform.position = myTransform.position + new Vector3 (0, 1.5f, 0);
		cam.transform.eulerAngles = myTransform.eulerAngles + new Vector3(0, 180.0f, 0);
		cam.transform.parent = myTransform;
		cam.name = "Head";

		// Check if the night mode is activated
		if (settings.nightMode)
			myTransform.FindChild("Lamp").gameObject.SetActive (true);

		//Initialise Wiimotes
		//InitWiimotes();
	}

	void Update() {

		Animator playerAnimator = GetComponent<Animator> ();

		if (settings.timeStopped) {
			
			playerAnimator.speed = 0;

		} else if (playerAnimator.speed == 0) {
		
			playerAnimator.speed = 1.0f;
		}
		
		if (!isLocalPlayer || settings.gamePaused || settings.timeStopped)
			return;

		Collider[] maps = Physics.OverlapSphere (myTransform.position, 3.0f);

		foreach (Collider map in maps) {
		
			if (map.gameObject.layer == 9)
				map.gameObject.layer = 0;
		}

		Move ();
		Look ();

		if (Input.GetAxis ("Fire1") == 1 && !onBomb && bombNum > 0) {
			
			Bomb ();

		} else if (Input.GetAxis ("Fire2") == 1 && zaWarodu) {
		
			//StartCoroutine (TheWorld ());
			//zaWarodu = false;
		}
			
        if(isWiimote) {
			
            UseWiimote();

            if (wiimote.Accel.accel[0] > 650 && !onBomb && bombNum > 0) {
				
                Bomb();
            }
        }
	}

	/// <summary>
	/// Move the player.
	/// </summary>
	void Move() {

		float v = Input.GetAxis ("Vertical") * Time.deltaTime * speed;
		float h = Input.GetAxis ("Horizontal") * Time.deltaTime * speed;

		WalkAnim (ref v, h);
			
		controller.Move (myTransform.TransformDirection(new Vector3(-h, 0, -v)));
	}

	/// <summary>
	/// Rotate the camera of the player.
	/// </summary>
	void Look() {

		float v = Input.GetAxisRaw ("VerticalRight") * Time.deltaTime * 175.0f;
		float h = Input.GetAxisRaw ("HorizontalRight") * Time.deltaTime * 175.0f;

		myTransform.Rotate (new Vector3 (0, h, 0));

		myTransform.FindChild("Head").GetComponent<Camera> ().GetComponent<Transform> ().Rotate (new Vector3 (v, 0, 0));
	}
		
	/// <summary>
	/// Plant a bomb.
	/// </summary>
	void Bomb() {
			
		Vector3 pos = settings.RoundPosition (myTransform.position);

		CmdSpawnBomb (pos);

		bombNum--;
		onBomb = true;
	}

	/// <summary>
	/// Manages the animation of the walk.
	/// </summary>
	/// <param name="h">The value of the left joystick vertical axis.</param>
	/// <param name="v">The value of the left joystick horizontal axis.</param>
	void WalkAnim(ref float v, float h) {

		Animator walkAnim = GetComponent<Animator> ();

		if (v == 0 && h == 0) {

			walkAnim.SetBool ("Walking", false);
			walkAnim.SetFloat ("Speed", 1.0f);

		} else {

			walkAnim.SetBool ("Walking", true);
		
			if (v != 0)
				walkAnim.SetFloat ("Speed", speed / 8.0f * v * 10.0f);

			if (h != 0)
				walkAnim.SetFloat ("Speed", speed / 8.0f * h * 10.0f);
		}
	}

   /// <summary>
   /// Initiate the Wiimotes
   /// </summary>
    void InitWiimotes() {
		
        WiimoteManager.FindWiimotes();

        foreach (WiimoteApi.Wiimote remote in WiimoteManager.Wiimotes) {
			
            wiimote = remote;
            wiimote.SendPlayerLED(false, false, false, false);
        }

        //If there's no Wiimote detected
		if (wiimote == null) {
			
            isWiimote = false;
            return;
        }

        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        isWiimote = true;
    }

    /// <summary>
    /// Controls for the Wiimotes and Nunchuck
    /// </summary>
    void UseWiimote() {
		
        do {
            wiimoteData = wiimote.ReadWiimoteData();

            //Movements with the nunuchuck
            if (wiimote.current_ext == ExtensionController.NUNCHUCK) {
				
                nunchuck = wiimote.Nunchuck;
                float[] stick = nunchuck.GetStick01();
                speed = 2.0f;

                //Right & left
                if (Math.Abs(stick[0]) > 0.8f) {
					
                    controller.Move(myTransform.TransformDirection(Vector3.right * Time.deltaTime * speed));

                } else if (Math.Abs(stick[0]) < 0.2f) {
					
                    controller.Move(myTransform.TransformDirection(Vector3.left * Time.deltaTime * speed));
                }

                //Top & Bottom
                if (Math.Abs(stick[1]) > 0.8f) {
					
                    controller.Move(myTransform.TransformDirection(Vector3.forward * Time.deltaTime * speed));

                } else if (Math.Abs(stick[1]) < 0.2f) {
					
                    controller.Move(myTransform.TransformDirection(Vector3.back * Time.deltaTime * speed));
                }
            }
        } while (wiimoteData > 0);
    }

	/// <summary>
	/// Called if the player enter a trigger.
	/// </summary>
	/// <param name="collider">The GameObject who enter the trigger.</param>
	void OnTriggerEnter(Collider collider) {

		if (!isServer)
			return;

		if (collider.CompareTag ("Explosion")) {

			GameObject.Find ("Minimap Camera").GetComponent<Camera> ().rect = new Rect (Vector2.zero, new Vector2 (1.0f, 1.0f));
			Destroy (gameObject);

		} else if (collider.CompareTag ("The World")) {

			if (collider.GetComponent<TheWorld> ().owner != gameObject) {
				
				settings.timeStopped = true;
			}
		}
	}
		
	/// <summary>
	/// Called if the player exit a trigger.
	/// </summary>
	/// <param name="collider">The GameObject who exit the trigger.</param>
	void OnTriggerExit(Collider collider) {

		if (!isLocalPlayer)
			return;

		if (collider.CompareTag ("Bomb")) {
			
			collider.GetComponent<BoxCollider> ().isTrigger = false;
			onBomb = false;
		}
	}

	/// <summary>
	/// Called if the player collide with an object.
	/// </summary>
	/// <param name="hit">The collision between the player and the object.</param>
	void OnControllerColliderHit(ControllerColliderHit hit) {

		GameObject col = hit.gameObject;

		if (col.CompareTag ("Bomb")) {

			if (!isLocalPlayer || col.GetComponent<BombExplosion> ().pushing)
				return;

			if (!isServer)
				col.GetComponent<BombExplosion> ().Push (myTransform.position);

			col.GetComponent<BombExplosion> ().CmdPush (myTransform.position);

		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>The world.</returns>
	IEnumerator TheWorld() {
	
		GetComponent<AudioSource> ().volume = 0.5f;
		GetComponent<AudioSource> ().PlayOneShot (settings.theWorldSound);

		yield return new WaitForSeconds (1.8f);

		CmdSpawnTheWorld ();
	}
}