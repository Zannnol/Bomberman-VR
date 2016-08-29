using UnityEngine;
using System.Collections;

public class BrickExplo : MonoBehaviour {
    
	private float timeLeft = 2.0f;

	void Update () {
	
        timeLeft -= Time.deltaTime;

        foreach(Transform child in transform) {

            Renderer rend = child.GetComponent<Renderer>();
            foreach(Material childMat in rend.materials) {
                childMat.shader = Shader.Find("Legacy Shaders/Transparent/Bumped Diffuse");
                childMat.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, timeLeft / 2.0f));
            }
        }

        if (timeLeft <= 0) {

            Destroy(gameObject);
        }
	}
}
