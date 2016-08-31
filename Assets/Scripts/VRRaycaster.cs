using UnityEngine;

public class VRRaycaster : MonoBehaviour {

    [SerializeField]
    private Texture reticule;
    private Rect posReticule;

    private VRInput target = null;

    void Start() {

        posReticule = new Rect((Screen.width - reticule.width) / 2, (Screen.height - reticule.height) / 2, reticule.width, reticule.height);
    }

	void Update() {

        if(Raycast() == null && target != null)
            target.hover = false;

        target = Raycast();

        if(target != null) {

            if(Input.GetButton("Fire1"))
                target.Click();
            else {
                target.Hover();
                target.hover = true;
            }
        }
	}

    VRInput Raycast() {

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);
        VRInput input = null;

        foreach(RaycastHit hit in hits)
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
                input = hit.collider.GetComponent<VRInput>();

        return input;
    }

    void OnGUI() {

        GUI.DrawTexture(posReticule, reticule);
    }
}
