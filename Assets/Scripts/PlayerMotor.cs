using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CharacterController), typeof(Player))]
public class PlayerMotor : MonoBehaviour {

	private float speed = 0;

	private Transform myTransform;
	private CharacterController controller;
	private Player player;
	private Animator anim;

	[SerializeField]
	private Camera playerCamera;
	[SerializeField]
	private float lookSensitivity = 300.0f;

	void Awake() {

		myTransform = GetComponent<Transform> ();
		controller = GetComponent<CharacterController> ();
		player = GetComponent<Player> ();
		anim = GetComponent<Animator> ();
	}

	void Update() {

		Move ();
		Look ();

		Vector3 pos = controller.transform.position;
		if(pos.y != 0)
			controller.transform.position -= new Vector3(0, pos.y + 1.0f, 0);
	}

	void Move() {

        float playerSpeed = 1 + 0.1f * ((float)player.speed - 1);
		float hAxis = Input.GetAxisRaw ("Horizontal") * playerSpeed * 7.0f * Time.deltaTime;
		float vAxis = Input.GetAxisRaw ("Vertical") * playerSpeed * 7.0f * Time.deltaTime;

		Walk (hAxis, vAxis);

		controller.Move (myTransform.TransformDirection (new Vector3(-hAxis, 0, -vAxis)));
	}

	void Look() {

		float hAxis = Input.GetAxisRaw ("Controller H") * lookSensitivity * Time.deltaTime;
		float vAxis = Input.GetAxisRaw ("Controller V") * lookSensitivity * Time.deltaTime;

		playerCamera.transform.Rotate (new Vector3 (hAxis, 0));
		myTransform.transform.Rotate (new Vector3 (0, vAxis));
	}

	void Walk(float hAxis, float vAxis) {

		if (hAxis != 0 || vAxis != 0) {

			if (Mathf.Abs (hAxis) > 0.1f)
				hAxis = 0.1f * Mathf.Sign (hAxis);

			if (Mathf.Abs (vAxis) > 0.1f)
				vAxis = 0.1f * Mathf.Sign (vAxis);

			hAxis /= 0.1f;
			vAxis /= 0.1f;

			float maxAxis = Mathf.Max (Mathf.Abs (hAxis), Mathf.Abs (vAxis));
			float maxAxisSign = 0;

			if (Mathf.Abs (hAxis) == maxAxis)
				maxAxisSign = Mathf.Sign (hAxis);
			else
				maxAxisSign = Mathf.Sign (vAxis);

			float test = maxAxis * maxAxisSign * player.speed;

			anim.SetFloat ("speed", test);
			anim.SetBool ("walking", true);

		} else {
			
			anim.SetFloat ("speed", 1.0f);
			anim.SetBool ("walking", false);
		}
	}
}
