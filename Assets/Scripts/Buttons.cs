using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 
using UnityEngine.SceneManagement;
using System.Collections;

public class Buttons : MonoBehaviour {

	//Enum for the server's infos
	enum Button {Host, Client, IP, Exit};

	[SerializeField] public bool showGUI = true;
	[SerializeField] public int offsetX;
	[SerializeField] public int offsetY;

	private NetworkClient myClient;
	private NetworkManager manager;
	private GameObject myCanvas;

	void Awake() {
		manager = GameObject.Find ("Network Manager").GetComponent<NetworkManager>();
		myCanvas = GameObject.Find ("Canvas_main_menu");
	}
	//All functions for the menu's buttons

	/// <summary>
	/// Create and host a game
	/// </summary>
	public void Host() {
		manager.StartServer ();
	}

	/// <summary>
	/// Connect a client to a host
	/// </summary>
	public void Connect() {
		//manager.networkAddress = GameObject.Find("I_IP").GetComponent<InputField>().text;
		manager.networkAddress = myCanvas.transform.FindChild ("I_IP").GetComponent<InputField> ().text;
		manager.StartClient ();

		/*ClientScene.Ready (manager.client.connection);
		ClientScene.AddPlayer (1);*/
	}

	/// <summary>
	/// Exit the game	
	/// </summary>
	public void Exit () {
		Application.Quit ();
	}

	void OnGUI() {
		if(!showGUI) {
			return;
		}
		// Variables for position
		int xpos = 10 + offsetX;
		int ypos = 40 + offsetY;
		int spacing = 24;

		//Server's infos on host
		if(NetworkServer.active) {
			GUI.Label (new Rect(xpos, ypos, 300, 20), "Server : port = " + manager.networkPort);
			ypos += spacing;
		}
		//Server's infos on client
		if(NetworkClient.active) {
			GUI.Label (new Rect(xpos, ypos, 300, 20), "Client : adress = " + manager.networkAddress + " port = " + manager.networkPort);
			ypos += spacing;
		}

		//On client and scene is not ready
		if(NetworkClient.active && !ClientScene.ready) {
			if(GUI.Button(new Rect(xpos, ypos, 200, 20), "Client is ready")) {
				ClientScene.Ready (manager.client.connection);
				if(ClientScene.localPlayers.Count == 0) {
					ClientScene.AddPlayer (0);
				}
			}
			ypos += spacing;
		}

		//Client or server are ready
		if(NetworkServer.active || NetworkClient.active) {
			if(GUI.Button(new Rect(xpos, ypos, 200, 20), "Exit")) {
				manager.StopHost();
			}
			ypos += spacing;
		}
	}
		
}
