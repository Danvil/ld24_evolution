using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {

	public AudioClip audioDissolve;

	public const float cHealthPerBubble = 0.05f;

	public const float cLifetime = 7.0f;

	float currentcLifetime = cLifetime + 0.4f*Random.value;

	public bool IsFollowing { get; set; }

	public bool IsGood { get { return IsFollowing; } }

	GameObject particle;

	public Color Color {
		get {
			return renderer.material.color;
		}
		set {
			renderer.material.color = value;
			particle.renderer.material.SetColor("_TintColor", value);
		}
	}

	public float Alpha {
		set {
			Color col = particle.renderer.material.GetColor("_TintColor");
			col.a = value;
			particle.renderer.material.SetColor("_TintColor", col);
		}
	}

	public float Health { get; set;	}

	public void SetPoisonBubble() {
		IsFollowing = false;
		Color = Color.red;
		Health = -cHealthPerBubble;
	}

	public void SetHealthBubble() {
		IsFollowing = true;
		Color = Color.green;
		Health = +cHealthPerBubble;
	}

	public Vector3 velocity;

	void Awake() {
		particle = this.transform.FindChild("BubbleParticle").gameObject;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// delete bubble if too old
		currentcLifetime -= MyTime.deltaTime;
		if(currentcLifetime < 0) {
			Globals.BubbleManager.Remove(this.gameObject);
			Destroy(this.gameObject);
		}
		// move with velocity
		this.transform.position += MyTime.deltaTime * velocity;
		velocity *= Mathf.Exp(-2.0f*MyTime.deltaTime);
		// follow player
		if(IsFollowing) {
			Vector3 d = (Globals.Player.transform.position - this.transform.position);
			float m = d.magnitude;
			if(m < 3.0f) {
				float f = Mathf.Exp(-m);
				this.transform.position += MyTime.deltaTime * f * d;
			}
		}
		// set alpha according to lifetime
		float q = Mathf.Min(currentcLifetime, 3.0f);
		Alpha = MoreMath.Parabel(q, 3.0f, 1.0f, 0.0f, 0.0f);
	}

}
