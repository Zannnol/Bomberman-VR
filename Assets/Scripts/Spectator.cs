using UnityEngine;
using System.Collections.Generic;

public class Spectator : MonoBehaviour {

	void Update() {

        if(!GameManager.hasGameEnded) {

		    Player[] players = new Player[GameManager.players.Count];
		    GameManager.players.Values.CopyTo(players, 0);

		    Vector3[] pos = new Vector3[players.Length];
		    for (int i = 0; i < players.Length; i++)
			    pos [i] = players [i].transform.position;

		    Vector3 center = FindCenterPoint (pos);
            if (pos.Length > 0) {

                float cameraHeight = Vector3.Distance(center, pos[0]) + 11.0f;
                transform.position = new Vector3(center.x, cameraHeight, center.z);

            } else {

                transform.position = new Vector3(14.0f, 24.0f, 12.0f);
            }
        }
	}

	Vector3 FindCenterPoint(Vector3[] pos) {

		Vector3 center = Vector3.zero;

        if (pos.Length > 0) {

            Bounds bounds = new Bounds(pos[0], Vector3.zero);
            for (int i = 1; i < pos.Length; i++)
                bounds.Encapsulate(pos[i]);
            center = bounds.center;

        }

		return center;
	}
}
