using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour {

	private const string PLAYER_ID_PREFIX = "Player ";

    //public GameObject mainCamera;

    //private bool test = false;
    public GameObject UI;
    public static GameObject winUI;
    public Text playerUI;

    public static bool hasGameStarted = false;
    public static bool isGameFinished = false;
    public static bool hasGameEnded = false;
    public static bool isGamePaused = false;
	public static bool isTimeStopped = false;

    private static Player[] playersTab;
	public static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public static Dictionary<Player, int> playersIDs = new Dictionary<Player, int>();

    void Awake() {

        UpdatePlayersTab();
        winUI = UI.transform.Find("Win").gameObject;
    }

    void Update() {

        if(playersTab != null)
            if (playersTab.Length > 1 && !hasGameStarted) {

                hasGameStarted = true;
                Debug.Log("Game started!");
                Time.timeScale = 1;
            }

        if(hasGameStarted && !isGameFinished && playersTab.Length <= 1) {

            isGameFinished = true;

            if(playersTab.Length == 1)
                StartCoroutine(StartWin(playersTab[0]));
            else
                StartCoroutine(StartWin());
        }
    }

    /// <summary>
    /// Registers a player.
    /// </summary>
    /// <param name="netID">The "netId" of the player</param>
    /// <param name="player">The player to registers</param>
	public static void RegisterPlayer(string netID, Player player) {
	
		string playerID = PLAYER_ID_PREFIX + netID;
		players.Add (playerID, player);
        playersIDs.Add(player, players.Count);
		player.transform.name = playerID;

        UpdatePlayersTab();

		Debug.Log (playerID + " registered!");
	}

    /// <summary>
    /// Unregisters a player.
    /// </summary>
    /// <param name="playerID">The player to unregister</param>
	public static void UnregisterPlayer(string playerID) {

        playersIDs.Remove (GetPlayer(playerID));
		players.Remove (playerID);

        UpdatePlayersTab();

        Debug.Log(playerID + " unregistered!");
	}

    /// <summary>
    /// Return the player.
    /// </summary>
    /// <param name="playerID">The name of the player (with netId)</param>
    /// <returns></returns>
	public static Player GetPlayer(string playerID) {

		if (players.ContainsKey (playerID))
			return players [playerID];
		else
			return null;
	}

    /// <summary>
    /// Round a position to the grid of the game.
    /// </summary>
    /// <param name="pos">The position to round</param>
    /// <returns></returns>
	public static Vector3 RoundPosition(Vector3 pos) {

		// Round the coordinates x and z
		float roundX = Mathf.Round (pos.x / 2.0f) * 2.0f;
		float roundZ = Mathf.Round (pos.z / 2.0f) * 2.0f;

		Vector3 newPos = new Vector3 (roundX, pos.y, roundZ);

		return newPos;
	}

    /// <summary>
    /// Return the name of the player.
    /// </summary>
    /// <param name="player">The player to get the name</param>
    /// <returns></returns>
    public static string GetPlayerName(Player player) {

        return PLAYER_ID_PREFIX + playersIDs[player];
    }

    /// <summary>
    /// Update the array of the players.
    /// </summary>
    private static void UpdatePlayersTab() {

        playersTab = new Player[players.Count];
        players.Values.CopyTo(playersTab, 0);
    }

    /// <summary>
    /// Kills a player.
    /// </summary>
    /// <param name="player">The player to kill</param>
	public static void Kill(Player player) {

		player.Die ();
	}

    /// <summary>
    /// Adds a number of bombs to a player.
    /// </summary>
    /// <param name="player">The player to add bombs</param>
    /// <param name="howMuch">The number of bombs to add</param>
	public static void AddNumBomb(Player player, int howMuch) {

		player.numBomb += howMuch;
	}

    /// <summary>
    /// Adds a number of "Range" to a player.
    /// </summary>
    /// <param name="player">The player to add "Range"</param>
    /// <param name="howMuch">The number of "Range" to add</param>
	public static void AddRange(Player player, int howMuch) {

		player.range += howMuch;
	}

    /// <summary>
    /// Adds a number of "Speed" to a player.
    /// </summary>
    /// <param name="player">The player to add "Speed"</param>
    /// <param name="howMuch">The number of "Speed" to add</param>
	public static void AddSpeed(Player player, int howMuch) {

		player.speed += howMuch;
    }

    public static void Win(string winner) {

        if(winner != "")
            winUI.GetComponentInChildren<Text> ().text = winner + " wins!";
        else
            winUI.GetComponentInChildren<Text> ().text = "Equality!";

        winUI.SetActive(true);
        hasGameEnded = true;
    }

    IEnumerator StartWin(Player player = default(Player)) {

        yield return new WaitForSeconds(3.0f);

        string winnerName = "";

        if(player != default(Player))
            winnerName = player.name;

        Win(winnerName);
    }
}
