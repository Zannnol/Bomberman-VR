using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

    [SerializeField]
    private GameObject head;

    [SerializeField]
    private Text HUDNumBomb,
                 HUDRange,
                 HUDSpeed;

	private bool _isDead = false;
	public bool isDead {
		get { return _isDead; }
		set { _isDead = value; }
	}

	[SyncVar(hook="OnChangeNumBomb")]
	private int _numBomb = 1;
	public int numBomb {
		get { return _numBomb; }
		set {
            _numBomb = value;
            HUDNumBomb.text = _numBomb.ToString();
        }
	}

	private int _range = 1;
	public int range {
		get { return _range; }
		set {
            _range = value;
            HUDRange.text = _range.ToString();
        }
	}
		
	private int _speed = 1;
	public int speed {
		get { return _speed; }
		set {
            _speed = value;
            HUDSpeed.text = _speed.ToString();
        }
	}

	private bool _onBomb = false;
	public bool onBomb {
		get { return _onBomb; }
		set { _onBomb = value; }
	}

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

    [SerializeField]
    private Animator anim;

    void Update() {

        Collider[] cover = Physics.OverlapSphere(transform.position, 3.0f);
        foreach(Collider col in cover)
            if(LayerMask.LayerToName(col.gameObject.layer) == "Hidden")
                col.gameObject.layer = LayerMask.NameToLayer("Default");
    }

	void Setup() {
	
		wasEnabled = new bool[disableOnDeath.Length];
		for (int i = 0; i < disableOnDeath.Length; i++)
			wasEnabled [i] = disableOnDeath [i].enabled;

		SetDefaults ();
	}

	public void Die() {
	
		_isDead = true;

		for (int i = 0; i < disableOnDeath.Length; i++)
			disableOnDeath [i].enabled = false;

		Collider col = GetComponent<Collider> ();
		if (col != null)
			col.enabled = true;

        anim.Play("Death");
        GetComponentInChildren<Camera> ().transform.parent = head.transform;

		Debug.Log (transform.name + " is dead!");
	}

	void SetDefaults() {
	
		_isDead = false;

		for (int i = 0; i < disableOnDeath.Length; i++)
			disableOnDeath [i].enabled = wasEnabled [i];

		Collider _col = GetComponent<Collider> ();
		if (_col != null)
			_col.enabled = true;
	}

    void OnChangeNumBomb(int numBomb) {

        // Actualiser l'HUD du joueur
    }

    void OnTriggerEnter(Collider col) {

        if(col.tag == "Explosion")
            Die();
    }

    public void SetColliderBomb() {

        //print("test");
        //print(bomb.GetComponent<NetworkIdentity>().netId);
    }
}
