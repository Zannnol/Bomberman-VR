using UnityEngine;

public class VRInput : MonoBehaviour {

    public bool hover;

    private Renderer rend;
    private Color color;
    private Color hoverColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color clickColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);

    void Awake() {
        rend = GetComponent<MeshRenderer>();
        color = rend.material.GetColor("_Color");
    }

    void Update() {

        if(!hover)
            rend.material.SetColor("_Color", color);
    }

    public void Hover() {
        rend.material.SetColor("_Color", hoverColor);
    }

	public void Click() {
        if(hover) {
            rend.material.SetColor("_Color", clickColor);
        }
    }
}
