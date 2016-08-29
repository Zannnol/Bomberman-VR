using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	private const string REMOTE_LAYER_NAME = "RemotePlayer";
    private const string LOCAL_UI_LAYER_NAME = "LocalUI";

    [SerializeField]
    private GameObject HUD;

    private Text HUDPlayerID;

	[SerializeField]
	private Behaviour[] componentsToDisable;

	private Camera sceneCamera;

	void Awake() {
	
		sceneCamera = Camera.main;
        HUDPlayerID = HUD.transform.GetChild(0).GetComponent<Text>();
	}

	void Start() {
	
		if (!isLocalPlayer) {
			
			DisableComponents ();
			AssignRemoteLayer (gameObject.transform.GetChild(2).gameObject, REMOTE_LAYER_NAME);

		} else {

            string playerName = GameManager.GetPlayerName(GameManager.GetPlayer(gameObject.name));
            HUDPlayerID.text = playerName;
            AssignRemoteLayer(HUD, LOCAL_UI_LAYER_NAME);

			if (sceneCamera != null)
				sceneCamera.gameObject.SetActive (false);
		}
	}

	public override void OnStartClient() {
	
		base.OnStartClient ();

		string netID = GetComponent<NetworkIdentity> ().netId.ToString ();
		Player player = GetComponent<Player> ();

		GameManager.RegisterPlayer (netID, player);
	}

	void AssignRemoteLayer(GameObject go, string layerName) {

        LayerMask layer = LayerMask.NameToLayer(layerName);
		go.layer = layer;
	}

	void DisableComponents () {
	
		for (int i = 0; i < componentsToDisable.Length; i++)
			componentsToDisable [i].enabled = false;
	}

	void OnDisable() {

		if (sceneCamera != null)
			sceneCamera.gameObject.SetActive (true);

		GameManager.UnregisterPlayer (transform.name);
	}
}
