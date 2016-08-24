using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	private const string REMOTE_LAYER_NAME = "RemotePlayer";

	[SerializeField]
	private Behaviour[] componentsToDisable;

	private Camera sceneCamera;

	void Awake() {
	
		sceneCamera = Camera.main;
	}

	void Start() {
	
		if (!isLocalPlayer) {
			
			DisableComponents ();
			AssignRemoteLayer ();

		} else {

            string playerName = GameManager.GetPlayerName(GameManager.GetPlayer(gameObject.name));
            GameObject.Find("Player").GetComponentInChildren<Text>().text = playerName;

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

	void AssignRemoteLayer() {
		gameObject.layer = LayerMask.NameToLayer(REMOTE_LAYER_NAME);
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
