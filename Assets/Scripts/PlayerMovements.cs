using UnityEngine;
using WiimoteApi;
using System;
using UnityEngine.UI;
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

	private Collider currentBomb;
	private bool onBomb = false;

	private bool throwing = false;
	private BombExplosion bombBE;

    //Wiimote
    private WiimoteApi.Wiimote wiimote;
    private NunchuckData nunchuck;
    private int wiimoteData;
    private bool isWiimote = false;

	/// <summary>
	/// Make the server spawn a bomb.
	/// </summary>
	/// <param name="x">The x coordinate for the bomb.</param>
	/// <param name="z">The z coordinate for the bomb.</param>
	[Command]
	void CmdSpawnBomb(float x, float z) {

		GameObject bomb = Instantiate (settings.bombPrefab);
		bomb.transform.position = new Vector3 (x, 0.75f, z);
		bomb.GetComponent<BombExplosion> ().playerMove = GetComponent<PlayerMovements> ();
		bomb.GetComponent<BombExplosion> ().bombPower = bombPower;
		bomb.name = "Bomb";

		NetworkServer.SpawnWithClientAuthority (bomb, connectionToClient);
	}

	/// <summary>
	/// Make the server pause the game.
	/// </summary>
	[Command]
	void CmdPause() {
	
		settings.gamePaused = false;
	}

	/// <summary>
	/// Make the server spawn an explosion.
	/// </summary>
	/// <param name="x">The x coordinate for the explosion.</param>
	/// <param name="z">The z coordinate for the explosion.</param>
	/// <param name="parent">The parent of the explosion.</param>
	[Command]
	public void CmdSpawnExplo(float x, float z, GameObject parent) {

		var explo = Instantiate (settings.exploPrefab);
		explo.transform.parent = parent.transform;
		explo.transform.position = new Vector3(x, 1f, z);
		explo.name = "Explosion";

		NetworkServer.Spawn (explo);

		Destroy (explo, 1.0f);
	}

	/// <summary>
	/// Make the server spawn a bonus.
	/// </summary>
	/// <param name="position">The position for the bonus.</param>
	[Command]
	public void CmdSpawnBonus(Vector3 position) {

		int randBonus = (int)UnityEngine.Random.Range (0, 2.999f);

		var currentBonus = Instantiate(settings.bonusPrefabs [randBonus]);
		currentBonus.transform.position = position;
		currentBonus.transform.eulerAngles += new Vector3 (15.0f, 0, 0);
		currentBonus.name = settings.bonusPrefabs[randBonus].name;

		NetworkServer.Spawn (currentBonus);
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

		// Correct the minimap rotation
		settings.playerHUD[1].transform.eulerAngles = new Vector3 (90.0f, transform.eulerAngles.y, 0);

		// Create the camera
		var cam = new GameObject ();
		cam.AddComponent<Camera> ();
		cam.AddComponent<FlareLayer> ();
		cam.AddComponent<AudioListener> ();
		cam.transform.position = myTransform.position + new Vector3 (0, 0.6f, -0.1f);
		cam.transform.rotation = myTransform.rotation;
		cam.transform.parent = myTransform;
		cam.name = "Head";

		// Set the lamp position and the parent
		Transform lamp = myTransform.FindChild ("Lamp");
		lamp.GetComponent<Transform> ().position += new Vector3(0, 0.6f, 0);
		lamp.transform.parent = cam.transform;

		// Check if the night mode is activated
		if (settings.nightMode)
			lamp.gameObject.SetActive (true);

        //Initialise Wiimotes
        InitWiimotes();
	}

	void Update() {
		
		if (!isLocalPlayer || settings.gamePaused)
			return;

		Collider[] maps = Physics.OverlapSphere (myTransform.position, 3.0f);

		foreach (Collider map in maps) {
		
			if (map.gameObject.layer == 9)
				map.gameObject.layer = 0;
		}

		Move ();
		Look ();
        if(Input.GetAxis("Fire1") == 1 && !onBomb && bombNum > 0) {
            Bomb();
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

		float h = Input.GetAxis ("Horizontal") * Time.deltaTime * speed;
		float v = Input.GetAxis ("Vertical") * Time.deltaTime * speed;

		controller.Move (myTransform.TransformDirection(new Vector3(h, 0, v)));
	}

	/// <summary>
	/// Rotate the camera of the player.
	/// </summary>
	void Look() {

		float h = Input.GetAxisRaw ("HorizontalRight") * Time.deltaTime * 175.0f;
		float v = Input.GetAxisRaw ("VerticalRight") * Time.deltaTime * 175.0f;

		myTransform.Rotate (new Vector3 (0, h, 0));

		GameObject.Find ("Head").GetComponent<Camera> ().GetComponent<Transform> ().Rotate (new Vector3 (v, 0, 0));
	}
		
	/// <summary>
	/// Plant a bomb.
	/// </summary>
	void Bomb() {
		
			
			Vector3 pos = settings.RoundPosition (myTransform.position);

			bombNum--;
				
			CmdSpawnBomb (pos.x, pos.z);

			onBomb = true;
	}

   /// <summary>
   /// Initiate the Wiimotes
   /// </summary>
    void InitWiimotes()
    {
        WiimoteManager.FindWiimotes();
        foreach (WiimoteApi.Wiimote remote in WiimoteManager.Wiimotes)
        {
            wiimote = remote;
            wiimote.SendPlayerLED(false, false, false, false);
        }
        //If there's no Wiimote detected
        if (wiimote == null)
        {
            isWiimote = false;
            return;
        }
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        isWiimote = true;
    }

    /// <summary>
    /// Controls for the Wiimotes and Nunchuck
    /// </summary>
    void UseWiimote()
    {
        do
        {
            wiimoteData = wiimote.ReadWiimoteData();
            //Movements with the nunuchuck
            if (wiimote.current_ext == ExtensionController.NUNCHUCK)
            {
                nunchuck = wiimote.Nunchuck;
                float[] stick = nunchuck.GetStick01();
                speed = 2.0f;

                //Right & left
                if (Math.Abs(stick[0]) > 0.8f)
                {
                    controller.Move(myTransform.TransformDirection(Vector3.right * Time.deltaTime * speed));
                }
                else if (Math.Abs(stick[0]) < 0.2f)
                {
                    controller.Move(myTransform.TransformDirection(Vector3.left * Time.deltaTime * speed));
                }
                //Top & Bottom
                if (Math.Abs(stick[1]) > 0.8f)
                {
                    controller.Move(myTransform.TransformDirection(Vector3.forward * Time.deltaTime * speed));
                }
                else if (Math.Abs(stick[1]) < 0.2f)
                {
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

		if (!isLocalPlayer)
			return;

		if (collider.CompareTag ("Bomb")) {

			currentBomb = collider;

		} else if (collider.CompareTag ("Explosion")) {

			GameObject.Find ("Minimap Camera").GetComponent<Camera> ().rect = new Rect(Vector2.zero, new Vector2(1.0f, 1.0f));
			Destroy (gameObject);
		}
	}
		
	/// <summary>
	/// Called if the player exit a trigger.
	/// </summary>
	/// <param name="collider">The GameObject who exit the trigger.</param>
	void OnTriggerExit(Collider collider) {

		if (!isLocalPlayer)
			return;

		currentBomb = default(Collider);

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

			if (!isServer) {

				col.GetComponent<BombExplosion> ().Push (myTransform.position);
			}

			col.GetComponent<BombExplosion> ().CmdPush (myTransform.position);

		} else if (col.CompareTag ("Wall") || col.CompareTag ("Brick")) {
		
			col.layer = 0;
		}
	}
}